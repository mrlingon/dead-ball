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
    public BallPhysicsBody ball;

    void Update()
    {
        if (!ball) return;
        switch (currentState)
        {
            case EnemyState.RUN_AWAY:
                flocking.goalTransform = FindEscapePoint();
                flocking.Move();
                break;
            case EnemyState.RUN_TOWARDS:
                flocking.goalTransform = ball.transform.position;
                flocking.Move();
                CatchBall();
                break;
            case EnemyState.HAS_BALL:
                flocking.goalTransform = new Vector3(10f, 10f, 0f);
                flocking.Move();
                HasBall();
                break;
            case EnemyState.SHOOTING:
                break;
        }
    }



    Vector2 FindEscapePoint()
    {
        Vector2 directionToGoal = ball.transform.position - transform.position;
        Vector2 directionFromGoal = new Vector2(-directionToGoal.x, -directionToGoal.y);

        Vector2 rayDirection = transform.right;  // Create a direction vector going right from the position of this game object

        RaycastHit2D[] hits = Physics2D.RaycastAll(ball.transform.position, directionFromGoal, 100);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "Ground") // If the hit collider has the tag "MyTag"
            {
                return hit.point;
            }
        }
        return new Vector2(0, 0);
    }

    void CatchBall()
    {
        float dist = Vector2.Distance(ball.transform.position, transform.position);

        Debug.Log(dist);

        if (dist <= 3.0)
        {
            Debug.Log("HAS BALL");

            currentState = EnemyState.HAS_BALL;
        }
    }

    void GrabBall()
    {
        ball.SetFrozen(true);
    }

    void HasBall()
    {
        ball.transform.position = transform.position;
    }
}
