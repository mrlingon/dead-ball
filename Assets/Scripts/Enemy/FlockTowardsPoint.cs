using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlockTowardsPoint : MonoBehaviour
{
    public Vector3 baseRotation;

    [Range(0, 10)]
    public float maxSpeed = 1f;

    [Range(.1f, .5f)]
    public float maxForce = .03f;

    [Range(1, 50)]
    public float neighborhoodRadius = 3f;

    [Range(0, 30)]
    public float separationAmount = 1f;

    [Range(0, 30)]
    public float cohesionAmount = 1f;

    [Range(0, 30)]
    public float alignmentAmount = 1f;

    [Range(0, 30)]
    public float goalAmount = 1f;

    public Vector2 goalTransform;

    public Vector2 acceleration;
    public Vector2 velocity;

    private Vector2 Position
    {
        get
        {
            return gameObject.transform.position;
        }
        set
        {
            gameObject.transform.position = new Vector3(value.x, value.y, gameObject.transform.position.z);
        }
    }

    private void Start()
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        // transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public void Move()
    {
        var boidColliders = Physics2D.OverlapCircleAll(Position, neighborhoodRadius);
        var boids = boidColliders.Select(o => o.GetComponent<FlockTowardsPoint>()).ToList();
        boids.Remove(this);

        Flock(boids);
        UpdateVelocity();
        UpdatePosition();
        UpdateRotation();

        DebugDraw.Circle(Position, neighborhoodRadius, Color.cyan);
        DebugDraw.Arrow(Position, velocity, Color.red, 1f, 0.25f, 0.5f);
    }

    /* Move towards the goal without flocking behaviour */
    public void MoveIndependently()
    {
        var goal = Goal();
        acceleration = goal * goalAmount;
        UpdateVelocity();
        UpdatePosition();
        UpdateRotation();
        DebugDraw.Circle(Position, neighborhoodRadius, Color.cyan);
        DebugDraw.Arrow(Position, velocity, Color.red, 1f, 0.25f, 0.5f);
    }

    private void Flock(IEnumerable<FlockTowardsPoint> boids)
    {
        var goal = Goal();
        var alignment = Alignment(boids);
        var separation = Separation(boids);
        var cohesion = Cohesion(boids);

        acceleration = goal * goalAmount + alignmentAmount * alignment + cohesionAmount * cohesion + separationAmount * separation;
    }

    public void UpdateVelocity()
    {
        velocity += acceleration;
        velocity = LimitMagnitude(velocity, maxSpeed);
    }

    private void UpdatePosition()
    {
        Position += velocity * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
    }

    private Vector2 Goal()
    {
        Vector2 directionToGoal = goalTransform - new Vector2(transform.position.x, transform.position.y);

        var steer = Steer(directionToGoal.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Alignment(IEnumerable<FlockTowardsPoint> boids)
    {
        var velocity = Vector2.zero;
        if (!boids.Any()) return velocity;

        foreach (var boid in boids)
        {
            if (boid)
                velocity += boid.velocity;
        }
        velocity /= boids.Count();

        var steer = Steer(velocity.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Cohesion(IEnumerable<FlockTowardsPoint> boids)
    {
        if (!boids.Any()) return Vector2.zero;

        var sumPositions = Vector2.zero;
        foreach (var boid in boids)
        {
            if (boid)
                sumPositions += boid.Position;
        }
        var average = sumPositions / boids.Count();
        var direction = average - Position;

        var steer = Steer(direction.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Separation(IEnumerable<FlockTowardsPoint> boids)
    {
        var direction = Vector2.zero;
        boids = boids.Where(o =>
        {
            if (o) return DistanceTo(o) <= neighborhoodRadius / 2;
            else return false;
        }
    );
        if (!boids.Any()) return direction;

        foreach (var boid in boids)
        {
            var difference = Position - boid.Position;
            direction += difference.normalized / difference.magnitude;
        }
        direction /= boids.Count();

        var steer = Steer(direction.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Steer(Vector2 desired)
    {
        var steer = desired - velocity;
        steer = LimitMagnitude(steer, maxForce);

        return steer;
    }

    private float DistanceTo(FlockTowardsPoint boid)
    {

        return Vector3.Distance(boid.transform.position, Position);
    }

    private Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
    {
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }
        return baseVector;
    }
}
