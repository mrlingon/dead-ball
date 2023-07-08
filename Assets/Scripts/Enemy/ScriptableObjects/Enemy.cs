using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/Enemy", order = 1)]
public class Enemy : ScriptableObject
{
    public string enemyName;
    public float size = 1f; // Speed when chasing the ball
    public float health = 10f;
    public float strength = 10f;

    public float chaseSpeed = 5f; // Speed when chasing the ball
    public float retreatSpeed = 10f; // Speed when moving away from the ball

    public EnemyBehaviour behaviour;
}