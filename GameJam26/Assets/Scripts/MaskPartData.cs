using System.Collections.Generic;
using UnityEngine;

public enum MaskPartSlot
{
    Nose,
    Mouth,
    Eye,
    Horn
}

public class MaskPartData
{
    public string partName;
    public string spriteName;
    public Sprite sprite;
    public MaskPartSlot slot;
    public int scaryStat;
    public int goofyStat;
    public int beautyStat;
    public int anonymityStat;
    public bool spawnsInPairs;
    public List<string> tags = new();
}
