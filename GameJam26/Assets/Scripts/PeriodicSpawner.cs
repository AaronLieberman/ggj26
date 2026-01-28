using UnityEngine;

public class PeriodicSpawner : MonoBehaviour
{
    public float RateInSeconds = 1;
    public float InitialWait = 1;
    public GameObject SpawnPrefab;
    public Transform SpawnPosition;
    
    bool _enabled;
    float _lastSpawn;

    public void SetEnabled(bool v)
    {
        _enabled = v;
        _lastSpawn = Time.time - (RateInSeconds - InitialWait);
    }

    void Update()
    {
        if (SpawnPrefab == null || SpawnPosition == null || !_enabled)
            return;

        if (Time.time - _lastSpawn > RateInSeconds)
        {
            Instantiate(SpawnPrefab, SpawnPosition.position, Quaternion.identity);
            _lastSpawn = Time.time;
        }
    }
}
