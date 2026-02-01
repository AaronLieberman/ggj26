using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIPulse : MonoBehaviour
{
    public Color pulseColor = Color.white;
    [Range(0f, 1f)] public float baseIntensity = 0.2f;
    [Range(0f, 1f)] public float intensity = 0.8f;
    public float speed = 2f;

    Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        img.color = Color.Lerp(new Color(255,255,255,0), pulseColor, baseIntensity);
    }

    private void Update()
    {
        float pct = (Mathf.Sin(Time.time * (2f * Mathf.PI / speed)) * 0.5f) + 0.5f;
        img.color = Color.Lerp(new Color(pulseColor.r, pulseColor.g, pulseColor.b, 0), pulseColor, baseIntensity + intensity * pct);
    }
}