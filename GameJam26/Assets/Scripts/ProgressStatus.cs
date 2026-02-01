using System.Linq;
using UnityEngine;

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
        {
            return;
        }

        CustomerData customerData = _shopManager.CurrentCustomer.Data;
        var mask = Utilities.GetRootComponentRecursive<Mask>();
        // var md = GameObject.Find("MaskDisplay");
        // var m = md.transform.Find("Mask");
        // var c = md.GetComponent<Mask>();
        // var mask = GameObject.Find("MaskDisplay").transform.Find("Mask").GetComponent<Mask>();
        var activeMaskParts = MaskPiece.GetActiveMaskPartData(mask);

        int maskBeauty = activeMaskParts.Sum(p => p.beautyStat);
        BarBeauty.SetArrowMin(customerData.maskBeauty.Min);
        BarBeauty.SetArrowMax(customerData.maskBeauty.Max);
        BarBeauty.SetMeter(maskBeauty);

        int maskScary = activeMaskParts.Sum(p => p.scaryStat);
        BarScary.SetArrowMin(customerData.maskScary.Min);
        BarScary.SetArrowMax(customerData.maskScary.Max);
        BarScary.SetMeter(maskScary);

        int maskGoofy = activeMaskParts.Sum(p => p.goofyStat);
        BarGoofy.SetArrowMin(customerData.maskGoofy.Min);
        BarGoofy.SetArrowMax(customerData.maskGoofy.Max);
        BarGoofy.SetMeter(maskGoofy);

        int maskAnonymity = activeMaskParts.Sum(p => p.anonymityStat);
        BarAnon.SetArrowMin(customerData.maskAnonymity.Min);
        BarAnon.SetArrowMax(customerData.maskAnonymity.Max);
        BarAnon.SetMeter(maskAnonymity);
    }
}
