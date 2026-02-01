using UnityEngine;

public class MountPoint : MonoBehaviour
{
    [SerializeField]
    MaskPieceType _type;

    [SerializeField]
    Handedness _handedness;


    public MaskPieceType Type { get { return _type; } }
    public bool IsLeft { get
        {
            return _handedness == Handedness.Left;
        }
    }

    public bool IsRight
    {
        get
        {
            return _handedness == Handedness.Right;
        }
    }

    public bool IsCenter
    {
        get
        {
            return _handedness == Handedness.None;
        }
    }

    void Awake()
    {
        //// TODO is this the best way to find name?
        //foreach (MaskPieceType type in System.Enum.GetValues(typeof(MaskPieceType)))
        //{
        //    if (this.transform.parent.gameObject.name.StartsWith(type.ToString()))
        //    {
        //        _type = type;
        //        break;
        //    }
        //}
        Debug.Log($"MountPoint type:{_type} {name}");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public enum Handedness
{
    Left,
    Right,
    None
}