using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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
    public List<GameObject> _mountHints = new List<GameObject>();

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


        UnityEngine.Debug.Log($"MaskPiece type:{_type} {name}");
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _physics = GetComponent<Rigidbody2D>();

        RefreshMountPoints();
    }

    public void Start()
    {
        if (Data == null)
        {
            UnityEngine.Debug.LogError("No data on MaskPiece!");
            GameObject.Destroy(this.gameObject);
            return;
        }

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
        _mountPoints = FindObjectsByType<MountPoint>(FindObjectsSortMode.None)
            .Where(p => p.Type == _type
                //&& Data.isLeft == p.IsLeft
                && p.transform.parent.GetComponentsInChildren<MaskPiece>(false).Length == 0)
            .ToArray();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _physics.linearVelocity = Vector2.zero;
        _physics.angularVelocity = 0f;

        RefreshMountPoints();
        this.transform.SetParent(OriginalParent, true);
        _dragPos = _rectTransform.anchoredPosition;
        this.transform.SetAsLastSibling();

        DetachMountHints();
        AttachMountHints();

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void AttachMountHints()
    {
        foreach (var mount in _mountPoints)
        {
            GameObject hint = Instantiate(this.gameObject, mount.transform.parent);
            hint.transform.localPosition = Vector3.zero;
            hint.transform.localRotation = Quaternion.identity;
            hint.transform.localScale = Vector3.one;
            hint.name = this.name + "_hint";

            var comp = hint.gameObject.GetComponent<MaskPiece>();
            if (comp != null)
            {
                Destroy(comp);
            }

            var imgs = new List<Image>(hint.GetComponentsInChildren<Image>());
            var rootImg = hint.GetComponent<Image>();
            if (rootImg != null && !imgs.Contains(rootImg))
                imgs.Insert(0, rootImg);

            foreach (var img in imgs)
            {
                var pulse = img.AddComponent<UIPulse>();
                pulse.pulseColor = new Color(255, 0, 255);
                pulse.baseIntensity = 0.3f;
                pulse.intensity = 0.3f;
                pulse.speed = 0.5f;
            }
            hint.transform.SetParent(mount.transform, false);     
            _mountHints.Add(hint);             
        }
    }

    public void DetachMountHints()
    {
        foreach (var hint in _mountHints)
        {
            GameObject.Destroy(hint);
        }

        _mountHints.Clear();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MountPoint closest = FindClosest();

        _dragPos += (eventData.delta / _canvas.scaleFactor);
        if (closest == null)
        {
            this.transform.SetParent(OriginalParent, false);
            _rectTransform.anchoredPosition = _dragPos;
            this.transform.SetAsLastSibling();
        }
    }

    private MountPoint FindClosest()
    {
        float closestDist = 0f;
        MountPoint closest = null;

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

        //if (closest.transform.parent.name.Contains("Left"))
        //{
        //    this.transform.localScale = new Vector3(-1, 1, 1);
        //}

        return closest;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
        }

        DetachMountHints();

        //_physics.linearVelocity = _nonDragVelocity;
    }

    public static MaskPartData[] GetActiveMaskPartData(Mask mask)
    {
        var maskPieces = mask.GetComponentsInChildren<MaskPiece>();
        var maskPartData = maskPieces
            .Where(piece => piece.Data != null) // filter out the fake ones like the ones at the anchor points
            .Select(piece => piece.Data);
        return maskPartData.ToArray();
    }
    public static MaskPiece[] GetActiveMaskParts(Mask mask)
    {
        return mask.GetComponentsInChildren<MaskPiece>();
    }
}
