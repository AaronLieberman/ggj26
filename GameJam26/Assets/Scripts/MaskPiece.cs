using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class MaskPiece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public MaskPartData Data { get; set; }

    MaskPieceType _type;
    Transform _originalParent;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Rigidbody2D _physics;
    private CanvasGroup _canvasGroup;
    Vector2 _dragPos;
    Vector2 _nonDragVelocity;

    public MaskPieceType Type { get { return _type; } }
    public Transform OriginalParent { get { return _originalParent; } }
    public MountPoint[] _mountPoints;

    private void Awake()
    {
        _originalParent = this.transform.parent;

        // TODO is this the best way to find name?
        /*
        foreach (MaskPieceType type in System.Enum.GetValues(typeof(MaskPieceType)))
        {
            if (this.gameObject.name.StartsWith(type.ToString()))
            {
                _type = type;
                break;
            }
        }
        */


        Debug.Log($"MaskPiece type:{_type} {name}");
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _physics = GetComponent<Rigidbody2D>();

        RefreshMountPoints();
    }

    public void Start()
    {
        switch (Data.slot.ToString())
        {
            case "Nose":
                _type = MaskPieceType.Nose;
                break;
            case "Mouth":
                _type = MaskPieceType.Mouth;
                break;
            case "Eye":
                _type = MaskPieceType.Eyes;
                break;
            case "Horn":
                _type = MaskPieceType.Horns;
                break;
            default:
                _type = MaskPieceType.Base;
                break;
        }

        _nonDragVelocity = _physics.linearVelocity;
    }

    public void RefreshMountPoints()
    {
        _mountPoints = Object.FindObjectsByType<MountPoint>(FindObjectsSortMode.None)
                            .Where(p => p.Type == _type
                                    && p.transform.parent.GetComponentsInChildren<MaskPiece>(false).Length == 0)
                            .ToArray();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _physics.linearVelocity = Vector2.zero;

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

        //_physics.linearVelocity = _nonDragVelocity;
    }
}
