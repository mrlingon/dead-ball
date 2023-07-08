using System;
using System.Collections.Generic;
using UnityEngine;

// Temporary class for spawning enemies
public class EnemyInstantiator : MonoBehaviour
{
    public List<Transform> Team1SpawnPoints = new List<Transform>();
    public List<Transform> Team2SpawnPoints = new List<Transform>();

    public Team teamOne;
    public Team teamTwo;

    public GameObject enemyPrefab;

    public event Action<EnemyController, Team> OnInstantiate;

    private bool runOnce = false;

    private void Update()
    {
        if (runOnce) return;
        var i = 0;
        foreach (var enemy in teamOne.enemies)
        {
            if (Team1SpawnPoints.Count - 1 < i)
            {
                Debug.LogWarning("Not enough spawn points.");
                break;
            }
            var gameObject = Instantiate(enemyPrefab, Team1SpawnPoints[i].position, Quaternion.identity);
            var enemyController = gameObject.GetComponent<EnemyController>();
            enemyController.enemyData = enemy;
            OnInstantiate?.Invoke(enemyController, teamOne);
            i++;
        }
        i = 0;
        foreach (var enemy in teamTwo.enemies)
        {
            if (Team2SpawnPoints.Count - 1 < i)
            {
                Debug.LogWarning("Not enough spawn points.");
                break;
            }
            var gameObject = Instantiate(enemyPrefab, Team2SpawnPoints[i].position, Quaternion.identity);
            var enemyController = gameObject.GetComponent<EnemyController>();
            Debug.Log(enemyController);
            enemyController.enemyData = enemy;
            OnInstantiate?.Invoke(enemyController, teamTwo);
            i++;
        }
        runOnce = true;
    }
}
