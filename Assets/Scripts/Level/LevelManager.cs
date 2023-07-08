using System;
using System.Collections;
using System.Collections.Generic;
using ElRaccoone.Timers;
using UnityEngine;


[System.Serializable]
public struct Level
{
    public Team teamOne;
    public Team teamTwo;
}

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    public Level[] levels;

    public BallCameraController ballCameraController;
    public BallPhysicsBody ball;
    public EnemyInstantiator enemyInstantiator;
    public EnemyManager enemyManager;

    public int currentLevel = 0;

    public event Action<Level, int> LevelStarted;
    public event Action<Level, int> LevelCompleted;
    public event Action<Level, int> AllLevelsComplete;

    void Start()
    {
        Timers.SetTimeout(1000, () => StartLevel(levels[currentLevel]));
        enemyManager.AllEnemiesDead += NextLevel;
    }

    void NextLevel()
    {
        Debug.Log("Next Level");
        LevelCompleted?.Invoke(levels[currentLevel], currentLevel);
        currentLevel++;
        if (currentLevel > levels.Length)
        {
            AllLevelsComplete?.Invoke(levels[currentLevel], currentLevel);
        }
        else
        {
            StartLevel(levels[currentLevel]);
        }
    }

    // Update is called once per frame
    void StartLevel(Level level)
    {
        LevelStarted?.Invoke(levels[currentLevel], currentLevel);
        enemyInstantiator.SetUpLevel(level);
    }
}
