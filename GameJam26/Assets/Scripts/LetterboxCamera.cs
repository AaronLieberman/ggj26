using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterboxCamera : MonoBehaviour
{
    const float TargetAspect = 16f / 9f;

    RectTransform _canvasRect;
    int _lastWidth;
    int _lastHeight;

    readonly Dictionary<RectTransform, (Vector2 min, Vector2 max)> _originalAnchors = new();

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

        for (int i = 0; i < _canvasRect.childCount; i++)
        {
            var child = _canvasRect.GetChild(i) as RectTransform;
            if (child != null)
                _originalAnchors[child] = (child.anchorMin, child.anchorMax);
        }

        UpdateAnchors();
    }

    void Update()
    {
        if (Screen.width != _lastWidth || Screen.height != _lastHeight)
            UpdateAnchors();
    }

    void UpdateAnchors()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;

        if (_canvasRect == null) return;

        Vector2 canvasSize = _canvasRect.rect.size;
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
    }
}
