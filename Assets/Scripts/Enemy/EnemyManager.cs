using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyController> enemies = new List<EnemyController>();
    public float ballSpeedCutoff = 1f;
    public BallPhysicsBody ball;

    void Start()
    {
        if (TryGetComponent<EnemyInstantiator>(out var enemyInstantiator))
        {
            enemyInstantiator.OnInstantiate += go =>
            {
                var enemy = go.GetComponent<EnemyController>();
                RegisterEnemy(enemy);
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ball.Rigidbody.velocity.magnitude <= ballSpeedCutoff)
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
