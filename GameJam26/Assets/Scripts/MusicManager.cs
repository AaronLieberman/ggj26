using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] float _fadeDuration = 0.5f;

    AudioSource _audioSource;
    float _targetVolume;
    Coroutine _fadeCoroutine;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _targetVolume = _audioSource.volume;
    }

    public void FadeOut()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeTo(0f));
    }

    public void FadeIn()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeTo(_targetVolume));
    }

    IEnumerator FadeTo(float target)
    {
        float startVolume = _audioSource.volume;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _audioSource.volume = Mathf.Lerp(startVolume, target, elapsed / _fadeDuration);
            yield return null;
        }

        _audioSource.volume = target;
        _fadeCoroutine = null;
    }
}
