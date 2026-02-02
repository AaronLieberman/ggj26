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
    public float AnimationSpeed = 8f;

    const float ArrowWidth = 0.1f;
    const float MeterWidth = 0.1f;

    RectTransform _arrowMinRect;
    RectTransform _arrowMaxRect;
    RectTransform _meterRect;
    RectTransform _rangeRect;
    List<Material> _glowMaterials = new List<Material>();

    float _meterCurrent;
    float _meterTarget;
    float _arrowMinCurrent;
    float _arrowMinTarget;
    float _arrowMaxCurrent;
    float _arrowMaxTarget;
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
        _meterTarget = Mathf.Clamp01(t / 10);
    }

    public void SetArrowMin(float t)
    {
        t = Mathf.Clamp01(t / 10);
        _arrowMinTarget = t;
        ArrowMin.SetActive(t >= 0.05);
    }

    public void SetArrowMax(float t)
    {
        t = Mathf.Clamp01(t / 10);
        _arrowMaxTarget = t;
        ArrowMax.SetActive(t <= 0.95);
    }

    public void SetInRange(bool value)
    {
        _inRange = value;
    }

    void Update()
    {
        float dt = Time.deltaTime * AnimationSpeed;
        _meterCurrent = Mathf.Lerp(_meterCurrent, _meterTarget, dt);
        _arrowMinCurrent = Mathf.Lerp(_arrowMinCurrent, _arrowMinTarget, dt);
        _arrowMaxCurrent = Mathf.Lerp(_arrowMaxCurrent, _arrowMaxTarget, dt);

        float mx = _meterCurrent * (1f - MeterWidth);
        _meterRect.anchorMin = new Vector2(mx - MeterWidth / 2, _meterRect.anchorMin.y);
        _meterRect.anchorMax = new Vector2(mx + MeterWidth / 2, _meterRect.anchorMax.y);

        _arrowMinRect.anchorMin = new Vector2(_arrowMinCurrent - ArrowWidth / 2, _arrowMinRect.anchorMin.y);
        _arrowMinRect.anchorMax = new Vector2(_arrowMinCurrent + ArrowWidth / 2, _arrowMinRect.anchorMax.y);

        _arrowMaxRect.anchorMin = new Vector2(_arrowMaxCurrent - ArrowWidth / 2, _arrowMaxRect.anchorMin.y);
        _arrowMaxRect.anchorMax = new Vector2(_arrowMaxCurrent + ArrowWidth / 2, _arrowMaxRect.anchorMax.y);

        _rangeRect.anchorMin = new Vector2(_arrowMinCurrent, _rangeRect.anchorMin.y);
        _rangeRect.anchorMax = new Vector2(_arrowMaxCurrent, _rangeRect.anchorMax.y);
    }

    void LateUpdate()
    {
        foreach (var material in _glowMaterials)
        {
            material.SetFloat(GlowIntensityId, _inRange ? GlowOnIntensity : 0f);
        }
    }
}
