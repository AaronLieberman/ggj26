using UnityEngine;

public class MountPoint : MonoBehaviour
{
    MaskPieceType _type;

    public MaskPieceType Type { get { return _type; } }

    void Awake()
    {
        // TODO is this the best way to find name?
        foreach (MaskPieceType type in System.Enum.GetValues(typeof(MaskPieceType)))
        {
            if (this.transform.parent.gameObject.name.StartsWith(type.ToString()))
            {
                _type = type;
                break;
            }
        }
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
