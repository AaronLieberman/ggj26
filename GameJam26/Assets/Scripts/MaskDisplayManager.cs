using UnityEngine;
using UnityEngine.UI;

public class MaskDisplayManager : MonoBehaviour
{
    private Button maskDeliveryButton;
    private ShopManager shopManager;
    private MaskComputer maskComputer;

    private void Awake()
    {
        maskDeliveryButton = transform.Find("DeliverButton").GetComponent<Button>();
        shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
        maskComputer = Utilities.GetRootComponentRecursive<MaskComputer>();
    }

    private void Update()
    {
        maskDeliveryButton.interactable = shopManager.MaskIsDeliverable /*&& maskComputer.GetActiveMaskAcceptable()*/;
    }
}
