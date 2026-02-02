using UnityEngine;
using System.Collections;

public class CelebrationAnim : MonoBehaviour
{
    private RectTransform _rect;

    void Awake() => _rect = GetComponent<RectTransform>();

    public void StartCelebration(float duration = 2.0f)
    {
        StartCoroutine(CelebrateCoroutine(duration));
    }

    private IEnumerator CelebrateCoroutine(float duration)
    {
        Vector2 startPos = _rect.anchoredPosition;
        float elapsed = 0;

        float seed = Random.value * 100f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            float noiseX = Mathf.PerlinNoise(seed + Time.time * 2f, 0) - 0.5f;
            float noiseY = Mathf.PerlinNoise(0, seed + Time.time * 2f) - 0.5f;
            Vector2 drift = new Vector2(noiseX, noiseY) * 50f; // 50px drift range

            float bounce = Mathf.Abs(Mathf.Sin(elapsed * 12f)) * 60f * (1 - t);

            _rect.anchoredPosition = startPos + drift + new Vector2(0, bounce);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _rect.anchoredPosition = startPos;
    }
}