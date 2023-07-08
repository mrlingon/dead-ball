using System;
using System.Collections.Generic;
using UnityEngine;

// Temporary class for spawning enemies
public class EnemyInstantiator : MonoBehaviour
{
    public List<Transform> Team1SpawnPoints = new List<Transform>();
    public List<Transform> Team2SpawnPoints = new List<Transform>();

    public Transform shootPositionOne;
    public Transform shootPositionTwo;

    public GameObject enemyPrefab;

    public event Action BeginInstantiation;
    public event Action<EnemyController, Team> OnInstantiate;

    public void SetUpLevel(Level level)
    {
        BeginInstantiation?.Invoke();
        SetUpTeam(level.teamOne, Team1SpawnPoints, 1);
        SetUpTeam(level.teamTwo, Team2SpawnPoints, 2);
    }

    private void SetUpTeam(Team team, List<Transform> spawnPoints, int teamIndex)
    {
        int i = 0;
        foreach (var enemy in team.enemies)
        {
            if (spawnPoints.Count - 1 < i)
            {
                Debug.LogWarning("Not enough spawn points.");
                break;
            }
            var gameObject = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);
            var enemyController = gameObject.GetComponent<EnemyController>();
            enemyController.enemyData = enemy;

            if (teamIndex == 1) enemyController.shootPosition = shootPositionOne.position;
            if (teamIndex == 2) enemyController.shootPosition = shootPositionTwo.position;

            enemyController.team = teamIndex;

            enemyController.transform.name = enemyController.enemyData.enemyName;

            enemyController.transform.localScale = enemyController.transform.localScale * enemyController.enemyData.size;
            var renderer = enemyController.GetComponentInChildren<SpriteRenderer>();
            renderer.color = new Color(team.color.r, team.color.g, team.color.b, 1);

            enemyController.OnSpawn();
            OnInstantiate?.Invoke(enemyController, team);
            i++;
        }
    }
}
