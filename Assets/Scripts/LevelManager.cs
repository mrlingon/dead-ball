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

    public int startLevel = 1;
    public int currentLevel { get; private set; } = 0;

    public int levelTransitionTime = 2500;

    public event Action<Level, int> LevelStarted;
    public event Action<Level, int> LevelCompleted;
    public event Action<Level, int> AllLevelsComplete;

    void Start()
    {
        currentLevel = startLevel - 1;
        Timers.SetTimeout(1000, () => StartLevel(levels[currentLevel]));
        enemyManager.AllEnemiesDead += NextLevel;
    }

    void NextLevel()
    {
        LevelCompleted?.Invoke(levels[currentLevel], currentLevel);
        currentLevel++;
        if (currentLevel >= levels.Length)
            GameManager.Instance?.GameWin();
        else
            StartLevel(levels[currentLevel]);
    }

    // Update is called once per frame
    void StartLevel(Level level)
    {
        if (currentLevel == 0)
        {
            PrepareLevel(level);
        }
        else
        {
            Timers.SetTimeout(1000, () =>
            {
                PrepareLevel(level);
            });
        }

        Timers.SetTimeout(levelTransitionTime, () =>
        {
            LevelStarted?.Invoke(levels[currentLevel], currentLevel);
            ball.SetFrozen(false);
            ballCameraController.ZoomTo(ballCameraController.DefaultStartZoom, 0.5f);
        });

        Timers.SetTimeout(levelTransitionTime + 1000, () =>
        {
            enemyManager.ActivateEnemies();
        });
    }

    private void PrepareLevel(Level level)
    {
        ball.SetFrozen(true);
        enemyInstantiator.SetUpLevel(level);
        ball.transform.position = Vector3.zero;
        ballCameraController.ZoomTo(3f, 1f);
    }
}
