using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MaskPartSlot
{
    Nose,
    Mouth,
    Eye,
    Horn,
    Ear
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
    public int artRequestPriority;
    public bool spawnsInPairs;
    public List<string> tags = new ();
    public bool Notflipped = true;

    public MaskPartData Clone()
    {
        MaskPartData copy = new();
        copy.partName = partName;
        copy.spriteName = spriteName;
        copy.sprite = sprite;
        copy.slot = slot;
        copy.scaryStat = scaryStat;
        copy.goofyStat = goofyStat;
        copy.beautyStat = beautyStat;
        copy.anonymityStat = anonymityStat;
        copy.artRequestPriority = artRequestPriority;
        copy.spawnsInPairs = spawnsInPairs;
        copy.tags = tags.ToList();
        copy.Notflipped = Notflipped;
        return copy;
    }
}
