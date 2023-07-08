using System;
using System.Collections.Generic;
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
            Destroy(enemy.gameObject);
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
            foreach (EnemyController enemy in enemies)
            {
                if (enemy.currentState != EnemyController.EnemyState.HAS_BALL)
                {
                    enemy.currentState = EnemyController.EnemyState.RUN_SPAWN;
                }
            }
        }
    }

    void Reset()
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
        enemy.transform.localScale = enemy.transform.localScale * enemy.enemyData.size;

        var renderer = enemy.GetComponentInChildren<SpriteRenderer>();
        renderer.color = new Color(team.color.r, team.color.g, team.color.b, 1);
        enemies.Add(enemy);
        enemy.ball = ball;
        enemy.transform.SetParent(transform);
        enemy.transform.name = enemy.enemyData.enemyName;
    }

    public void ActivateEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.Activate();
        }
    }
}