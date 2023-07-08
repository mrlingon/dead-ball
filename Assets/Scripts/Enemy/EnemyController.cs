using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using ElRaccoone.Timers;

public class EnemyController : MonoBehaviour
{

    public enum EnemyState
    {
        RUN_AWAY,
        RUN_TOWARDS,
        RUN_SPAWN,
        HAS_BALL,
    }
    public EnemyState currentState = EnemyState.RUN_TOWARDS;
    public Enemy enemyData;

    public Rigidbody2D Rigidbody { get; private set; }
    public Collider2D Collider { get; private set; }
    private FlockTowardsPoint flock;

    [HideInInspector]
    public BallPhysicsBody ball;

    // Where the enemy will move when they have grabbed the ball
    public Vector2 shootPosition;
    private Vector2 spawn;

    // Used when the enemy is grabbing the ball while moving.
    private float grabbedBallOffset = 1.2f;

    private bool activated = false;

    public void Activate()
    {
        activated = true;
    }

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        spawn = transform.position;
        TryGetComponent<FlockTowardsPoint>(out flock);
    }

    void Update()
    {
        if (!ball) return;
        if (!activated) return;

        if (currentState != EnemyState.HAS_BALL)
        {
            if (ball.VelocityLenSq <= enemyData.behaviour.minBallSpeed && !ball.Frozen) currentState = EnemyState.RUN_TOWARDS;
            if (ball.VelocityLenSq > enemyData.behaviour.minBallSpeed && !ball.Frozen) currentState = EnemyState.RUN_AWAY;
        }

        switch (currentState)
        {
            case EnemyState.RUN_AWAY:
                flock.maxSpeed = enemyData.retreatSpeed;
                flock.goalTransform = FindEscapePoint();
                flock.Move();
                break;
            case EnemyState.RUN_TOWARDS:
                flock.maxSpeed = enemyData.chaseSpeed;
                flock.goalTransform = ball.transform.position;
                flock.Move();
                CatchBall();
                break;
            case EnemyState.RUN_SPAWN:
                flock.maxSpeed = enemyData.retreatSpeed;
                flock.goalTransform = spawn;
                flock.Move();
                break;
            case EnemyState.HAS_BALL:
                flock.maxSpeed = enemyData.retreatSpeed;
                HasBall();
                SetBallPosition();
                break;

        }
    }

    void CatchBall()
    {
        float dist = Vector2.Distance(ball.transform.position, transform.position);

        const float CatchDistance = 3f;
        if (dist <= CatchDistance && !ball.Frozen && !ball.IsAirborne)
        {
            EnterHasBallState();
            GameManager.Instance.CatchedOrReleasedBall(released: false, this);
        }
    }

    void HasBall()
    {
        flock.goalTransform = shootPosition;
        flock.MoveIndependently();

        if (math.distance(flock.goalTransform, new Vector2(transform.position.x, transform.position.y)) < 0.3f)
        {
            ball.SetFrozen(false);

            float3 dir = math.normalize(new float3(flock.goalTransform.x, flock.goalTransform.y, 0.05f) - new float3(transform.position.x, transform.position.y, 0));
            ball.ApplyForce(dir * 25);
            GameManager.Instance.EnemyShootBall();

            ExitHasBallState();
        }
    }
    public void EnterHasBallState()
    {
        currentState = EnemyState.HAS_BALL;
        Rigidbody.simulated = false;
        Collider.enabled = false;
    }

    public void ExitHasBallState()
    {
        currentState = EnemyState.RUN_SPAWN;

        Timers.SetTimeout(1000, () =>
        {
            Rigidbody.velocity = Vector2.zero;
            Rigidbody.simulated = true;
            Collider.enabled = true;
        });
    }

    void SetBallPosition()
    {
        var dir = flock.velocity.normalized * grabbedBallOffset;
        ball.transform.position = transform.position + new Vector3(dir.x, dir.y, 0);
    }

    Vector2[] SuitableEscapePoints()
    {
        Vector2 directionToGoal = ball.transform.position - transform.position;
        Vector2 directionFromGoal = new Vector2(-directionToGoal.x, -directionToGoal.y);
        var startAngle = Mathf.Atan2(directionFromGoal.y, directionFromGoal.x) * Mathf.Rad2Deg;
        int numberOfRays = 12;
        Vector2[] points = new Vector2[numberOfRays];
        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = ((i / (float)numberOfRays) * 360 + startAngle) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            RaycastHit2D[] hits = Physics2D.RaycastAll(ball.transform.position, direction, 100);
            foreach (RaycastHit2D hit in hits.OrderBy(hit => hit.distance))
            {
                if (hit.collider.tag == "Ground")
                {
                    points[i] = hit.point;
                    break;
                }
            }
        }
        return points;
    }

    Vector2 FindEscapePoint()
    {
        Vector2 directionToGoal = ball.transform.position - transform.position;
        Vector2 directionFromGoal = new Vector2(-directionToGoal.x, -directionToGoal.y);
        Vector2[] points = SuitableEscapePoints();

        float distanceDefaultPointToFollow = math.distance(points[0], new Vector2(transform.position.x, transform.position.y));
        if (directionToGoal.magnitude < enemyData.behaviour.distanceToSelf && distanceDefaultPointToFollow < enemyData.behaviour.distanceToPoint)
        {
            Vector2 bestPoint = points[0];
            float bestValue = (new Vector2(ball.transform.position.x, ball.transform.position.y) - bestPoint).magnitude;
            for (int i = 1; i < points.Length; i++)
            {
                var value = (new Vector2(ball.transform.position.x, ball.transform.position.y) - points[i]).magnitude;
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


}
