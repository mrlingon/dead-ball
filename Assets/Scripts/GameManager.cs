using System;
using System.Collections.Generic;
using Cinemachine;
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
    public GameField GameField { get; set; }

    public ScoreManager Scores { get; set; }

    private bool IsTicking;

    protected void Awake()
    {
        LeanTween.init(800);

        InnerInstance = this;
        DontDestroyOnLoad(gameObject);
        IsTicking = true;
    }

    protected void Start()
    {
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

    public void LoadScene(int targetScene)
    {
        Pause();
        Player?.ToggleControl(false);

        var asyncop = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
        asyncop.completed += operation =>
        {
            Resume();
            Player?.ToggleControl(true);
        };
    }
}
