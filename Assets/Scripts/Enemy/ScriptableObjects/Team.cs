using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "Enemy/Team", order = 1)]
public class Team : ScriptableObject
{
    public Color color;
    public string teamName;
    public Enemy[] enemies;
}