using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public Image TimerFront;

    public CustomerData Data { get; set; }

    float _totalTime;
    float _timeRemaining;

    void Start()
    {
        TimerFront.gameObject.SetActive(false);
    }

    void Update()
    {
        if (TimerFront == null) return;

        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining < 0) _timeRemaining = 0;
        TimerFront.fillAmount = _timeRemaining / _totalTime;
    }

    public void StartTimer(float duration)
    {
        _totalTime = duration;
        _timeRemaining = duration;

        TimerFront.gameObject.SetActive(true);
        TimerFront.type = Image.Type.Filled;
        TimerFront.fillMethod = Image.FillMethod.Radial360;
        TimerFront.fillOrigin = (int)Image.Origin360.Top;
        TimerFront.fillClockwise = false;
        TimerFront.fillAmount = 1f;
    }

    public void StopTimer()
    {
        TimerFront.gameObject.SetActive(false);
    }
}
