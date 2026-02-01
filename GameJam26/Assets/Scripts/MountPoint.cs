using UnityEngine;

public class MountPoint : MonoBehaviour
{
    [SerializeField]
    MaskPartSlot _type;

    [SerializeField]
    Handedness _handedness;


    public MaskPartSlot Type { get { return _type; } }
    public Handedness Handedness { get { return _handedness; } }

    public bool IsLeft
    {
        get
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