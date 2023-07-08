using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    public float runawaySpeed = 5f; // Speed of the human
    public float runSpeed = 10f; // Speed of the human
}