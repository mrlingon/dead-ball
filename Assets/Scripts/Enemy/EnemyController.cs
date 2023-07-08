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
        RUN_SPAWN,
        HAS_BALL,
    }

    public EnemyData humanData;
    public EnemyState currentState = EnemyState.RUN_TOWARDS;

    private FlockTowardsPoint flock;

    [HideInInspector]
    public BallPhysicsBody ball;

    [Range(0, 30)]
    public float distanceToPoint = 3.5f;

    [Range(0, 30)]
    public float distanceToSelf = 9.5f;

    [Range(0, 30)]
    public float ballPositionLength = 1.2f;

    public float ballSpeedCutoff = 20f;

    public Vector2 kickoffPoint;
    private Vector2 spawn;

    public Rigidbody2D Rigidbody { get; private set; }
    public Collider2D Collider { get; private set; }

    public event Action<EnemyController> OnCatchBall;

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

        if (currentState != EnemyState.HAS_BALL)
        {
            if (ball.VelocityLenSq <= ballSpeedCutoff && !ball.Frozen) currentState = EnemyState.RUN_TOWARDS;
            if (ball.VelocityLenSq > ballSpeedCutoff && !ball.Frozen) currentState = EnemyState.RUN_AWAY;
        }

        switch (currentState)
        {
            case EnemyState.RUN_AWAY:
                flock.goalTransform = FindEscapePoint();
                flock.Move();
                break;
            case EnemyState.RUN_TOWARDS:
                flock.goalTransform = ball.transform.position;
                flock.Move();
                CatchBall();
                break;
            case EnemyState.RUN_SPAWN:
                flock.goalTransform = spawn;
                flock.Move();
                break;
            case EnemyState.HAS_BALL:
                HasBall();
                SetBallPosition();
                break;

        }
    }

    void CatchBall()
    {
        float dist = Vector2.Distance(ball.transform.position, transform.position);

        if (dist <= 3.0 && !ball.Frozen)
        {
            ball.SetFrozen(true);
            currentState = EnemyState.HAS_BALL;
            Rigidbody.simulated = false;
            Collider.enabled = false;
        }
    }

    void HasBall()
    {
        flock.goalTransform = kickoffPoint;
        flock.MoveIndependently();

        if (math.distance(flock.goalTransform, new Vector2(transform.position.x, transform.position.y)) < 0.3f)
        {

            ball.SetFrozen(false);
            Rigidbody.velocity = Vector2.zero;
            GameManager.Instance.Player.CanControl = false;
            ball.ApplyForce(new float3(-20, 0, 0));

            currentState = EnemyState.RUN_SPAWN;
        }
    }

    void SetBallPosition()
    {
        var dir = flock.velocity.normalized * ballPositionLength;
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
        if (directionToGoal.magnitude < distanceToSelf && distanceDefaultPointToFollow < distanceToPoint)
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
