using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaskPiece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public MaskPartData Data { get; set; }

    private RectTransform _rectTransform;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    struct CurrentSnap
    {
        public Mask Mask;
        public MaskPieceType SnapType;
        public RectTransform SnapPoint;
    }

    CurrentSnap? _currentSnap = null;

    private Vector2 _cursorLocation;
    private Mask[] _masks;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _masks = Object.FindObjectsByType<Mask>(FindObjectsSortMode.None);

        _cursorLocation = _rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        _cursorLocation = _rectTransform.anchoredPosition;

        UpdateMaskSnap();
    }

    private void UpdateMaskSnap()
    {
        foreach (var mask in _masks)
        {
            var result = mask.CheckSnap(_rectTransform.position);
            if (result.DidSnap)
            {
                Debug.Log($"Snap match: {result.SnappedType} {result.SnappedPoint}");
                _currentSnap = new CurrentSnap() { Mask = mask, SnapPoint = result.SnappedPoint, SnapType = result.SnappedType };
                return;
            }
        }

        _currentSnap = null;
        _rectTransform.anchoredPosition = _cursorLocation;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
            if (_currentSnap.HasValue)
            {
                _currentSnap.Value.Mask.MountPiece(this, _currentSnap.Value.SnapType, _currentSnap.Value.SnapPoint);
            }
        }
    }
}
