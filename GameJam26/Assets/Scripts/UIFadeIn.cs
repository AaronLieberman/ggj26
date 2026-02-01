using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIFadeIn : MonoBehaviour
{
    public float delay = 0f;
    public float fadeDuration = 0.5f;
    public float moveDuration = 0.5f;
    public Vector2 startOffset = new Vector2(0f, -50f);
    public AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public bool autoStart = true;

    Image _image;
    RectTransform _rect;
    Color _baseColor;
    Vector2 _targetAnchoredPos;
    Vector2 _startAnchoredPos;

    float _elapsed = 0f;
    float _maxDuration = 0.0001f;
    bool _isPlaying = false;

    void Awake()
    {
        _image = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();

        _baseColor = _image.color;
        _image.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, 0f);

        _targetAnchoredPos = _rect.anchoredPosition;
        _startAnchoredPos = _targetAnchoredPos + startOffset;

        _rect.anchoredPosition = _startAnchoredPos;
    }

    void Start()
    {
        if (autoStart) StartFadeIn();
    }

    void Update()
    {
        if (!_isPlaying) return;

        _elapsed += Time.deltaTime;
        float elapsedClamped = Mathf.Max(0f, _elapsed);

        float tFade = fadeDuration > 0f ? Mathf.Clamp01(elapsedClamped / fadeDuration) : 1f;
        float tMove = moveDuration > 0f ? Mathf.Clamp01(elapsedClamped / moveDuration) : 1f;

        float alphaT = fadeCurve.Evaluate(tFade);
        float moveT = moveCurve.Evaluate(tMove);

        _image.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, alphaT * _baseColor.a);
        _rect.anchoredPosition = Vector2.Lerp(_startAnchoredPos, _targetAnchoredPos, moveT);

        if (_elapsed >= _maxDuration)
        {
            _image.color = _baseColor;
            _rect.anchoredPosition = _targetAnchoredPos;
            _isPlaying = false;
        }
    }

    public void StartFadeIn()
    {
        _elapsed = -Mathf.Max(0f, delay);
        _maxDuration = Mathf.Max(0.0001f, Mathf.Max(fadeDuration, moveDuration));
        _isPlaying = true;
    }

    public void StopFadeIn(bool complete = false)
    {
        _isPlaying = false;
        if (complete)
        {
            _image.color = _baseColor;
            _rect.anchoredPosition = _targetAnchoredPos;
        }
    }
}
