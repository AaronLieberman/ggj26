using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaskDeliverableDisplay : MonoBehaviour
{
    public float AlphaDim = 0.5f;

    void Update()
    {
        float newAlpha = Utilities.GetRootComponent<ScoreCalculator>().GetActiveMaskAcceptable()
            ? 1
            : AlphaDim;

        var image = GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);;
        }

        var text = GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, newAlpha);;
        }
    }
}
