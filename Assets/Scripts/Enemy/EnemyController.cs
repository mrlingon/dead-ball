using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public enum EnemyState
    {
        RUN_AWAY,
        RUN_TOWARDS,
        HAS_BALL,
        SHOOTING,
    }

    public EnemyData humanData;
    public EnemyState currentState = EnemyState.RUN_TOWARDS;
    public PhysicsEvents2D ballCollisionEvents;

    private FlockTowardsPoint flock;
    [HideInInspector]
    public Transform follow;

    public event Action<EnemyController> OnCatchBall;

    void Start()
    {
        TryGetComponent<FlockTowardsPoint>(out flock);
        ballCollisionEvents.TriggerEnter += other =>
        {
            OnCatchBall?.Invoke(this);
        };
    }

    void Update()
    {
        if (!follow) return;
        switch (currentState)
        {
            case EnemyState.RUN_AWAY:
                flock.goalTransform = FindEscapePoint();
                flock.Move();
                break;
            case EnemyState.RUN_TOWARDS:
                flock.goalTransform = follow.position;
                flock.Move();
                break;
            case EnemyState.HAS_BALL:
                HasBall();
                break;
            case EnemyState.SHOOTING:
                break;
        }
    }

    Vector2 FindEscapePoint()
    {
        Vector2 directionToGoal = follow.position - transform.position;
        Vector2 directionFromGoal = new Vector2(-directionToGoal.x, -directionToGoal.y);

        Vector2 rayDirection = transform.right;  // Create a direction vector going right from the position of this game object

        RaycastHit2D[] hits = Physics2D.RaycastAll(follow.position, directionFromGoal, 100);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "Ground")
            {
                return hit.point;
            }
        }
        return new Vector2(0, 0);
    }

    void HasBall()
    {

    }
}
