using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float ParallaxEffect = 0;
    Vector2 _startPos;
    Vector2 _initialCameraPos;

    bool _firstFrame = true;

    void Start()
    {
        _startPos = transform.position;
    }

    void LateUpdate()
    {
        // do this the first frame because otherwise Player may not have been initialized yet
        if (_firstFrame)
        {
            _initialCameraPos = Camera.main.transform.position;
            _firstFrame = false;
        }

        Vector2 dist = (Camera.main.transform.position - new Vector3(_initialCameraPos.x, _initialCameraPos.y, 0)) * ParallaxEffect;

        transform.position = new Vector3(_startPos.x + dist.x, _startPos.y + dist.y, transform.position.z);
    }
}
