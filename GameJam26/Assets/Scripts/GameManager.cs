using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { STARTED, ENDED }

public class GameManager : MonoBehaviour
{
    private ShopManager shopManager;
    private ConveyorManager conveyorManager;

    [SerializeField] private int customersToServe;
    private int customersServed = 0;
    private GameState gameState;

    [SerializeField] private float startGameTransitionDuration;
    [SerializeField] private float endGameTransitionDuration;
    private CanvasGroup startingGameCanvasGroup;
    private CanvasGroup endingGameCanvasGroup;

    [SerializeField] private Vector2 endingMaskPieceScaling;
    [SerializeField] private Vector2 endingMaskSizing;
    [SerializeField] private GameObject endingCustomerViewerPrefab;
    private Transform endingCustomerHolder;

    [SerializeField] private Vector2 customerMaskSizing;

    private void Awake()
    {
        shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
        conveyorManager = Utilities.GetRootComponentRecursive<ConveyorManager>();

        startingGameCanvasGroup = GameObject.Find("StartingGameUI").GetComponent<CanvasGroup>();
        endingGameCanvasGroup = GameObject.Find("EndingGameUI").GetComponent<CanvasGroup>();

        endingCustomerHolder = GameObject.Find("EndofGameCustomersHolder").transform;
    }

    private void Start()
    {
        StartGame();
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

    private void StartGame()
    {
        Debug.Log("Starting game");

        StartCoroutine(FadeUI(startingGameCanvasGroup, 1.0f, 0.0f, startGameTransitionDuration));
        startingGameCanvasGroup.interactable = false;

        shopManager.ActivateManager();
        conveyorManager.ActivateManager();

        gameState = GameState.STARTED;
    }

    private void EndGame()
    {
        Debug.Log("Ending game");

        StartCoroutine(FadeUI(endingGameCanvasGroup, 0.0f, 1.0f, endGameTransitionDuration));
        endingGameCanvasGroup.interactable = true;
        PopulateEndingShopCustomers();

        shopManager.DeactivateManager();
        conveyorManager.DeactivateManager();

        gameState = GameState.ENDED;
    }

    private IEnumerator FadeUI(CanvasGroup canvasGroup, float startingAlpha, float endingAlpha, float duration)
    {
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
        // TODO: Replace with call to shop manager to receive customers results.
        List<CustomerResult> customerResults = new List<CustomerResult>();
        GameObject maskGameObject = Utilities.GetRootComponentRecursive<Mask>().gameObject;
        CustomerData customerData1 = new CustomerData();
        customerData1.customerImageName = shopManager.Sprites[0].name;
        customerResults.Add(
            new CustomerResult(
                customerData1,
                maskGameObject,
                false
            )
        );

        CustomerData customerData2 = new CustomerData();
        customerData2.customerImageName = shopManager.Sprites[1].name;
        customerResults.Add(
            new CustomerResult(
                customerData2,
                maskGameObject,
                true
            )
        );

        foreach (CustomerResult customerResult in customerResults)
        {
            GameObject endingCustomerViewer = Instantiate(endingCustomerViewerPrefab, Vector3.zero, Quaternion.identity, endingCustomerHolder);
            endingCustomerViewer.GetComponent<Image>().sprite = shopManager.GetCustomerSprite(customerResult.CustomerData.customerImageName);
            endingCustomerViewer.transform.Find("CustomerSatisfactionUI").GetComponent<Image>().color = customerResult.Satisfied ? Color.green : Color.red;

            GameObject endingCustomerMask = Instantiate(customerResult.CustomerMask, Vector3.zero, Quaternion.identity, endingCustomerViewer.transform);
            endingCustomerMask.GetComponent<RectTransform>().sizeDelta = endingMaskSizing;
            endingCustomerMask.transform.Find("MountPoints").GetComponent<RectTransform>().localScale = endingMaskPieceScaling;
        }
    }

    public void DeliverMask()
    {
        if (shopManager.MaskIsDeliverable)
        {
            GameObject maskGameObject = GameObject.Find("MaskDisplay").transform.GetComponentInChildren<Mask>().gameObject;
            Transform customerTransform = shopManager.CurrentCustomer.transform;
            GameObject customerMask = Instantiate(maskGameObject, Vector3.zero, Quaternion.identity, customerTransform);
            customerMask.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            customerMask.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            customerMask.GetComponent<RectTransform>().sizeDelta = customerMaskSizing;
        }
    }
}
