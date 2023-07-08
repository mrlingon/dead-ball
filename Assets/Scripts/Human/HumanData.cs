using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HumanData", menuName = "Enemy/HumanData", order = 1)]
public class HumanData : ScriptableObject
{
    public float runawaySpeed = 5f; // Speed of the human
    public float runSpeed = 10f; // Speed of the human
}