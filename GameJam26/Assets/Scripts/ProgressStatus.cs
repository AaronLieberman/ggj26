using System.Linq;
using UnityEngine;
using System;

public class ProgressStatus : MonoBehaviour
{
    public BuildProgressBar BarBeauty;
    public BuildProgressBar BarScary;
    public BuildProgressBar BarGoofy;
    public BuildProgressBar BarAnon;

    ShopManager _shopManager;

    void Start()
    {
        _shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
    }

    void Update()
    {
        if ( _shopManager.CurrentCustomer == null )
            return;

        CustomerData customerData = _shopManager.CurrentCustomer.Data;
        var mask = GameObject.Find("MaskDisplay").GetComponentInChildren<Mask>();
        if (mask == null)
        {
            return;
        }
        var activeMaskParts = MaskPiece.GetActiveMaskPartData(mask);

        var mountPoints = mask.GetComponentsInChildren<MountPoint>();
        int numberOfSlots = mountPoints.Select(mp => Tuple.Create(mp.Type, mp.Handedness)).Distinct().Count();

        float maskBeauty = (float)activeMaskParts.Sum(p => p.beautyStat) / numberOfSlots;
        bool maskBeautyInRange = customerData.maskBeauty.InRange(maskBeauty);
        BarBeauty.SetArrowMin(customerData.maskBeauty.Min);
        BarBeauty.SetArrowMax(customerData.maskBeauty.Max);
        BarBeauty.SetMeter(maskBeauty);
        BarBeauty.SetInRange(maskBeautyInRange);

        float maskScary = (float)activeMaskParts.Sum(p => p.scaryStat) / numberOfSlots;
        bool maskScaryInRange = customerData.maskScary.InRange(maskScary);
        BarScary.SetArrowMin(customerData.maskScary.Min);
        BarScary.SetArrowMax(customerData.maskScary.Max);
        BarScary.SetMeter(maskScary);
        BarScary.SetInRange(maskScaryInRange);

        float maskGoofy = (float)activeMaskParts.Sum(p => p.goofyStat) / numberOfSlots;
        bool maskGoofyInRange = customerData.maskGoofy.InRange(maskGoofy);
        BarGoofy.SetArrowMin(customerData.maskGoofy.Min);
        BarGoofy.SetArrowMax(customerData.maskGoofy.Max);
        BarGoofy.SetMeter(maskGoofy);
        BarGoofy.SetInRange(maskGoofyInRange);

        float maskAnonymity = (float)activeMaskParts.Sum(p => p.anonymityStat) / numberOfSlots;
        bool maskAnonymityInRange = customerData.maskAnonymity.InRange(maskAnonymity);
        BarAnon.SetArrowMin(customerData.maskAnonymity.Min);
        BarAnon.SetArrowMax(customerData.maskAnonymity.Max);
        BarAnon.SetMeter(maskAnonymity);
        BarAnon.SetInRange(maskAnonymityInRange);
    }
}
