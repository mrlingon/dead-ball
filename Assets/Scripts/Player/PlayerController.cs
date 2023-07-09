using System;
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

    [SerializeField] [ReadOnly]
    private float ReleasePowerLeft = 1.0f;

    [Header("Refs")]
    public InputActionAsset InputActions;
    public BallPhysicsBody Ball;
    public PlayerHoldDrag PlayerHoldDrag;
    public DragPower DragPower;
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

        DontDestroyOnLoad(gameObject);

        ToggleControl(true);
    }

    protected void Start()
    {
        DragPower ??= GetComponent<DragPower>();
        PlayerHoldDrag ??= GetComponent<PlayerHoldDrag>();

        PlayerHoldDrag.StartDrag += (mode) =>
        {
        };

        PlayerHoldDrag.Released += (mode, drag) => {
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
                GameManager.Instance.BallCamera?.Shake(0.004f, 1f, 0.444f);
                GameManager.Instance.Scores.AddScore(1);
                GameManager.Instance.Scores.AddKill();
            }
            else
            {
                GameManager.Instance.BallCamera?.Shake(0.001f, 1f, 0.444f);
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
            if (ReleasePowerLeft <= 0.0f)
            {
                GameManager.Instance.CatchedOrReleasedBall(true);
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
