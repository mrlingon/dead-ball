using System;
using System.Collections;
using System.Collections.Generic;
using ElRaccoone.Timers;
using UnityEngine;
using UnityEngine.Events;

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

    public bool startLevelOnImmediately = true;
    public int startLevel = 1;
    public int currentLevel { get; private set; } = 0;

    public int levelTransitionTime = 2500;

    public event Action<Level, int> LevelSetUp;
    public event Action<Level, int> LevelStart;
    public event Action<Level, int> LevelCompleted;

    void Awake()
    {
        if (GameManager.Instance?.LevelManager != null)
        {
            Destroy(gameObject);
            return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.LevelManager = this;

    }

    void Start()
    {
        GameManager.Instance.Player.ToggleControl(false);
        currentLevel = startLevel - 1;
        enemyManager.AllEnemiesDead += NextLevel;
        if (startLevelOnImmediately) NextLevel();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.N)) NextLevel();
#endif
    }

    void NextLevel()
    {
        if (currentLevel >= 0) LevelCompleted?.Invoke(levels[currentLevel], currentLevel);
        currentLevel++;
        if (currentLevel >= levels.Length)
            GameManager.Instance?.GameWin();
        else
        {
            Timers.SetTimeout(1000, () =>
            {
                NewLevel(levels[currentLevel]);
            });
        }
    }

    void NewLevel(Level level)
    {
        Reset();

        LevelSetUp?.Invoke(levels[currentLevel], currentLevel);
        SetUpLevel(level);

        Timers.SetTimeout(levelTransitionTime, () =>
        {
            LevelStart?.Invoke(levels[currentLevel], currentLevel);
            StartLevel(levels[currentLevel]);
        });

    }

    private void Reset()
    {
        enemyManager.Reset();
    }


    private void SetUpLevel(Level level)
    {
        ball.SetFrozen(true);
        enemyInstantiator.SetUpLevel(level);
        ball.transform.position = Vector3.zero;
        ballCameraController.ZoomTo(3f, 1f);
    }

    private void StartLevel(Level level)
    {
        GameManager.Instance.Player.ToggleControl(true);
        ball.SetFrozen(false);
        ballCameraController.ZoomTo(ballCameraController.DefaultStartZoom, 0.5f);
        enemyManager.ActivateEnemies();
    }
}
