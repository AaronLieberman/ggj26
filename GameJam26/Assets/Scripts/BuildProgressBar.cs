using UnityEngine;

public class BuildProgressBar : MonoBehaviour
{
    public GameObject Meter;
    public GameObject Range;
    public GameObject ArrowMin;
    public GameObject ArrowMax;

    const float ArrowWidth = 0.1f;

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
        _meterRect.anchorMax = new Vector2(t, _meterRect.anchorMax.y);
    }

    public void SetArrowMin(float t)
    {
        t = Mathf.Clamp01(t / 10);
        _arrowMinValue = t;
        float x = t * (1f - ArrowWidth);
        _arrowMinRect.anchorMin = new Vector2(x, _arrowMinRect.anchorMin.y);
        _arrowMinRect.anchorMax = new Vector2(x + ArrowWidth, _arrowMinRect.anchorMax.y);
        UpdateRange();
    }

    public void SetArrowMax(float t)
    {
        t = Mathf.Clamp01(t / 10);
        _arrowMaxValue = t;
        float x = t * (1f - ArrowWidth);
        _arrowMaxRect.anchorMin = new Vector2(x, _arrowMaxRect.anchorMin.y);
        _arrowMaxRect.anchorMax = new Vector2(x + ArrowWidth, _arrowMaxRect.anchorMax.y);
        UpdateRange();
    }

    void UpdateRange()
    {
        float minX = _arrowMinValue * (1f - ArrowWidth) + ArrowWidth * 0.5f;
        float maxX = _arrowMaxValue * (1f - ArrowWidth) + ArrowWidth * 0.5f;
        _rangeRect.anchorMin = new Vector2(minX, _rangeRect.anchorMin.y);
        _rangeRect.anchorMax = new Vector2(maxX, _rangeRect.anchorMax.y);
    }
}
