using UnityEngine;
using UnityEngine.UI;

public class MaskDisplayManager : MonoBehaviour
{
    private Button maskDeliveryButton;
    private ShopManager shopManager;
    private ScoreCalculator ScoreCalculator;
    private Mask displayMask;

    private void Awake()
    {
        displayMask = transform.GetComponentInChildren<Mask>();
        maskDeliveryButton = transform.Find("DeliverButton").GetComponent<Button>();
        shopManager = Utilities.GetRootComponentRecursive<ShopManager>();
        ScoreCalculator = Utilities.GetRootComponentRecursive<ScoreCalculator>();
    }

    private void Update()
    {
        maskDeliveryButton.interactable = shopManager.MaskIsDeliverable &&
            (ScoreCalculator.GetActiveStatsScore()?.IsAcceptable() ?? false);
    }

    public void ClearMaskDisplay()
    {
        displayMask = transform.GetComponentInChildren<Mask>();
        if (displayMask == null)
        {
            return;
        }

        displayMask.transform.SetParent(GameObject.Find("MaskExit").transform, true);
        displayMask.FlyOff();
        displayMask.enabled = false;
    }
}
