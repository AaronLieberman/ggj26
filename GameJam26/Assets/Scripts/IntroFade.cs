using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class IntroFade : MonoBehaviour
{
    public GameObject Fade;
    TaskCompletionSource<bool> _done = new TaskCompletionSource<bool>();

    private void Start()
    {
    }

    public Task Go()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.enabled = true;
        return _done.Task;
    }

    void FadeComplete()
    {
        Fade.SetActive(false);
    }

    void TextComplete()
    {
        _done.SetResult(true);
    }
}
