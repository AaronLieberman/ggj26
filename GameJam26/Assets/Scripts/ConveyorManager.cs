using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConveyorManager : MonoBehaviour
{
    [SerializeField]
    GameObject _maskPiecePrefab;

    [SerializeField]
    Sprite[] _sprites;

    [SerializeField]
    float _spawnDelaySeconds;

    [SerializeField]
    Vector3 spawnPositionVariation;

    [SerializeField]
    Vector3 _maskPieceScale;

    Transform _beltTransform;
    Transform _beltSpawnPointTransform;
    float _nextSpawnTime;
    bool _isActive;
    MaskPartData[] _parts;
    MaskPartData _overrideSpawnNext;

    void Awake()
    {
        _beltTransform = transform.Find("Belt");
        _beltSpawnPointTransform = transform.Find("MaskPieceSpawnPoint");
        var partData = MaskPartDataLoader.Load();

        var partsList = new List<MaskPartData>();
        foreach (var part in partData)
        {
            var entry = _sprites.SingleOrDefault(s => s.name == part.spriteName);
            if (entry != null)
            {
                part.sprite = entry;
                partsList.Add(part);
            }
        }

        _parts = partsList.ToArray();
    }

    void Start()
    {
        _nextSpawnTime = Time.time;
    }

    void Update()
    {
        if (_isActive && _nextSpawnTime <= Time.time)
        {
            AddToConveyor();
        }
    }

    void AddToConveyor()
    {
        MaskPartData partData;

        if (_overrideSpawnNext == null)
        {
            // need to clone because we can modify isLeft
            partData = _parts[Random.Range(0, _parts.Length)].Clone();
            if (partData.spawnsInPairs)
            {
                _overrideSpawnNext = partData;
            }
        }
        else
        {
            partData = _overrideSpawnNext.Clone();
            _overrideSpawnNext = null;
            partData.NotFlipped = false;
        }

        Vector3 randomOffset = new Vector3(Random.Range(0, spawnPositionVariation.x), Random.Range(0, spawnPositionVariation.y), Random.Range(0, spawnPositionVariation.z));
        Vector3 adjustedPosition = _beltSpawnPointTransform.position + randomOffset;

        GameObject created = Instantiate(_maskPiecePrefab, adjustedPosition, _beltSpawnPointTransform.rotation, _beltTransform);
        created.transform.localScale = _maskPieceScale;
        if (!partData.NotFlipped)
        {
            created.transform.localScale = new(-created.transform.localScale.x, created.transform.localScale.y, created.transform.localScale.z);
        }
        
        created.GetComponent<Image>().sprite = partData.sprite;
        var maskPiece = created.GetComponent<MaskPiece>();
        maskPiece.Data = partData;
        maskPiece.ApplySpriteSize(partData.sprite);

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
