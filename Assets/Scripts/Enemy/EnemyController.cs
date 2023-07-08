using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;

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

    private FlockTowardsPoint flock;
    [HideInInspector]
    public Transform follow;

    [Range(0, 30)]
    public float distanceToPoint = 3.5f;

    [Range(0, 30)]
    public float distanceToSelf = 9.5f;

    public event Action<EnemyController> OnCatchBall;

    void Start()
    {
        TryGetComponent<FlockTowardsPoint>(out flock);
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

    Vector2[] SuitableEscapePoints()
    {
        Vector2 directionToGoal = follow.position - transform.position;
        Vector2 directionFromGoal = new Vector2(-directionToGoal.x, -directionToGoal.y);
        var startAngle = Mathf.Atan2(directionFromGoal.y, directionFromGoal.x) * Mathf.Rad2Deg;
        int numberOfRays = 12;
        Vector2[] points = new Vector2[numberOfRays];
        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = ((i / (float)numberOfRays) * 360 + startAngle) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            RaycastHit2D[] hits = Physics2D.RaycastAll(follow.position, direction, 100);
            foreach (RaycastHit2D hit in hits.OrderBy(hit => hit.distance))
            {
                if (hit.collider.tag == "Ground")
                {
                    //DebugDraw.Circle(hit.point, 1, Color.red);
                    points[i] = hit.point;
                    break;
                }
            }
        }
        return points;
    }

    Vector2 FindEscapePoint()
    {
        Vector2 directionToGoal = follow.position - transform.position;
        Vector2 directionFromGoal = new Vector2(-directionToGoal.x, -directionToGoal.y);
        Vector2[] points = SuitableEscapePoints();

        float distanceDefaultPointToFollow = math.distance(points[0], new Vector2(transform.position.x, transform.position.y));
        if (directionToGoal.magnitude < distanceToSelf && distanceDefaultPointToFollow < distanceToPoint)
        {
            Vector2 bestPoint = points[0];
            float bestValue = (new Vector2(follow.position.x, follow.position.y) - bestPoint).magnitude;
            for (int i = 1; i < points.Length; i++)
            {
                var value = (new Vector2(follow.position.x, follow.position.y) - points[i]).magnitude;
                if (value > bestValue + 2f)
                {
                    bestPoint = points[i];
                    bestValue = value;
                }
            }
            DebugDraw.Line(new Vector3(bestPoint.x, bestPoint.y, 0), transform.position, Color.black);
            return bestPoint;
        }
        else
        {
            DebugDraw.Line(new Vector3(points[0].x, points[0].y, 0), transform.position, Color.black);
            return points[0];
        }

    }

    void HasBall()
    {

    }
}
