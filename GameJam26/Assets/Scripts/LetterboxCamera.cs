using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LetterboxCamera : MonoBehaviour
{
    const float TargetAspect = 16f / 9f;

    RectTransform _canvasRect;
    CanvasScaler _scaler;
    int _lastWidth;
    int _lastHeight;
    int _lastChildCount;

    readonly Dictionary<RectTransform, (Vector2 min, Vector2 max)> _originalAnchors = new();
    readonly HashSet<RectTransform> _barPanels = new();
    RectTransform _barLeft, _barRight, _barTop, _barBottom;

    void Start()
    {
        var cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
        }

        var canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        _canvasRect = canvas.GetComponent<RectTransform>();
        _scaler = canvas.GetComponent<CanvasScaler>();

        CreateBarPanels();
        ScanChildren();
        UpdateAnchors();
    }

    void CreateBarPanels()
    {
        _barLeft = CreateBar("LetterboxLeft");
        _barRight = CreateBar("LetterboxRight");
        _barTop = CreateBar("LetterboxTop");
        _barBottom = CreateBar("LetterboxBottom");
    }

    RectTransform CreateBar(string name)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        go.transform.SetParent(_canvasRect, false);

        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = Vector2.zero;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var img = go.GetComponent<Image>();
        img.color = Color.black;
        img.raycastTarget = false;

        var cg = go.GetComponent<CanvasGroup>();
        cg.blocksRaycasts = true;

        _barPanels.Add(rt);
        return rt;
    }

    void ScanChildren()
    {
        var toRemove = _originalAnchors
            .Where(kvp => kvp.Key == null || kvp.Key.parent != _canvasRect)
            .ToList();

        foreach (var (rt, orig) in toRemove)
        {
            if (rt != null)
            {
                rt.anchorMin = orig.min;
                rt.anchorMax = orig.max;
            }
            _originalAnchors.Remove(rt);
        }

        for (int i = 0; i < _canvasRect.childCount; i++)
        {
            var child = _canvasRect.GetChild(i) as RectTransform;
            if (child != null && !_originalAnchors.ContainsKey(child) && !_barPanels.Contains(child))
                _originalAnchors[child] = (child.anchorMin, child.anchorMax);
        }

        _lastChildCount = _canvasRect.childCount;
    }

    void Update()
    {
        if (_canvasRect == null) return;

        bool sizeChanged = Screen.width != _lastWidth || Screen.height != _lastHeight;
        bool childCountChanged = _canvasRect.childCount != _lastChildCount;

        if (childCountChanged)
            ScanChildren();

        if (sizeChanged || childCountChanged)
            UpdateAnchors();
    }

    void UpdateAnchors()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;

        if (_canvasRect == null) return;

        Vector2 canvasSize;
        if (_scaler != null && _scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            float screenAspect = (float)Screen.width / Screen.height;
            float match = screenAspect > TargetAspect ? 1f : 0f;
            _scaler.matchWidthOrHeight = match;

            Vector2 refRes = _scaler.referenceResolution;
            float logWidth = Mathf.Log(Screen.width / refRes.x, 2);
            float logHeight = Mathf.Log(Screen.height / refRes.y, 2);
            float scaleFactor = Mathf.Pow(2f, Mathf.Lerp(logWidth, logHeight, match));
            canvasSize = new Vector2(Screen.width / scaleFactor, Screen.height / scaleFactor);
        }
        else
        {
            canvasSize = _canvasRect.rect.size;
        }

        if (canvasSize.x <= 0 || canvasSize.y <= 0) return;

        float canvasAspect = canvasSize.x / canvasSize.y;

        float normLeft, normRight, normBottom, normTop;

        if (canvasAspect > TargetAspect)
        {
            float safeWidth = canvasSize.y * TargetAspect;
            normLeft = (canvasSize.x - safeWidth) / (2f * canvasSize.x);
            normRight = 1f - normLeft;
            normBottom = 0f;
            normTop = 1f;
        }
        else if (canvasAspect < TargetAspect)
        {
            float safeHeight = canvasSize.x / TargetAspect;
            normBottom = (canvasSize.y - safeHeight) / (2f * canvasSize.y);
            normTop = 1f - normBottom;
            normLeft = 0f;
            normRight = 1f;
        }
        else
        {
            normLeft = 0f;
            normRight = 1f;
            normBottom = 0f;
            normTop = 1f;
        }

        foreach (var (rt, orig) in _originalAnchors)
        {
            if (rt == null) continue;

            rt.anchorMin = new Vector2(
                Mathf.Lerp(normLeft, normRight, orig.min.x),
                Mathf.Lerp(normBottom, normTop, orig.min.y)
            );
            rt.anchorMax = new Vector2(
                Mathf.Lerp(normLeft, normRight, orig.max.x),
                Mathf.Lerp(normBottom, normTop, orig.max.y)
            );
        }

        SetBarAnchors(_barLeft, 0f, 0f, normLeft, 1f);
        SetBarAnchors(_barRight, normRight, 0f, 1f, 1f);
        SetBarAnchors(_barBottom, 0f, 0f, 1f, normBottom);
        SetBarAnchors(_barTop, 0f, normTop, 1f, 1f);

        _barLeft.SetAsLastSibling();
        _barRight.SetAsLastSibling();
        _barTop.SetAsLastSibling();
        _barBottom.SetAsLastSibling();
    }

    void SetBarAnchors(RectTransform bar, float minX, float minY, float maxX, float maxY)
    {
        if (bar == null) return;
        bar.anchorMin = new Vector2(minX, minY);
        bar.anchorMax = new Vector2(maxX, maxY);
        bar.offsetMin = Vector2.zero;
        bar.offsetMax = Vector2.zero;
    }
}
