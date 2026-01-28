using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HideSprite : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    public void Hide()
    {
        _sprite.enabled = false;
    }

    public void Show()
    {
        _sprite.enabled = true;
    }
}