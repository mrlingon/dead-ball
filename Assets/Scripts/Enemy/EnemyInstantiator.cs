using System;
using System.Collections.Generic;
using UnityEngine;

// Temporary class for spawning enemies
public class EnemyInstantiator : MonoBehaviour
{
    public List<Transform> Team1SpawnPoints = new List<Transform>();
    public List<Transform> Team2SpawnPoints = new List<Transform>();

    public GameObject enemyPrefab;

    public event Action<GameObject> OnInstantiate;

    [Range(0, 300)]
    public int number;

    private bool runOnce = false;

    private void Update()
    {
        if (runOnce) return;

        foreach (Transform t in Team1SpawnPoints)
        {
            GameObject go = Instantiate(enemyPrefab, t.position, Quaternion.identity);
            OnInstantiate?.Invoke(go);
        }

        foreach (Transform t in Team2SpawnPoints)
        {
            GameObject go = Instantiate(enemyPrefab, t.position, Quaternion.identity);
            OnInstantiate?.Invoke(go);
        }

        runOnce = true;
    }
}
