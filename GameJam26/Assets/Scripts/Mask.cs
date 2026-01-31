using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MaskPieceAnchor
{
    public enum State
    {
        Empty,
        Mounted
    }

    public State CurrentState = State.Empty;
    public MaskPiece MountedPiece;
    public Transform MountedPoint;

    public MaskPieceType Type;
    public List<RectTransform> MountPoints; 
}

public class Mask : MonoBehaviour
{   
    [SerializeField] RectTransform MountPoints;
    [SerializeField] RectTransform MountObjContainer;

    [SerializeField] public float SnapDistance = 1f;


    public Dictionary<MaskPieceType, MaskPieceAnchor> MaskPieceAnchors
        = new Dictionary<MaskPieceType, MaskPieceAnchor>();

    void Awake()
    {
        ResyncMounts();
    }


    public struct CheckSnapResult
    {
        public bool ShouldSnap;
        public float Distance;
        public MaskPieceType SnappedType;
        public RectTransform SnappedPoint;
    }

    public CheckSnapResult CheckSnap(Vector2 fromPosition)
    {
        CheckSnapResult? closest = null;

        foreach (var pieceTypeMount in MaskPieceAnchors.Values
                                            .Where(anchor => anchor.CurrentState != MaskPieceAnchor.State.Mounted))
        {
            foreach (var mountPoint in pieceTypeMount.MountPoints)
            {
                var dist = Vector2.Distance(
                            fromPosition,
                            mountPoint.position);
                if ((dist < (closest?.Distance ?? 0))
                    || dist < SnapDistance)
                {
                    Debug.Log($"SNAPPED: {pieceTypeMount.Type} :: {fromPosition} :: {mountPoint} :: {dist}");
                    closest = new CheckSnapResult {
                        ShouldSnap = true,
                        Distance = dist,
                        SnappedPoint = mountPoint,
                        SnappedType = pieceTypeMount.Type };
                }
            }
        }        

        if (closest != null)
        {
            return closest.Value;
        }

        return new CheckSnapResult { ShouldSnap = false };
    }

    public void MountPiece(MaskPiece piece, MaskPieceType type, Transform mountPoint)
    {
        Debug.Log($"MOUNT: {piece} {type}");
        var anchor = MaskPieceAnchors[type];
        if (anchor.MountedPiece != null)
        {
            GameObject.Destroy(anchor.MountedPiece.gameObject);
        }

        anchor.CurrentState = MaskPieceAnchor.State.Mounted;   
        piece.transform.SetParent(MountObjContainer);
        piece.transform.position = mountPoint.position;
        anchor.MountedPiece = piece;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Resync mounts")]
    public void ResyncMounts()
    {
        MaskPieceAnchors.Clear();

        foreach (MaskPieceType type in System.Enum.GetValues(typeof(MaskPieceType)))
        {
            string suffix = type.ToString();
            var containersToTry = new string[] {suffix, suffix + "Left", suffix + "Right"};
            var transforms = new List<RectTransform>();

            foreach (var tryContainer in containersToTry) 
            {
                Debug.Log($"{tryContainer}");
                var container = MountPoints.Find(tryContainer);

                if (container != null)
                {
                    for (int i = 0; i < container.childCount; i++)
                    {
                        var t = container.GetChild(i);
                        transforms.Add(t.GetComponent<RectTransform>());
                    }
                }
            }

            if (transforms.Count == 0)
            {
                continue;
            }

            MaskPieceAnchors[type] = new MaskPieceAnchor { Type = type, MountedPiece = null, MountedPoint = null, MountPoints = transforms };
        }
    }
}


