using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlphaHitTest : MonoBehaviour
{
    public float AlphaHitTestMinimumThreshold = 0.1f;

    void Awake()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaHitTestMinimumThreshold;
    }
}
