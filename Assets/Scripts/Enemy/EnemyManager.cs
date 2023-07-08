using System.Collections.Generic;
using ElRaccoone.Timers;
using NaughtyAttributes;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [ReadOnly]
    public List<EnemyController> enemies = new List<EnemyController>();

    public float ballSpeedCutoff = 20f;

    public BallPhysicsBody ball;

    void Start()
    {
        if (TryGetComponent<EnemyInstantiator>(out var enemyInstantiator))
        {
            enemyInstantiator.OnInstantiate += go =>
            {
                // just temp timer to wait for enemy to be initialized
                Timers.SetTimeout(3000, () =>
                {
                    var enemy = go.GetComponent<EnemyController>();
                    RegisterEnemy(enemy);
                });
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ball.VelocityLenSq <= ballSpeedCutoff)
        {
            foreach (EnemyController h in enemies)
            {
                h.currentState = EnemyController.EnemyState.RUN_TOWARDS;
            }
        }
        else
        {
            foreach (EnemyController h in enemies)
            {
                h.currentState = EnemyController.EnemyState.RUN_AWAY;
            }
        }
    }

    void RegisterEnemy(EnemyController enemy)
    {
        enemies.Add(enemy);
        enemy.follow = ball.transform;
        enemy.transform.SetParent(transform);
        enemy.OnCatchBall += OnEnemyCatchBall;
    }

    void OnEnemyCatchBall(EnemyController enemy)
    {
        Debug.Log(enemy.name + " catched the ball!");
    }
}
