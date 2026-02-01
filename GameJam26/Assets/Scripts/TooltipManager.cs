
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Audio.ProcessorInstance;

[RequireComponent(typeof(CanvasGroup))]
class TooltipManager : MonoBehaviour 
{
    public static TooltipManager Instance;

    [SerializeField] public PieceTooltip PieceStatsTooltip;
    [SerializeField] public float TooltipYOffset = 140;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private RectTransform _canvasRect;
    private Camera _canvasCamera;
    private Vector2 _lastMousePos = Vector2.zero;
    private MaskPiece _showingOnPiece;
    private void Awake()
    {
        Instance = this;
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvas = _rectTransform.GetComponentInParent<Canvas>();
        if (_canvas != null)
        {
            _canvasRect = _canvas.GetComponent<RectTransform>();
            _canvasCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        }

        HideAll();
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        Vector2 mousePos = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        bool mouseDown = Mouse.current != null ? Mouse.current.leftButton.isPressed : false;
#else
        Vector2 mousePos = Input.mousePosition;
        bool mouseDown = Input.GetMouseButtonDown(0);
#endif
        _rectTransform.position = mousePos;

        if (_showingOnPiece != null && PieceStatsTooltip != null)
        {
            PieceStatsTooltip.transform.position = mousePos + new Vector2(0, TooltipYOffset);
        }

        if (_lastMousePos == mousePos
            || mouseDown)
        {
            if (mouseDown)
            {
                HideAll();
            }
            return;
        }

        _lastMousePos = mousePos;

        var conveyorManager = Utilities.GetRootComponentRecursive<ConveyorManager>();
        var pieces = conveyorManager.GetComponentsInChildren<MaskPiece>();

        foreach (var piece in pieces)
        {
            var rt = piece.GetComponent<RectTransform>();
            var canvas = piece.GetComponentInParent<Canvas>();
            var cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;

            if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, cam))
            {
                ShowTooltipOnPiece(piece, mousePos);
                return;
            }
        }

        HideAll();
    }

    public void ShowTooltipOnPiece(MaskPiece piece, Vector2 mousePos)
    {
        if (_showingOnPiece == piece)
        {
            return;
        }

        _showingOnPiece = piece;
        _canvasGroup.alpha = 1;

        if (PieceStatsTooltip == null)
        {
            return;
        }

        UnityEngine.Debug.Log($"Showing tooltip for piece: {piece.name}");
        PieceStatsTooltip.transform.position = mousePos + new Vector2(0, TooltipYOffset);
        PieceStatsTooltip.Data = piece.Data;
    }

    public void HideIfShowingPiece(MaskPiece piece)
    {
        if (PieceStatsTooltip.Data == piece.Data)
        {
            HideAll();
        }
    }

    public void HideAll()
    {
        _canvasGroup.alpha = 0;
        _showingOnPiece = null;

    }

    static Vector2 ClampTooltipLocalPoint(RectTransform tooltipRect, RectTransform canvasRect, Vector2 localPoint, float padding)
    {
        Vector2 size = tooltipRect.rect.size;
        Vector2 pivot = tooltipRect.pivot;

        float minX = canvasRect.rect.xMin + padding + size.x * pivot.x;
        float maxX = canvasRect.rect.xMax - padding - size.x * (1f - pivot.x);
        float minY = canvasRect.rect.yMin + padding + size.y * pivot.y;
        float maxY = canvasRect.rect.yMax - padding - size.y * (1f - pivot.y);

        localPoint.x = Mathf.Clamp(localPoint.x, minX, maxX);
        localPoint.y = Mathf.Clamp(localPoint.y, minY, maxY);
        return localPoint;
    }    
}