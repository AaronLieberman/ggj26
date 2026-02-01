using UnityEngine;
using UnityEngine.UI;

public class MaskDisplayManager : MonoBehaviour
{
    private Button maskDeliveryButton;
    private ShopManager shopManager;
    private MaskComputer maskComputer;
    private Mask displayMask;

    private void Awake()
    {
        displayMask = transform.GetComponentInChildren<Mask>();
        maskDeliveryButton = transform.Find("DeliverButton").GetComponent<Button>();
        shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
        maskComputer = Utilities.GetRootComponentRecursive<MaskComputer>();
    }

    private void Update()
    {
        maskDeliveryButton.interactable = shopManager.MaskIsDeliverable && maskComputer.GetActiveMaskAcceptable();
    }

    public void ClearMaskDisplay()
    {
        foreach (MaskPiece maskPiece in MaskPiece.GetActiveMaskParts(displayMask.GetComponent<Mask>()))
        {
            GameObject.Destroy(maskPiece.gameObject);
        }
    }
}
