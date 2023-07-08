using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour
{

    public enum HumanState
    {
        RUN_AWAY,
        RUN_TOWARDS,
        HAS_BALL,
        SHOOTING,
    }

    public HumanData humanData;

    [SerializeField]
    public HumanState currentState;
    public Flocking flocking;
    public Transform ballTransform;

    void Start()
    {

    }

    void Update()
    {
        switch (currentState)
        {
            case HumanState.RUN_AWAY:
                flocking.goalTransform = FindEscapePoint();
                flocking.Move();
                break;
            case HumanState.RUN_TOWARDS:
                flocking.goalTransform = ballTransform.position;
                flocking.Move();
                break;
            case HumanState.HAS_BALL:
                HasBall();
                break;
            case HumanState.SHOOTING:
                break;
        }
    }

    Vector2 FindEscapePoint()
    {
        Vector2 directionToGoal = new Vector2(ballTransform.position.x, ballTransform.position.y) - new Vector2(transform.position.x, transform.position.y);
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
                break;  // Exit the loop as we found a hit with the correct tag
            }
        }
        return new Vector2(0, 0);
    }

    void HasBall()
    {

    }
}
