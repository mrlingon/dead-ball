using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ball Settings")]
    [Range(0.01f, 0.5f)]
    public float DragForceMultiplier = 0.05f;
    [Range(0.001f, 0.05f)]
    public float DragForceHeightMultiplier = 0.015f;

    [Header("Refs")]
    public BallPhysicsBody Ball;
    public PlayerHoldDrag PlayerHoldDrag;
    public DragPower DragPower;
    public bool CanControl { get; private set; }

    protected void Awake()
    {
        if (GameManager.Instance.Player != null)
        {
            Destroy(gameObject);
            return;
        }

        gameObject.transform.SetParent(null);
        GameManager.Instance.Player = this;

        DontDestroyOnLoad(gameObject);
    }

    protected void Start()
    {
        DragPower ??= GetComponent<DragPower>();
        PlayerHoldDrag ??= GetComponent<PlayerHoldDrag>();

        PlayerHoldDrag.StartDrag += () =>
        {
        };

        PlayerHoldDrag.Released += (drag) => {
            float2 forceXY = drag * DragForceMultiplier;
            float forceZ = math.lengthsq(forceXY) * DragForceHeightMultiplier;
            float3 force = new float3(forceXY.x, forceXY.y, forceZ);

            Ball.ApplyForce(force);
        };
    }

    protected void FixedUpdate()
    {

    }

    public void ToggleControl(bool canControl)
    {
        CanControl = canControl;
    }
}
