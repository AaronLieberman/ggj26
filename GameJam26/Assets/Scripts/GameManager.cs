using System.Collections;
using UnityEngine;

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

    private void Awake()
    {
        shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
        conveyorManager = Utilities.GetRootComponentRecursive<ConveyorManager>();

        startingGameCanvasGroup = GameObject.Find("StartingGameUI").GetComponent<CanvasGroup>();
        endingGameCanvasGroup = GameObject.Find("EndingGameUI").GetComponent<CanvasGroup>();

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

        shopManager.ActivateManager();
        conveyorManager.ActivateManager();

        gameState = GameState.STARTED;
    }

    private void EndGame()
    {
        Debug.Log("Ending game");

        StartCoroutine(FadeUI(endingGameCanvasGroup, 0.0f, 1.0f, endGameTransitionDuration));

        shopManager.DeactivateManager();
        conveyorManager.DeactivateManager();

        gameState = GameState.ENDED;
    }

    private IEnumerator FadeUI(CanvasGroup canvasGroup, float startingAlpha, float endingAlpha, float transitionDurationSeconds)
    {
        float timeElapsed = 0f;

        while (timeElapsed < transitionDurationSeconds)
        {
            float t = timeElapsed / transitionDurationSeconds;

            canvasGroup.alpha = Mathf.Lerp(startingAlpha, endingAlpha, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
