using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(SurfaceEffector2D))]
public class ConveyorBeltScroller : MonoBehaviour
{
    [SerializeField]
    float _pixelsPerUnit = 100f;

    RawImage _rawImage;
    SurfaceEffector2D _effector;

    void Awake()
    {
        _rawImage = GetComponent<RawImage>();
        _effector = GetComponent<SurfaceEffector2D>();
    }

    void Update()
    {
        var uvRect = _rawImage.uvRect;
        float textureWidthInWorldUnits = _rawImage.texture.width / _pixelsPerUnit;
        uvRect.x += _effector.speed * Time.deltaTime / textureWidthInWorldUnits;
        _rawImage.uvRect = uvRect;
    }
}
