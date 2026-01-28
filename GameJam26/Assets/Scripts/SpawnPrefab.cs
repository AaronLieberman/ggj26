using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{
    public GameObject PrefabToSpawn;
    public Transform SpawnPoint;

    private void OnValidate()
    {
        if (SpawnPoint == null)
        {
            SpawnPoint = transform;
        }
    }

    public void Spawn()
    {
        Instantiate(PrefabToSpawn, SpawnPoint.position, SpawnPoint.rotation);
    }
}
