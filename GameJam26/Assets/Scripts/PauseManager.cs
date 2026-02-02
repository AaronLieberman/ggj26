using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    float _savedTimeScale;
    bool _isPaused;
    GameObject _pauseOverlay;
    Button _pauseButton;
    TextMeshProUGUI _buttonText;

    void Start()
    {
        var canvas = Utilities.GetRootComponentRecursive<Canvas>();
        CreatePauseButton(canvas.transform);
        CreatePauseOverlay(canvas.transform);
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

    void CreatePauseOverlay(Transform parent)
    {
        _pauseOverlay = new GameObject("PauseOverlay");
        _pauseOverlay.transform.SetParent(parent, false);

        var rect = _pauseOverlay.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        var image = _pauseOverlay.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.4f);
        image.raycastTarget = true;

        var textObj = new GameObject("PausedText");
        textObj.transform.SetParent(_pauseOverlay.transform, false);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(400, 100);

        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "PAUSED";
        text.fontSize = 60;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        _pauseOverlay.SetActive(false);

        // Move pause button to front so it's still clickable over the overlay
        _pauseButton.transform.SetAsLastSibling();
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
        _pauseButton.transform.SetAsLastSibling();
    }

    void Unpause()
    {
        Time.timeScale = _savedTimeScale;
        _isPaused = false;
        _pauseOverlay.SetActive(false);
        _buttonText.text = "| |";
    }
}
