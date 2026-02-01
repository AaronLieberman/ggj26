using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MaskPiece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public MaskPartData Data { get; set; }

    MaskPieceType _type;
    Transform _originalParent;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    public MaskPieceType Type { get { return _type; } }
    public Transform OriginalParent { get { return _originalParent; } }
    public MountPoint[] _mountPoints;

    private void Awake()
    {
        _originalParent = this.transform.parent;

        // TODO is this the best way to find name?
        foreach (MaskPieceType type in System.Enum.GetValues(typeof(MaskPieceType)))
        {
            if (this.gameObject.name.StartsWith(type.ToString()))
            {
                _type = type;
                break;
            }
        }

        Debug.Log($"MaskPiece type:{_type} {name}");
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();

        RefreshMountPoints();
    }

    public void RefreshMountPoints()
    {
        _mountPoints = Object.FindObjectsByType<MountPoint>(FindObjectsSortMode.None)
                            .Where(p => p.Type == _type
                                    && p.transform.parent.GetComponentsInChildren<MaskPiece>(false).Length == 0)
                            .ToArray();
    }

    Vector2 _dragPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        RefreshMountPoints();
        this.transform.SetParent(OriginalParent, true);
        _dragPos = _rectTransform.anchoredPosition;
        this.transform.SetAsLastSibling();

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        MountPoint? closest = FindClosest();

        _dragPos += (eventData.delta / _canvas.scaleFactor);
        if (closest == null)
        {
            this.transform.SetParent(OriginalParent, false);
            _rectTransform.anchoredPosition = _dragPos;
            this.transform.SetAsLastSibling();
        }
    }

    private MountPoint? FindClosest()
    {
        float closestDist = 0f;
        MountPoint? closest = null;

        var fromPosition = Mouse.current.position.ReadValue();
        foreach (var mountPoint in _mountPoints)
        {
            var dist = Vector2.Distance(
                        fromPosition,
                        mountPoint.transform.position);

            if ((closest == null
                    && dist < 100f)
                || dist < closestDist)
            {
                closest = mountPoint;
                closestDist = dist;
            }
        }

        if (closest == null)
        {
            return null;
        }

        this.transform.SetParent(closest.transform, false);
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;

        if (closest.transform.parent.name.Contains("Left"))
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }

        return closest;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
        }
    }
}
