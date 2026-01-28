using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector]
    public Transform Player;

    [SerializeField] float ShakeAmount = 1;

    float _shakeSecondsRemaining;

    public void PlaySlideAnimation(GameObject o)
    {
        Debug.Log("Camera following " + o.name);

        GetComponent<Animator>().speed = Time.timeScale;
        GetComponent<Animator>().SetTrigger("Slide");
    }

    public IEnumerator Shake(float seconds)
    {
        _shakeSecondsRemaining = seconds;
        Debug.Log("Camera shake");
        yield return new WaitWhile(() => _shakeSecondsRemaining > 0);
    }

    private void Start()
    {
#if !UNITY_WEBGL
        //Screen.SetResolution(1280, 720, FullScreenMode.Windowed, new RefreshRate() { numerator = 60, denominator = 1 });
#endif
    }

    void Update()
    {
        if (_shakeSecondsRemaining > 0)
        {
            transform.parent.localPosition = UnityEngine.Random.insideUnitSphere * ShakeAmount;
            _shakeSecondsRemaining -= Time.deltaTime * Time.timeScale;
            if (_shakeSecondsRemaining <= 0)
            {
                transform.parent.localPosition = new Vector3();
                _shakeSecondsRemaining = 0;
            }
        }
    }
}
