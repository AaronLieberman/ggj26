using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildProgressBar : MonoBehaviour
{
    public GameObject Meter;
    public GameObject Range;
    public GameObject ArrowMin;
    public GameObject ArrowMax;
    public float GlowOnIntensity = 1.5f;

    const float ArrowWidth = 0.1f;
    const float MeterWidth = 0.1f;

    RectTransform _arrowMinRect;
    RectTransform _arrowMaxRect;
    RectTransform _meterRect;
    RectTransform _rangeRect;
    List<Material> _glowMaterials = new List<Material>();

    float _arrowMinValue;
    float _arrowMaxValue;
    bool _inRange;

    static readonly int GlowIntensityId = Shader.PropertyToID("_GlowIntensity");

    void Awake()
    {
        _arrowMinRect = ArrowMin.GetComponent<RectTransform>();
        _arrowMaxRect = ArrowMax.GetComponent<RectTransform>();
        _meterRect = Meter.GetComponent<RectTransform>();
        _rangeRect = Range.GetComponent<RectTransform>();

        var images = GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            if (image != null && image.material != null && image.material.HasFloat(GlowIntensityId))
            {
                var instance = new Material(image.material);
                image.material = instance;
                _glowMaterials.Add(instance);
            }
        }
    }

    public void SetMeter(float t)
    {
        t = Mathf.Clamp01(t / 10);
        float x = t * (1f - MeterWidth);
        _meterRect.anchorMin = new Vector2(x - MeterWidth / 2, _meterRect.anchorMin.y);
        _meterRect.anchorMax = new Vector2(x + MeterWidth / 2, _meterRect.anchorMax.y);
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

    public void SetInRange(bool value)
    {
        _inRange = value;
    }

    void UpdateRange()
    {
        float minX = _arrowMinValue;
        float maxX = _arrowMaxValue;
        _rangeRect.anchorMin = new Vector2(minX, _rangeRect.anchorMin.y);
        _rangeRect.anchorMax = new Vector2(maxX, _rangeRect.anchorMax.y);
    }

    void LateUpdate()
    {
        foreach (var material in _glowMaterials)
        {
            material.SetFloat(GlowIntensityId, _inRange ? GlowOnIntensity : 0f);
        }
    }
}
