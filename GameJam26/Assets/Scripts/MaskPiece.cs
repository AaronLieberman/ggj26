using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class MaskPiece : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    Vector3 maskPieceScale;
    [SerializeField]
    Vector3 maskPieceDisplayScale;

    public MaskPartData Data { get; set; }

    MaskPartSlot _type;
    Transform _originalParent;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Rigidbody2D _physics;
    private CanvasGroup _canvasGroup;
    BoxCollider2D _collider;
    Vector2 _dragPos;
    Vector2 _nonDragVelocity;

    public MaskPartSlot Type { get { return _type; } }
    public Transform OriginalParent { get { return _originalParent; } }
    public MountPoint[] _mountPoints;
    public List<GameObject> _mountHints = new List<GameObject>();

    private void Awake()
    {
        _originalParent = this.transform.parent;


        UnityEngine.Debug.Log($"MaskPiece type:{_type} {name}");
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _physics = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
    }

    public void ApplySpriteSize(Sprite sprite)
    {
        float w = sprite.rect.width;
        float h = sprite.rect.height;
        _rectTransform.sizeDelta = new Vector2(w, h);

        if (_collider != null)
        {
            _collider.offset = Vector2.zero;
            _collider.size = new Vector2(w, h);
        }
    }

    public void Start()
    {
        if (Data == null)
        {
            //UnityEngine.Debug.LogError("No data on MaskPiece!");
            //GameObject.Destroy(this.gameObject);
            return;
        }
        _type = Data.slot;
    
        _nonDragVelocity = _physics.linearVelocity;

        RefreshMountPoints();
    }

    public void RefreshMountPoints()
    {
        if (Data.NotFlipped) //I'm sorry. The NotFlipped variable is confusing. Claude is my scapegoat.
        {
            _mountPoints = FindObjectsByType<MountPoint>(FindObjectsSortMode.None)
                .Where(mountPoint => mountPoint.Type == _type
                    //&& mountPoint.transform.parent.GetComponentsInChildren<MaskPiece>(false).Length == 0
                    && (mountPoint.IsLeft || mountPoint.IsCenter ) 
                    )
                .ToArray();
        }
        else
        {
            _mountPoints = FindObjectsByType<MountPoint>(FindObjectsSortMode.None)
                .Where(mountPoint => mountPoint.Type == _type
                     //&& mountPoint.transform.parent.GetComponentsInChildren<MaskPiece>(false).Length == 0
                     && mountPoint.IsRight
                    )
                .ToArray();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<Rigidbody2D>().simulated = false;

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
    public void OnPointerUp(PointerEventData eventData)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
        }

        DetachMountHints();

        if (this._rectTransform.parent == OriginalParent)
        {
            GetComponent<Rigidbody2D>().simulated = true;
        }

        //_physics.linearVelocity = _nonDragVelocity;
    }

    public void AttachMountHints()
    {
        foreach (var mount in _mountPoints)
        {
            // Attach non-placeable hint
            if (mount.transform.parent.GetComponentsInChildren<MaskPiece>(false).Length != 0)
            {
                GameObject nonPlaceableHint = Instantiate(this.gameObject, mount.transform.parent);
                nonPlaceableHint.transform.localPosition = Vector3.zero;
                nonPlaceableHint.transform.localRotation = Quaternion.identity;
                if (Data.NotFlipped)
                {
                    nonPlaceableHint.transform.localScale = maskPieceDisplayScale;
                }
                else
                {
                    nonPlaceableHint.transform.localScale = new Vector3(-maskPieceDisplayScale.x, maskPieceDisplayScale.y, maskPieceDisplayScale.z);
                }
                nonPlaceableHint.name = this.name + "_nonPlaceableHint";

                var comp1 = nonPlaceableHint.gameObject.GetComponent<MaskPiece>();
                if (comp1 != null)
                {
                    Destroy(comp1);
                }

                var imgs1 = new List<Image>(nonPlaceableHint.GetComponentsInChildren<Image>());
                var rootImg1 = nonPlaceableHint.GetComponent<Image>();
                if (rootImg1 != null && !imgs1.Contains(rootImg1))
                    imgs1.Insert(0, rootImg1);

                foreach (var img in imgs1)
                {
                    var pulse = img.AddComponent<UIPulse>();
                    pulse.pulseColor = new UnityEngine.Color(255, 0, 0);
                    pulse.baseIntensity = 0.3f;
                    pulse.intensity = 0.3f;
                    pulse.speed = 1.0f;
                }
                nonPlaceableHint.transform.SetParent(mount.transform, false);
                _mountHints.Add(nonPlaceableHint);

                continue;
            }

            // Attach placeable hint
            GameObject hint = Instantiate(this.gameObject, mount.transform.parent);
            hint.transform.localPosition = Vector3.zero;
            hint.transform.localRotation = Quaternion.identity;
            if (Data.NotFlipped)
            {
                hint.transform.localScale = maskPieceDisplayScale;
            }
            else
            {
                hint.transform.localScale = new Vector3(-maskPieceDisplayScale.x, maskPieceDisplayScale.y, maskPieceDisplayScale.z);
            }
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
                pulse.pulseColor = new UnityEngine.Color(255, 0, 255);
                pulse.baseIntensity = 0.3f;
                pulse.intensity = 0.3f;
                pulse.speed = 1.0f;
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

        _dragPos += eventData.delta / _canvas.scaleFactor;
        if (closest == null)
        {
            transform.SetParent(OriginalParent, false);
            _rectTransform.anchoredPosition = _dragPos;
            transform.SetAsLastSibling();

            if (Data.NotFlipped)
            {
                transform.localScale = maskPieceScale;
            }
            else
            {
                transform.localScale = new Vector3(-maskPieceScale.x, maskPieceScale.y, maskPieceScale.z);
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {

    }

    private MountPoint FindClosest()
    {
        float closestDist = 0f;
        MountPoint closest = null;

        var fromPosition = Mouse.current.position.ReadValue();
        foreach (var mountPoint in _mountPoints)
        {
            UIPulse[] nonPlaceableHints = mountPoint.transform.parent.GetComponentsInChildren<UIPulse>().Where(hint => hint.name.Contains("_nonPlaceableHint")).ToArray();
            if (nonPlaceableHints.Length != 0)
            {
                continue;
            }

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
        if (Data.NotFlipped)
        {
            transform.localScale = maskPieceDisplayScale;
        }
        else
        {
            transform.localScale = new Vector3(-maskPieceDisplayScale.x, maskPieceDisplayScale.y, maskPieceDisplayScale.z);
        }

        return closest;
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
