using UnityEngine;

public class DeliverMask : MonoBehaviour
{
    public void DeliverMaskClick()
    {
        Utilities.GetRootComponentRecursive<GameManager>().DeliverMask();
    }
}
