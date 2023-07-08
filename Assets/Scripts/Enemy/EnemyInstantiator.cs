using System;
using UnityEngine;

// Temporary class for spawning enemies
public class EnemyInstantiator : MonoBehaviour
{
    public GameObject enemyPrefab;

    public event Action<GameObject> OnInstantiate;

    [Range(0, 300)]
    public int number;

    private bool runOnce = false;

    private void Update()
    {
        if (runOnce) return;
        for (int i = 0; i < number; i++)
        {
            GameObject go = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            OnInstantiate?.Invoke(go);
        }
        runOnce = true;
    }
}