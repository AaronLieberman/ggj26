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
        BarBeauty.SetArrowMin(customerData.maskBeauty.Min);
        BarBeauty.SetArrowMax(customerData.maskBeauty.Max);
        BarBeauty.SetMeter(maskBeauty);

        float maskScary = (float)activeMaskParts.Sum(p => p.scaryStat) / numberOfSlots;
        BarScary.SetArrowMin(customerData.maskScary.Min);
        BarScary.SetArrowMax(customerData.maskScary.Max);
        BarScary.SetMeter(maskScary);

        float maskGoofy = (float)activeMaskParts.Sum(p => p.goofyStat) / numberOfSlots;
        BarGoofy.SetArrowMin(customerData.maskGoofy.Min);
        BarGoofy.SetArrowMax(customerData.maskGoofy.Max);
        BarGoofy.SetMeter(maskGoofy);

        float maskAnonymity = (float)activeMaskParts.Sum(p => p.anonymityStat) / numberOfSlots;
        BarAnon.SetArrowMin(customerData.maskAnonymity.Min);
        BarAnon.SetArrowMax(customerData.maskAnonymity.Max);
        BarAnon.SetMeter(maskAnonymity);
    }
}
