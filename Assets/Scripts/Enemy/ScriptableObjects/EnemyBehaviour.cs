
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBehaviour", menuName = "Enemy/EnemyBehaviour", order = 1)]
public class EnemyBehaviour : ScriptableObject
{
    // How close the enemy must be to its retreat position
    // before it is thinking about choosing a new point.
    [Range(0, 30)]
    public float distanceToPoint = 3.5f;

    // How close the enemy must be to the ball
    // before it is thinking about choosing a new point.
    [Range(0, 30)]
    public float distanceToSelf = 9.5f;

    // The minimum speed of the ball before the enemy starts chasing it.
    public float minBallSpeed = 10f;
}