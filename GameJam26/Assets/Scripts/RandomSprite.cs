using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public SpriteRenderer sprite;

    public List<Sprite> Sprites;

    private void Start()
    {
        sprite.sprite = Sprites[Random.Range(0, Sprites.Count - 1)];
    }
}
