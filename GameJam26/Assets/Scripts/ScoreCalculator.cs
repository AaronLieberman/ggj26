using System.Linq;
using UnityEngine;
using System;

public class ScoreCalculator : MonoBehaviour
{
    ShopManager _shopManager;

    void Start()
    {
        _shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
    }

    public bool GetActiveMaskAcceptable()
    {
        CustomerData customerData = _shopManager.CurrentCustomer.Data;
        var mask = GameObject.Find("MaskDisplay").GetComponentInChildren<Mask>();
        var activeMaskParts = MaskPiece.GetActiveMaskPartData(mask);

        var mountPoints = mask.GetComponentsInChildren<MountPoint>();
        int numberOfSlots = mountPoints.Select(mp => Tuple.Create(mp.Type, mp.Handedness)).Distinct().Count();

        int maskScary = activeMaskParts.Sum(p => p.scaryStat);
        int maskGoofy = activeMaskParts.Sum(p => p.goofyStat);
        int maskBeauty = activeMaskParts.Sum(p => p.beautyStat);
        int maskAnonymity = activeMaskParts.Sum(p => p.anonymityStat);

        bool maskScaryInRange = customerData.maskScary.InRange(maskScary, numberOfSlots);
        bool maskGoofyInRange = customerData.maskGoofy.InRange(maskGoofy, numberOfSlots);
        bool maskBeautyInRange = customerData.maskBeauty.InRange(maskBeauty, numberOfSlots);
        bool maskAnonymityInRange = customerData.maskAnonymity.InRange(maskAnonymity, numberOfSlots);

        return maskScaryInRange && maskGoofyInRange && maskBeautyInRange && maskAnonymityInRange;
    }

    // make sure to test GetMaskAcceptable first before using this
    public int GetActiveMaskScore()
    {
        CustomerData customerData = _shopManager.CurrentCustomer.Data;
        var mask = GameObject.Find("MaskDisplay").GetComponentInChildren<Mask>();
        var activeMaskParts = MaskPiece.GetActiveMaskPartData(mask);

        int maskScary = activeMaskParts.Sum(p => p.scaryStat) * customerData.maskScary.Points;
        int maskGoofy = activeMaskParts.Sum(p => p.goofyStat) * customerData.maskScary.Points;
        int maskBeauty = activeMaskParts.Sum(p => p.beautyStat) * customerData.maskScary.Points;
        int maskAnonymity = activeMaskParts.Sum(p => p.anonymityStat) * customerData.maskScary.Points;

        return maskScary + maskGoofy + maskBeauty + maskAnonymity;
    }
}
