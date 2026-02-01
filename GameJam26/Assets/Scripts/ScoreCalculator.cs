using System.Linq;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    Mask _mask;
    ShopManager _shopManager;

    void Start()
    {
        _mask = Utilities.GetRootComponentRecursive<Mask>();
        _shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
    }

    void Update()
    {
    }

    public bool GetActiveMaskAcceptable()
    {
        CustomerData customerData = _shopManager.CurrentCustomer.Data;
        var activeMaskParts = MaskPiece.GetActiveMaskPartData(_mask);

        int maskScary = activeMaskParts.Sum(p => p.scaryStat);
        int maskGoofy = activeMaskParts.Sum(p => p.goofyStat);
        int maskBeauty = activeMaskParts.Sum(p => p.beautyStat);
        int maskAnonymity = activeMaskParts.Sum(p => p.anonymityStat);

        int numberOfSlots = _mask.GetComponentsInChildren<MountPoint>()
            .Select(mp => mp.Type).Distinct().Count();

        return customerData.maskScary.InRange(maskScary, numberOfSlots) &&
            customerData.maskGoofy.InRange(maskGoofy, numberOfSlots) &&
            customerData.maskBeauty.InRange(maskBeauty, numberOfSlots) &&
            customerData.maskAnonymity.InRange(maskAnonymity, numberOfSlots);
    }

    // make sure to test GetMaskAcceptable first before using this
    public int GetActiveMaskScore()
    {
        CustomerData customerData = _shopManager.CurrentCustomer.Data;
        var activeMaskParts = MaskPiece.GetActiveMaskPartData(_mask);

        int maskScary = activeMaskParts.Sum(p => p.scaryStat) * customerData.maskScary.Points;
        int maskGoofy = activeMaskParts.Sum(p => p.goofyStat) * customerData.maskScary.Points;
        int maskBeauty = activeMaskParts.Sum(p => p.beautyStat) * customerData.maskScary.Points;
        int maskAnonymity = activeMaskParts.Sum(p => p.anonymityStat) * customerData.maskScary.Points;

        return maskScary + maskGoofy + maskBeauty + maskAnonymity;
    }
}
