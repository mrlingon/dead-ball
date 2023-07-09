using System;
using System.Linq;
using ElRaccoone.Timers;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Ball Kick Settings")]
    public float KickXYForceMultipler = 0.05f;
    public float KickZForceMultipler = 0.01f;
    public float LobXYForceMultipler = 0.005f;
    public float LobZForceMultipler = 0.1f;

    [Header("Ball Release Settings")]
    public float ReleaseKeyPower = 0.15f;

    [SerializeField]
    [ReadOnly]
    private float ReleasePowerLeft = 1.0f;

    [Header("Refs")]
    public InputActionAsset InputActions;
    public BallPhysicsBody Ball;
    public PlayerHoldDrag PlayerHoldDrag;
    public DragPower DragPower;
    public GameObject TrackingIndicatorPrefab;

    public bool CanControl { get; set; } = true;

    private InputAction KickAction;
    private InputAction LobAction;
    private InputAction ReleaseAction;

    public event Action<bool> ShowingTrailParticles;

    protected void Awake()
    {
        if (GameManager.Instance.Player != null)
        {
            Destroy(gameObject);
            return;
        }

        gameObject.transform.SetParent(null);
        GameManager.Instance.Player = this;

        KickAction = InputActions.FindAction("Gameplay/Kick");
        LobAction = InputActions.FindAction("Gameplay/Lob");
        ReleaseAction = InputActions.FindAction("Gameplay/Release");

        ToggleControl(true);
    }

    protected void Start()
    {
        DragPower ??= GetComponent<DragPower>();
        PlayerHoldDrag ??= GetComponent<PlayerHoldDrag>();

        PlayerHoldDrag.StartDrag += (mode) =>
        {
        };

        PlayerHoldDrag.Released += (mode, drag) =>
        {
            float xyMult = mode == PlayerHoldDrag.PlayerKickMode.Kick ? KickXYForceMultipler : LobXYForceMultipler;
            float zMult = mode == PlayerHoldDrag.PlayerKickMode.Kick ? KickZForceMultipler : LobZForceMultipler;

            float2 forceXY = drag * xyMult;
            float forceZ = math.length(forceXY) * zMult;

            float3 force = new float3(forceXY.x, forceXY.y, forceZ);

            Ball.ApplyForce(force);
        };

        Ball.HitEnemy += (enemy, hit) =>
        {
            if (hit)
            {
                GameManager.Instance.BallCamera?.Shake(0.012f, 1f, 0.666f);
                GameManager.Instance.Scores.AddScore(1);
                GameManager.Instance.Scores.AddKill();
            }
            else
            {
                GameManager.Instance.BallCamera?.Shake(0.008f, 1f, 0.666f);
            }
        };


        GameManager.Instance.LevelManager.LevelStart += (level, level_rank) =>
        {
            foreach (var enemy in GameManager.Instance.LevelManager.enemyManager.enemies)
            {
                var go = Instantiate(TrackingIndicatorPrefab, Ball.transform);
                go.GetComponent<TargetIndicator>().SetTarget(enemy.transform);
            }
        };

        GameManager.Instance.OnCatchedBall += () =>
        {
            ReleaseAction.Enable();
            ReleasePowerLeft = 1.0f;
            PlayerHoldDrag.Reset();
        };

        GameManager.Instance.OnReleasedBall += () =>
        {
            ReleaseAction.Disable();
            PlayerHoldDrag.Reset();
            ReleasePowerLeft = 1.0f;
        };

        GameManager.Instance.OnEnemyShootBall += () =>
        {
            ReleaseAction.Disable();
            PlayerHoldDrag.Reset();
            ReleasePowerLeft = 1.0f;
            ToggleControl(false);
        };

        GameManager.Instance.LevelManager.LevelCompleted += (level, level_rank) =>
        {
            SetInitialRotation(0.333f);
        };

        GameManager.Instance.LevelManager.LevelSetUp += (level, level_rank) =>
        {
            SetInitialRotation(0f);
            Timers.SetTimeout(400, () =>
            {
                LookAtCameraRotationAnimation(2f);
            });
        };

        GameManager.Instance.LevelManager.LevelStart += (level, level_rank) =>
        {
            ToggleControl(true);
        };

        GameManager.Instance.OnGameOver += () =>
        {
            ToggleControl(false);
            PlayerHoldDrag.Reset();
        };

        GameManager.Instance.OnGameWin += () =>
        {
            ToggleControl(false);
            PlayerHoldDrag.Reset();
        };
    }

    private bool showingTrailParticles = false;
    protected void Update()
    {
        if (!ReleaseAction.enabled && GameManager.Instance.BallIsCatched)
        {
            ReleaseAction.Enable();
        }
        else if (ReleaseAction.enabled && !GameManager.Instance.BallIsCatched)
        {
            ReleaseAction.Disable();
        }

        if (GameManager.Instance.BallIsCatched && ReleaseAction.WasPressedThisFrame())
        {
            ReleasePowerLeft -= ReleaseKeyPower;
            GameManager.Instance.BallCamera?.Shake(0.005f, 1f, 0.666f);

            if (ReleasePowerLeft <= 0.0f)
            {
                GameManager.Instance.CatchedOrReleasedBall(true);
                GameManager.Instance.BallCamera?.Shake(0.008f, 1f, 0.666f);
            }
        }

        if (Ball.VelocityLenSq >= Ball.RequiredCollisionVelocity && !showingTrailParticles)
        {
            showingTrailParticles = true;
            GameManager.Instance.BallCamera?.ShowTrailParticles();
            ShowingTrailParticles?.Invoke(true);
        }
        else if (showingTrailParticles)
        {
            showingTrailParticles = false;
            GameManager.Instance.BallCamera?.HideTrailParticles();
            ShowingTrailParticles?.Invoke(false);
        }
    }

    public void SetInitialRotation(float time = 0.333f)
    {
        LeanTween.rotate(Ball.ModelParent.transform.gameObject, new Vector3(0, 90, -180), time).setEase(LeanTweenType.easeOutCubic);
    }

    public void LookAtCameraRotationAnimation(float time = 0.333f)
    {
        GameManager.Instance.BallCamera?.ShowTrailParticles();
        GameManager.Instance.BallCamera?.Shake(0.016f, 1f, time + 0.333f);
        LeanTween.rotateZ(Ball.ModelParent.transform.gameObject, 26, time).setEase(LeanTweenType.easeOutCubic).setOnComplete(() =>
        {
            GameManager.Instance.BallCamera?.HideTrailParticles();
        });
    }

    public float GetReleaseProgress()
    {
        return ReleasePowerLeft;
    }

    public void ToggleControl(bool canControl)
    {
        CanControl = canControl;
        if (CanControl)
        {
            KickAction.Enable();
            LobAction.Enable();
        }
        else
        {
            KickAction.Disable();
            LobAction.Disable();
        }
    }
}
