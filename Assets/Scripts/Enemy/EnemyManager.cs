using System.Collections.Generic;
using ElRaccoone.Timers;
using NaughtyAttributes;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [ReadOnly]
    public List<EnemyController> enemies = new List<EnemyController>();
    public BallPhysicsBody ball;
    public Transform kickoffPointOne;
    public Transform kickoffPointTwo;

    void Start()
    {
        if (TryGetComponent<EnemyInstantiator>(out var enemyInstantiator))
        {
            enemyInstantiator.OnInstantiate += (enemy, team) =>
            {
                // just temp timer to wait for enemy to be initialized
                Timers.SetTimeout(3000, () =>
                {
                    RegisterEnemy(enemy, team);
                });
            };
        }
    }

    void Update()
    {
        var hasBall = EnemyHasBall();
        if (hasBall)
        {
            foreach (EnemyController enemy in enemies)
            {
                if (enemy.currentState != EnemyController.EnemyState.HAS_BALL)
                {
                    enemy.currentState = EnemyController.EnemyState.RUN_SPAWN;
                }
            }
        }
    }

    bool EnemyHasBall()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.currentState == EnemyController.EnemyState.HAS_BALL) return true;
        }
        return false;
    }

    void RegisterEnemy(EnemyController enemy, Team team)
    {
        enemy.transform.localScale = enemy.transform.localScale * enemy.enemyData.size;

        var renderer = enemy.GetComponentInChildren<SpriteRenderer>();
        renderer.color = new Color(team.color.r, team.color.g, team.color.b, 1);

        enemies.Add(enemy);
        enemy.ball = ball;
        enemy.transform.SetParent(transform);
        enemy.transform.name = enemy.enemyData.enemyName;
        enemy.shootPosition = kickoffPointOne.position;
    }
}
