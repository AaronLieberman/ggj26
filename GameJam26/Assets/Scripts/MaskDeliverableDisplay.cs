using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaskDeliverableDisplay : MonoBehaviour
{
    public float AlphaDim = 0.5f;

    void Update()
    {
        var score = Utilities.GetRootComponent<ScoreCalculator>().GetActiveStatsScore();
        float newAlpha = (score?.IsAcceptable() ?? false)
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
