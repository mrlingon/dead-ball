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
            Debug.Log("YOOO");
            enemyInstantiator.OnInstantiate += go =>
            {
                enemies.Add(go.GetComponent<EnemyController>());
                if (go.TryGetComponent<EnemyController>(out var h))
                {
                    h.ballTransform = ball.transform;
                }
                go.transform.SetParent(transform);
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
}
