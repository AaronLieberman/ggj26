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

    [SerializeField] private int gameLengthSeconds;
    private float gameEndTime;
    private GameState gameState;

    [SerializeField] private float startGameTransitionDuration;
    [SerializeField] private float endGameTransitionDuration;
    private CanvasGroup startingGameCanvasGroup;
    private CanvasGroup endingGameCanvasGroup;

    [SerializeField] private GameObject endingCustomerViewerPrefab;
    private Transform endingCustomerHolder;

    private void Awake()
    {
        shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
        conveyorManager = Utilities.GetRootComponentRecursive<ConveyorManager>();

        startingGameCanvasGroup = GameObject.Find("StartingGameUI").GetComponent<CanvasGroup>();
        endingGameCanvasGroup = GameObject.Find("EndingGameUI").GetComponent<CanvasGroup>();

        endingCustomerHolder = GameObject.Find("EndofGameCustomersHolder").transform;

        gameEndTime = Time.time + gameLengthSeconds;
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (gameState != GameState.ENDED && gameEndTime <= Time.time)
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
        // TODO: Replace with call to shop manager to receive customers received.
        List<CustomerData> customers = new List<CustomerData>();
        CustomerData mockCustomerData1 = new CustomerData();
        mockCustomerData1.customerImageName = shopManager.Sprites[0].name;
        customers.Add(mockCustomerData1);
        CustomerData mockCustomerData2 = new CustomerData();
        mockCustomerData2.customerImageName = shopManager.Sprites[1].name;
        customers.Add(mockCustomerData2);

        foreach (CustomerData customerData in customers)
        {
            GameObject endingCustomerViewer = Instantiate(endingCustomerViewerPrefab, Vector3.zero, Quaternion.identity, endingCustomerHolder);
            endingCustomerViewer.GetComponent<Image>().sprite = shopManager.GetCustomerSprite(customerData.customerImageName);
        }
    }

}
