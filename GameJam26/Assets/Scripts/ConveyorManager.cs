using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct MaskPartEntry
{
    public string name;
    public Sprite sprite;
}

public class ConveyorManager : MonoBehaviour
{
    [SerializeField]
    GameObject _maskPiecePrefab;

    [SerializeField]
    MaskPartEntry[] _sprites;

    [SerializeField]
    float _speed;

    [SerializeField]
    float _spawnDelaySeconds;

    Transform _beltTransform;
    Transform _beltSpawnPointTransform;
    float _nextSpawnTime;
    bool _isActive;
    MaskPartData[] _parts;

    void Awake()
    {
        _beltTransform = transform.Find("Belt");
        _beltSpawnPointTransform = transform.Find("MaskPieceSpawnPoint");
        _parts = MaskPartDataLoader.Load();

        foreach (var part in _parts)
        {
            var entry = _sprites.SingleOrDefault(s => s.name == part.spriteName);
            if (entry.sprite != null)
                part.sprite = entry.sprite;
        }
    }

    void Start()
    {
        _nextSpawnTime = Time.time;
    }

    void Update()
    {
        if (_isActive && _nextSpawnTime <= Time.time)
            AddToConveyor();
    }

    void AddToConveyor()
    {
        var partData = _parts[Random.Range(0, _parts.Length)];

        GameObject created = Instantiate(_maskPiecePrefab, _beltSpawnPointTransform.position, _beltSpawnPointTransform.rotation, _beltTransform);
        created.GetComponent<Rigidbody2D>().AddForce(new Vector2(-_speed, 0.0f));

        if (partData.sprite != null)
            created.GetComponent<Image>().sprite = partData.sprite;

        created.GetComponent<MaskPiece>().Data = partData;

        _nextSpawnTime = Time.time + _spawnDelaySeconds;
    }

    public void ActivateManager()
    {
        _isActive = true;
    }

    public void DeactivateManager()
    {
        _isActive = false;
    }
}
