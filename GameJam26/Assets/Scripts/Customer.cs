using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public Sprite ProgressFill;
    public CustomerData Data { get; set; }

    float _totalTime;
    float _timeRemaining;
    Image _timerFill;

    void Update()
    {
        if (_timerFill == null) return;

        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining < 0) _timeRemaining = 0;
        _timerFill.fillAmount = _timeRemaining / _totalTime;
    }

    public void StartTimer(float duration)
    {
        _totalTime = duration;
        _timeRemaining = duration;

        var timerObj = new GameObject("TimerFill");
        timerObj.transform.SetParent(transform, false);

        var rect = timerObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(60, 60);
        rect.anchoredPosition = new Vector2(0, 50);

        _timerFill = timerObj.AddComponent<Image>();
        _timerFill.sprite = ProgressFill;
        _timerFill.color = new Color(1f, 1f, 1f, 0.5f);
        _timerFill.type = Image.Type.Filled;
        _timerFill.fillMethod = Image.FillMethod.Radial360;
        _timerFill.fillOrigin = (int)Image.Origin360.Top;
        _timerFill.fillClockwise = false;
        _timerFill.fillAmount = 1f;
    }
}
