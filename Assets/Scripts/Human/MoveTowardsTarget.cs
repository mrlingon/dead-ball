using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsTarget : MonoBehaviour
{


    public List<Transform> neighbors;
    public float neighbourDistance;
    public float separationWeight;

    public Rigidbody2D body;
    public Transform target;
    public float speed;

    void Start()
    {
        // Create a new list
        neighbors = new List<Transform>();

        // Find all objects of type FlockingBehavior
        MoveTowardsTarget[] allBoids = GameObject.FindObjectsOfType<MoveTowardsTarget>();

        // For each boid found
        foreach (MoveTowardsTarget boid in allBoids)
        {
            // Do not add itself to its neighbor list
            if (boid.gameObject != this.gameObject)
            {
                // Add the boid's transform to the neighbor list
                neighbors.Add(boid.transform);
            }
        }
    }

    public void Move()
    {
        // Calculate the direction towards the target point
        Vector3 direction = target.position - transform.position;

        Vector3 separation = Vector3.zero;
        int groupSize = 0;
        foreach (Transform boid in neighbors)
        {
            Vector3 diff = transform.position - boid.transform.position;
            if (diff.magnitude < neighbourDistance)
            {
                separation += diff.normalized / diff.magnitude; // stronger influence from nearby boids
                groupSize++;
            }
        }

        if (groupSize > 0)
        {
            // Separation: maintain a certain distance away from nearby boids
            separation = Vector3.ClampMagnitude(separation, separationWeight);
        }


        // Normalize the direction and move the human towards the target
        direction += separation;
        var newPos = body.transform.position + direction.normalized * speed * Time.deltaTime;
        body.MovePosition(newPos);
    }

    public void MoveAway()
    {
        // Calculate the direction towards the target point
        Vector3 direction = target.position - transform.position;
        direction = new Vector3(-direction.x, -direction.y, -direction.z);
        // Normalize the direction and move the human towards the target
        var newPos = body.transform.position + direction.normalized * speed * Time.deltaTime;
        body.MovePosition(newPos);
    }

    public void SetTarget(Transform targetPoint)
    {
        this.target = targetPoint;
    }
}
