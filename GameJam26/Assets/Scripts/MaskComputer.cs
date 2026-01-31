using System.Linq;
using UnityEngine;

public class MaskComputer : MonoBehaviour
{
    Mask _mask;
    ShopManager _shopManager;

    void Start()
    {
        _mask = transform.Find("Mask").GetComponent<Mask>();
        _shopManager = transform.Find("ShopManager").GetComponent<ShopManager>();
    }

    void Update()
    {
    }

    public MaskPartData[] GetActiveMaskParts()
    {
        var maskPieces = _mask.GetComponentsInChildren<MaskPiece>();
        var maskPartData = maskPieces.Select(piece => piece.Data);
        return maskPartData.ToArray();
    }

    public bool GetActiveMaskAcceptable()
    {
        CustomerData customerData = _shopManager.CurrentCustomer.Data;
        var activeMaskParts = GetActiveMaskParts();

        int maskScary = activeMaskParts.Sum(p => p.scaryStat);
        int maskGoofy = activeMaskParts.Sum(p => p.goofyStat);
        int maskBeauty = activeMaskParts.Sum(p => p.beautyStat);
        int maskAnonymity = activeMaskParts.Sum(p => p.anonymityStat);

        int numberOfSlots = 6; // TODO get from current mask base

        return customerData.maskScary.InRange(maskScary, numberOfSlots) &&
            customerData.maskGoofy.InRange(maskGoofy, numberOfSlots) &&
            customerData.maskBeauty.InRange(maskBeauty, numberOfSlots) &&
            customerData.maskAnonymity.InRange(maskAnonymity, numberOfSlots);
    }

    // make sure to test GetMaskAcceptable first before using this
    public int GetActiveMaskScore()
    {
        CustomerData customerData = _shopManager.CurrentCustomer.Data;
        var activeMaskParts = GetActiveMaskParts();

        int maskScary = activeMaskParts.Sum(p => p.scaryStat) * customerData.maskScary.Points;
        int maskGoofy = activeMaskParts.Sum(p => p.goofyStat) * customerData.maskScary.Points;
        int maskBeauty = activeMaskParts.Sum(p => p.beautyStat) * customerData.maskScary.Points;
        int maskAnonymity = activeMaskParts.Sum(p => p.anonymityStat) * customerData.maskScary.Points;

        return maskScary + maskGoofy + maskBeauty + maskAnonymity;
    }
}
