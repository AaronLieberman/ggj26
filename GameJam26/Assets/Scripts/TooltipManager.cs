
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Audio.ProcessorInstance;

[RequireComponent(typeof(CanvasGroup))]
class TooltipManager : MonoBehaviour 
{
    public static TooltipManager Instance;

    [SerializeField] public PieceTooltip PieceStatsTooltip;
    [SerializeField] public float TooltipXOffset = 20;
    [SerializeField] public float TooltipYOffset = 140;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private RectTransform _canvasRect;
    private RectTransform _tooltipRect;
    private Vector2 _lastMousePos = Vector2.zero;
    private MaskPiece _showingOnPiece;
    private void Awake()
    {
        Instance = this;
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvas = _rectTransform.GetComponentInParent<Canvas>();
        if (_canvas != null)
            _canvasRect = _canvas.GetComponent<RectTransform>();

        if (PieceStatsTooltip != null)
            _tooltipRect = PieceStatsTooltip.GetComponent<RectTransform>();

        HideAll();
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        Vector2 mousePos = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        Vector2 mousePos = Input.mousePosition;
#endif
        _rectTransform.position = mousePos;

        if (_showingOnPiece != null && PieceStatsTooltip != null)
        {
            PositionTooltip(mousePos);
        }

        if (_lastMousePos == mousePos)
            return;

        _lastMousePos = mousePos;

        var conveyorManager = Utilities.GetRootComponentRecursive<ConveyorManager>();
        var piecesOnConveyor = conveyorManager.GetComponentsInChildren<MaskPiece>();

        var maskDisplay = GameObject.Find("MaskDisplay");
        var piecesOnMaskDisplay = maskDisplay.GetComponentsInChildren<MaskPiece>();

        // Also find pieces being dragged (reparented to canvas root)
        var allPieces = piecesOnConveyor.Concat(piecesOnMaskDisplay);
        if (_canvasRect != null)
        {
            for (int i = 0; i < _canvasRect.childCount; i++)
            {
                var piece = _canvasRect.GetChild(i).GetComponent<MaskPiece>();
                if (piece != null)
                    allPieces = allPieces.Append(piece);
            }
        }

        foreach (var piece in allPieces)
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
        PositionTooltip(mousePos);
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

    void PositionTooltip(Vector2 mousePos)
    {
        if (_tooltipRect == null) return;

        float scale = _canvas.scaleFactor;
        float offset = TooltipXOffset * scale;
        Vector2 size = _tooltipRect.rect.size * scale;
        Vector2 pivot = _tooltipRect.pivot;
        Rect safe = GetSafeArea();

        float yOffset = TooltipYOffset * scale;

        // Place left edge of tooltip at cursor + offset
        float x = mousePos.x + offset + size.x * pivot.x;

        // If right edge exceeds safe area, flip to left of cursor
        float rightEdge = x + size.x * (1f - pivot.x);
        if (rightEdge > safe.xMax)
            x = mousePos.x - offset - size.x * (1f - pivot.x);

        float y = Mathf.Clamp(mousePos.y + yOffset,
            safe.yMin + size.y * pivot.y,
            safe.yMax - size.y * (1f - pivot.y));

        _tooltipRect.position = new Vector2(x, y);
    }

    Rect GetSafeArea()
    {
        float targetAspect = 16f / 9f;
        float screenAspect = (float)Screen.width / Screen.height;

        if (screenAspect > targetAspect)
        {
            float safeWidth = Screen.height * targetAspect;
            float left = (Screen.width - safeWidth) / 2f;
            return new Rect(left, 0, safeWidth, Screen.height);
        }

        if (screenAspect < targetAspect)
        {
            float safeHeight = Screen.width / targetAspect;
            float bottom = (Screen.height - safeHeight) / 2f;
            return new Rect(0, bottom, Screen.width, safeHeight);
        }

        return new Rect(0, 0, Screen.width, Screen.height);
    }
}