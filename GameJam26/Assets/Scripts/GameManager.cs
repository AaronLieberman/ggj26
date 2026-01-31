using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { STARTED, ENDED }

public class GameManager : MonoBehaviour
{
    [SerializeField] private int gameLengthSeconds;
    private float gameEndTime;
    private GameState gameState;

    [SerializeField] private float foregroundTransitionDuration;
    private Image foregroundImage;

    private void Awake()
    {
        foregroundImage = GameObject.Find("ForegroundImage").GetComponent<Image>();

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
        StartCoroutine(FadeInForeground());
        gameState = GameState.STARTED;
    }

    private void EndGame()
    {
        gameState = GameState.ENDED;
    }

    private IEnumerator FadeInForeground()
    {
        float timeElapsed = 0f;

        while (timeElapsed < foregroundTransitionDuration)
        {
            float t = timeElapsed / foregroundTransitionDuration;

            foregroundImage.color = new Color(foregroundImage.color.r, foregroundImage.color.g, foregroundImage.color.b, Mathf.Lerp(1.0f, 0.0f, t));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
