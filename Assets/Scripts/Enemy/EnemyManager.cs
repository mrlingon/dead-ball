using System;
using System.Collections.Generic;
using System.Linq;
using ElRaccoone.Timers;
using NaughtyAttributes;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [ReadOnly]
    public List<EnemyController> enemies = new List<EnemyController>();
    public BallPhysicsBody ball;

    public event Action AllEnemiesDead;

    void Start()
    {
        ball.KilledEnemy += (go) =>
        {
            var enemy = enemies.Find(ec => ec.gameObject == go);
            enemies.Remove(enemy);
            enemy.TriggerDeath();
            //Destroy(enemy.gameObject);
            if (enemies.Count == 0)
            {
                AllEnemiesDead?.Invoke();
            }
        };

        if (TryGetComponent<EnemyInstantiator>(out var enemyInstantiator))
        {
            enemyInstantiator.BeginInstantiation += Reset;
            enemyInstantiator.OnInstantiate += (enemy, team) =>
            {
                RegisterEnemy(enemy, team);
            };
        }
    }

    void Update()
    {
        var hasBall = EnemyHasBall();
        if (hasBall)
        {
            foreach (EnemyController enemy in enemies.Where(x => x.isDying == false))
            {
                if (enemy.currentState != EnemyController.EnemyState.HAS_BALL)
                {
                    enemy.currentState = EnemyController.EnemyState.RUN_SPAWN;
                }
            }
        }
    }

    public void Reset()
    {
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
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
        enemies.Add(enemy);
        enemy.transform.SetParent(transform);
        enemy.ball = ball;
    }

    public void ActivateEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.Activate();
        }
    }
}
