using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject _pauseOverlay;
    [SerializeField] Sprite _restartIcon;

    float _savedTimeScale;
    bool _isPaused;
    Button _pauseButton;
    TextMeshProUGUI _buttonText;
    GameObject _restartButtonObj;

    void Start()
    {
        var canvas = Utilities.GetRootComponentRecursive<Canvas>();
        CreatePauseButton(canvas.transform);
        CreateRestartButton(canvas.transform);
        _pauseOverlay.SetActive(false);
    }

    void CreatePauseButton(Transform parent)
    {
        var buttonObj = new GameObject("PauseButton");
        buttonObj.transform.SetParent(parent, false);

        var rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.pivot = new Vector2(0, 0);
        rect.anchoredPosition = new Vector2(10, 10);
        rect.sizeDelta = new Vector2(80, 40);

        var image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.7f);

        _pauseButton = buttonObj.AddComponent<Button>();
        _pauseButton.onClick.AddListener(TogglePause);

        var textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        _buttonText = textObj.AddComponent<TextMeshProUGUI>();
        _buttonText.text = "| |";
        _buttonText.fontSize = 20;
        _buttonText.alignment = TextAlignmentOptions.Center;
        _buttonText.color = Color.white;
    }

    void CreateRestartButton(Transform parent)
    {
        _restartButtonObj = new GameObject("RestartButton");
        _restartButtonObj.transform.SetParent(parent, false);

        var rect = _restartButtonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.pivot = new Vector2(0, 0);
        rect.anchoredPosition = new Vector2(100, 10);
        rect.sizeDelta = new Vector2(80, 40);

        var image = _restartButtonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.7f);

        var button = _restartButtonObj.AddComponent<Button>();
        button.onClick.AddListener(RestartGame);

        var iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(_restartButtonObj.transform, false);

        var iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.1f);
        iconRect.anchorMax = new Vector2(0.9f, 0.9f);
        iconRect.sizeDelta = Vector2.zero;

        var icon = iconObj.AddComponent<Image>();
        icon.sprite = _restartIcon;
        icon.preserveAspect = true;

        _restartButtonObj.SetActive(false);
    }

    public void ShowRestartButton()
    {
        _restartButtonObj.SetActive(true);
        _restartButtonObj.transform.SetAsLastSibling();
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void TogglePause()
    {
        if (_isPaused)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
    }

    void Pause()
    {
        _savedTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        _isPaused = true;
        _pauseOverlay.SetActive(true);
        _buttonText.text = "►";
        _restartButtonObj.SetActive(true);
        _pauseButton.transform.SetAsLastSibling();
        _restartButtonObj.transform.SetAsLastSibling();
    }

    void Unpause()
    {
        Time.timeScale = _savedTimeScale;
        _isPaused = false;
        _pauseOverlay.SetActive(false);
        _buttonText.text = "| |";
        _restartButtonObj.SetActive(false);
    }
}
