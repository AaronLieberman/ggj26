using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFinish : MonoBehaviour
{
    public Canvas MainCanvas;
    public Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ShowEndScreen();
        }
    }

    void ShowEndScreen()
    {
        MainCanvas.enabled = true;
        animator.Play("Fade");
        StartCoroutine(QuitAfterFinish());
    }

    IEnumerator QuitAfterFinish()
    {
        yield return Utilities.WaitForSeconds(3f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
