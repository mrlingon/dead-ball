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

    public int currentLevel { get; private set; } = 0;

    public int levelTransitionTime = 2500;

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
        ball.SetFrozen(true);
        ball.transform.position = Vector3.zero;
        ballCameraController.ZoomTo(5f, 0.1f);
        enemyInstantiator.SetUpLevel(level);

        Timers.SetTimeout(levelTransitionTime, () =>
        {
            LevelStarted?.Invoke(levels[currentLevel], currentLevel);
            ball.SetFrozen(false);
            ballCameraController.ZoomTo(ballCameraController.DefaultStartZoom, 0.5f);
            enemyManager.ActivateEnemies();
        });

    }
}
