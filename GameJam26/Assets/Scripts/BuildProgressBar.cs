using UnityEngine;

public class BuildProgressBar : MonoBehaviour
{
    public GameObject Meter;
    public GameObject Range;
    public GameObject ArrowMin;
    public GameObject ArrowMax;

    const float ArrowWidth = 0.1f;
    const float MeterWidth = 0.1f;

    RectTransform _arrowMinRect;
    RectTransform _arrowMaxRect;
    RectTransform _meterRect;
    RectTransform _rangeRect;

    float _arrowMinValue;
    float _arrowMaxValue;

    void Awake()
    {
        _arrowMinRect = ArrowMin.GetComponent<RectTransform>();
        _arrowMaxRect = ArrowMax.GetComponent<RectTransform>();
        _meterRect = Meter.GetComponent<RectTransform>();
        _rangeRect = Range.GetComponent<RectTransform>();
    }

    public void SetMeter(float t)
    {
        t = Mathf.Clamp01(t / 10);
        float x = t * (1f - MeterWidth);
        _meterRect.anchorMin = new Vector2(x - (t >= 0.05 ? MeterWidth / 2 : 0), _meterRect.anchorMin.y);
        _meterRect.anchorMax = new Vector2(x + (t <= 0.95 ? MeterWidth / 2 : 0), _meterRect.anchorMax.y);
    }

    public void SetArrowMin(float t)
    {
        t = Mathf.Clamp01(t / 10);
        _arrowMinValue = t;
        _arrowMinRect.anchorMin = new Vector2(t - ArrowWidth / 2, _arrowMinRect.anchorMin.y);
        _arrowMinRect.anchorMax = new Vector2(t + ArrowWidth / 2, _arrowMinRect.anchorMax.y);
        ArrowMin.SetActive(t >= 0.05);
        UpdateRange();
    }

    public void SetArrowMax(float t)
    {
        t = Mathf.Clamp01(t / 10);
        _arrowMaxValue = t;
        _arrowMaxRect.anchorMin = new Vector2(t - ArrowWidth / 2, _arrowMaxRect.anchorMin.y);
        _arrowMaxRect.anchorMax = new Vector2(t + ArrowWidth / 2, _arrowMaxRect.anchorMax.y);
        ArrowMax.SetActive(t <= 0.95);
        UpdateRange();
    }

    void UpdateRange()
    {
        float minX = _arrowMinValue;
        float maxX = _arrowMaxValue;
        _rangeRect.anchorMin = new Vector2(minX, _rangeRect.anchorMin.y);
        _rangeRect.anchorMax = new Vector2(maxX, _rangeRect.anchorMax.y);
    }
}
