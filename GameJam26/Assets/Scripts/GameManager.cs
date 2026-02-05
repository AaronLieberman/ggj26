using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public enum GameState { STARTED, ENDED }

public class GameManager : MonoBehaviour
{
    private ShopManager shopManager;
    private ConveyorManager conveyorManager;
    private MaskDisplayManager maskDisplayManager;

    [SerializeField] private int customersToServe;
    private int customersServed = 0;
    private GameState gameState;

    [SerializeField] private float preStartGameTransitionDuration = 2;
    [SerializeField] private float startGameTransitionDuration = 2;
    [SerializeField] private float endGameTransitionDuration = 0.5f;
    private CanvasGroup startingGameCanvasGroup;
    private CanvasGroup endingGameCanvasGroup;

    [SerializeField] private float shopStartSeconds = 2;

    [SerializeField] private Vector2 endingMaskPieceScaling;
    [SerializeField] private Vector2 endingMaskSizing;
    [SerializeField] private Vector2 endingMaskOffset;
    [SerializeField] private GameObject endingCustomerViewerPrefab;
    private Transform endingCustomerHolder;
    [SerializeField] private Sprite[] endingCustomerSatisfactionSprites;
    private TextMeshProUGUI endingCustomerScoreText;

    [SerializeField] private Vector2 customerMaskSizing;

    private void Awake()
    {
        shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
        conveyorManager = Utilities.GetRootComponentRecursive<ConveyorManager>();
        maskDisplayManager = Utilities.GetRootComponentRecursive<MaskDisplayManager>();

        startingGameCanvasGroup = GameObject.Find("StartingGameUI").GetComponent<CanvasGroup>();
        endingGameCanvasGroup = GameObject.Find("EndingGameUI").GetComponent<CanvasGroup>();
        endingCustomerScoreText = GameObject.Find("EndOfGameScoreEarned").GetComponent<TextMeshProUGUI>();

        endingCustomerHolder = GameObject.Find("EndofGameCustomersHolder").transform;
    }

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    private void Update()
    {
        if (gameState == GameState.STARTED && customersServed < customersToServe)
        {
            if (shopManager.AttemptCustomerSpawn())
            {
                customersServed++;
            }
        }

        if (gameState != GameState.ENDED && customersServed >= customersToServe && !shopManager.CustomerExists)
        {
            EndGame();
        }
    }

    private IEnumerator StartGame()
    {
        Debug.Log("Starting game");

        startingGameCanvasGroup.alpha = 1;
        StartCoroutine(FadeUI(startingGameCanvasGroup, 1.0f, 0.0f, preStartGameTransitionDuration, startGameTransitionDuration));
        startingGameCanvasGroup.interactable = false;

        conveyorManager.ActivateManager();

        gameState = GameState.STARTED;

        yield return new WaitForSeconds(shopStartSeconds);
        shopManager.ActivateManager();
    }

    private void EndGame()
    {
        Debug.Log("Ending game");

        StartCoroutine(FadeUI(endingGameCanvasGroup, 0.0f, 1.0f, 0, endGameTransitionDuration));
        endingGameCanvasGroup.interactable = true;
        PopulateEndingShopCustomers();

        shopManager.DeactivateManager();
        conveyorManager.DeactivateManager();

        Utilities.GetRootComponentRecursive<PauseManager>().ShowRestartButton();

        gameState = GameState.ENDED;
    }

    private IEnumerator FadeUI(CanvasGroup canvasGroup, float startingAlpha, float endingAlpha, float waitDuration, float duration)
    {
        if (waitDuration > 0)
        {
            Time.timeScale = 0;
            // wait 2s (ignores timeScale)
            yield return new WaitForSecondsRealtime(waitDuration);
            Time.timeScale = 1f;
        }

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            float t = Mathf.Clamp01(timeElapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startingAlpha, endingAlpha, t);

            yield return null;
        }
    }

    private void PopulateEndingShopCustomers()
    {
        List<CustomerResult> customerResults = new List<CustomerResult>();
        customerResults = shopManager.GetComponentsInChildren<Customer>().Select(customer => customer.CustomerResult).ToList();

        int totalScore = 0;
        foreach (CustomerResult customerResult in customerResults)
        {
            if (customerResult.CustomerData == null)
            {
                continue;
            }

            GameObject endingCustomerViewer = Instantiate(endingCustomerViewerPrefab, Vector3.zero, Quaternion.identity, endingCustomerHolder);
            endingCustomerViewer.GetComponent<Image>().sprite = shopManager.GetCustomerSprite(customerResult.CustomerData.customerImageName);
            endingCustomerViewer.transform.Find("CustomerSatisfactionUI").GetComponent<Image>().sprite 
                = customerResult.CustomerScore > 0 ? endingCustomerSatisfactionSprites[0] : endingCustomerSatisfactionSprites[1];

            totalScore += customerResult.CustomerScore;

            if (customerResult.CustomerMask)
            {
                GameObject endingCustomerMask = Instantiate(customerResult.CustomerMask, endingCustomerViewer.transform);
                RectTransform maskRect = endingCustomerMask.GetComponent<RectTransform>();
                maskRect.anchoredPosition = endingMaskOffset;
                maskRect.sizeDelta = endingMaskSizing;
                endingCustomerMask.transform.Find("MountPoints").GetComponent<RectTransform>().localScale = endingMaskPieceScaling;
            }
        }

        endingCustomerScoreText.text = string.Format("Score Earned: {0}", totalScore);
    }

    public bool DeliverMask()
    {
        if (shopManager.MaskIsDeliverable)
        {
            var mask = GameObject.Find("MaskDisplay").transform.GetComponentInChildren<Mask>();

            Transform customerTransform = shopManager.CurrentCustomer.transform;
            var maskCenter = customerTransform.Find("CustomerMaskCenter");
            shopManager.CurrentCustomer.StopTimer();
            StartCoroutine(LerpTo(shopManager.CurrentCustomer, mask, maskCenter.transform, 0.5f));
            return true;
        }

        return false;
    }

    public IEnumerator LerpTo(Customer customer, Mask maskGameObject, Transform target, float duration)
    {
        float time = 0;
        Vector3 startPos = maskGameObject.transform.position;
        Vector3 targetPos = target.position;

        while (time < duration)
        {
            float t = time / duration;
            t = t * t * (3f - 2f * t);
            maskGameObject.transform.position = Vector2.Lerp(startPos, targetPos, t);

            time += Time.deltaTime;
            yield return null;
        }

        maskGameObject.transform.position = targetPos;
        maskGameObject.transform.SetParent(target, true);

        var anim = customer.gameObject.AddComponent<CelebrationAnim>();
        anim.StartCelebration(1.5f);

        yield return new WaitForSeconds(1.7f);
        Utilities.GetRootComponentRecursive<ShopManager>().CustomerSatisfied(maskGameObject);
        maskDisplayManager.ClearMaskDisplay();
    }
}
