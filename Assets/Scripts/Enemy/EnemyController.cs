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

    [SerializeField]
    public EnemyState currentState = EnemyState.RUN_TOWARDS;

    public Flocking flocking;
    public Transform ballTransform;

    void Update()
    {
        if (!ballTransform) return;
        switch (currentState)
        {
            case EnemyState.RUN_AWAY:
                flocking.goalTransform = FindEscapePoint();
                flocking.Move();
                break;
            case EnemyState.RUN_TOWARDS:
                flocking.goalTransform = ballTransform.position;
                flocking.Move();
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
        Vector2 directionToGoal = ballTransform.position - transform.position;
        Vector2 directionFromGoal = new Vector2(-directionToGoal.x, -directionToGoal.y);

        Vector2 rayDirection = transform.right;  // Create a direction vector going right from the position of this game object

        RaycastHit2D[] hits = Physics2D.RaycastAll(ballTransform.position, directionFromGoal, 100);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "Ground") // If the hit collider has the tag "MyTag"
            {
                Debug.Log("We hit " + hit.collider.name);
                Debug.Log("Point where raycast hit: " + hit.point);
                return hit.point;
            }
        }
        return new Vector2(0, 0);
    }

    void HasBall()
    {

    }
}
