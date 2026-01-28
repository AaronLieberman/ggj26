using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyResources))]
public class EnemyDeathHandler : MonoBehaviour
{
    private EnemyResources enemyResources;

    private void Awake()
    {
        enemyResources = GetComponent<EnemyResources>();
    }

    private void Update()
    {
        if (enemyResources.Health <= 0)
            HandleDeath();
    }

    private void HandleDeath()
    {
        SpawnPrefab spawnPrefab = GetComponent<SpawnPrefab>();
        if (spawnPrefab != null)
            spawnPrefab.Spawn();

        Destroy(gameObject);
    }
}
