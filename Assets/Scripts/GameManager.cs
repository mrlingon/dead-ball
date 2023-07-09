using System;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-9999)]
public class GameManager : MonoBehaviour
{
    private static GameManager InnerInstance;

#if UNITY_EDITOR
    private static bool IsQuitting = false;
#endif

    public static GameManager Instance
    {
        get
        {
#if UNITY_EDITOR

            if (!Application.isPlaying || IsQuitting)
                return null;

            if (InnerInstance == null)
            {
                Instantiate(Resources.Load<GameManager>("GameManager"));
            }
#endif
            return InnerInstance;
        }
    }

    public PlayerController Player { get; set; }
    public BallCameraController BallCamera { get; set; }
    public BallPhysicsBody Ball { get; set; }
    public GameField GameField { get; set; }
    public ScoreManager Scores { get; set; }
    public LevelManager LevelManager { get; set; }
    public SceneLoader SceneLoader { get; set; }

    public event Action OnCatchedBall;
    public event Action OnReleasedBall;
    public event Action OnEnemyShootBall;

    public event Action OnGameOver;
    public event Action OnGameWin;

    public event Action OnGameStart;

    public bool BallIsCatched { get; private set; } = false;
    public EnemyController EnemyWithBall { get; private set; } = null;


    private bool IsTicking;

    protected void Awake()
    {
        if (InnerInstance != null && InnerInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        LeanTween.init(800);

        InnerInstance = this;
        DontDestroyOnLoad(gameObject);
        IsTicking = true;
    }

    protected void Start()
    {
        OnGameStart?.Invoke();
    }

#if UNITY_EDITOR
    protected void OnDestroy()
    {
        IsQuitting = true;
    }
#endif

    protected void Update()
    {
        if (IsTicking)
        {

        }
    }

    public void Pause()
    {
        IsTicking = false;
    }

    public void Resume()
    {
        IsTicking = true;
    }

    public void CatchedOrReleasedBall(bool released, EnemyController holder = null)
    {
        if (released)
        {
            Ball.SetFrozen(false);
            GameManager.Instance.Player.CanControl = true;
            OnReleasedBall?.Invoke();
            BallIsCatched = false;
            EnemyWithBall?.ExitHasBallState();
            EnemyWithBall = null;
            float3 force = new float3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), 10);
            Ball.ApplyForce(force);
        }
        else
        {
            GameManager.Instance.Player.CanControl = false;
            Ball.SetFrozen(true);
            OnCatchedBall?.Invoke();
            BallIsCatched = true;
            EnemyWithBall = holder;
        }
    }

    public void EnemyShootBall()
    {
        OnEnemyShootBall?.Invoke();
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
        Reset();
        GameManager.Instance.Player.CanControl = false;
        Debug.Log("Game Over. Score: " + Scores.Score + " Combo: " + Scores.Combo + ". Kills: " + Scores.Kills);
    }

    public void GameWin()
    {
        OnGameWin?.Invoke();
        Reset();
        GameManager.Instance.Player.CanControl = false;

        Debug.Log("You Win. Score: " + Scores.Score + " Combo: " + Scores.Combo + ". Kills: " + Scores.Kills);
    }


    public void Reset()
    {
        EnemyWithBall = null;
        BallIsCatched = false;
        GameManager.Instance.Player.CanControl = true;
    }
}
