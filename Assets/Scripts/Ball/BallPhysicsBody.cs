using System;
using System.Collections;
using System.Collections.Generic;
using ElRaccoone.Timers;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody2D))]
public class BallPhysicsBody : MonoBehaviour
{
    [Header("Settings")]

    // Height of the ball
    [Range(0.0f, 5.0f)]
    public float Height = 0.5f;

    // Height of the ball
    [Range(0.0f, 5.0f)]
    public float PlayerHeight = 1.0f;

    [Range(0.0f, 9.8f)]
    public float Gravity = 9.8f;

    [Range(0.0f, 50f)]
    public float SpinSpeed = 20.0f;
    public float SpinHeightMultiplier = 0.5f;


    [Range(0.01f, 1.0f)]
    public float ShadowScale = 0.15f;

    [Header("Collision")]

    [Range(0.0f, 500.0f)]
    public float RequiredCollisionVelocity = 250.0f;

    [Range(0.1f, 0.9f)]
    public float CollisionPenalty = 0.35f;


    [Header("Refs")]

    public Transform ModelPivot;

    public Transform ModelParent;

    public Transform ShadowPivot;

    public RunParticlesOnce BloodParticles;

    [Header("Debug")]
    public bool Debugging = true;

    public Rigidbody2D Rigidbody { get; private set; }

    public Collider2D Collider { get; private set; }

    private PhysicsEvents2D PhysicsEvents;

    private BloodTrail BloodTrail;

    public float HeightForce { get; private set; } = 0.0f;

    // Returns the velocity of the ball.
    public float3 Velocity => new float3(Rigidbody.velocity.x, Rigidbody.velocity.y, HeightForce);

    // Returns the sqr lenght of the velocity.
    public float VelocityLenSq => math.lengthsq(Velocity);

    public bool IsAirborne => !IsGrounded();

    public event Action<GameObject, bool> HitEnemy;

    public event Action<GameObject> KilledEnemy;

    public bool Frozen { get; private set; } = false;

    public int grabCooldown = 1000; // Frans när du ser detta säg till mig
    public bool canBeGrabbed = true; // Frans när du ser detta säg till mig

    public float3 originalScale;

    public bool IsGrounded()
    {
        const float GroundedDistance = 0.0001f;
        return Height <= GroundedDistance;
    }

    public void ApplyForce(float3 force)
    {
        //if (!IsGrounded())
        //{
        // Debug.LogWarning("Cannot apply force when airborne");
        // return;
        //}

        // We need to apply the force to the rigidbody
        Rigidbody.AddForce(new Vector2(force.x, force.y), ForceMode2D.Impulse);

        // We need to apply the force to the height
        HeightForce += force.z;
    }

    public void SetFrozen(bool frozen)
    {
        Frozen = frozen;
        Height = 0;
        HeightForce = 0f;
        Rigidbody.simulated = !frozen;
        Rigidbody.velocity = Vector2.zero;
    }

    protected void Awake()
    {
        if (GameManager.Instance?.Ball != null)
        {
            Destroy(gameObject);
            return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.Ball = this;
        originalScale = transform.localScale;
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();

        TryGetComponent(out PhysicsEvents);
        TryGetComponent(out BloodTrail);

        PhysicsEvents.CollisionEnter += (collision) =>
        {
            if (Frozen) return;

            if (collision.gameObject.CompareTag("Ground"))
            {
                HeightForce = 0;
            }
        };

        PhysicsEvents.TriggerEnter += (collision) =>
        {
            if (collision.gameObject.CompareTag("Goal"))
            {
                Timers.SetTimeout(250, () =>
                {
                    GameManager.Instance?.GameOver();
                    SetFrozen(true);
                });
            }

            if (Frozen) return;

            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (VelocityLenSq >= RequiredCollisionVelocity)
                {
                    if (Debugging)
                    {
                        DebugDraw.Circle(transform.position, 0.5f, Color.red, 3.0f);
                    }


                    if (collision.gameObject.TryGetComponent(out EnemyController enemy))
                    {
                        if (enemy.isDying) return;
                    }

                    HitEnemy?.Invoke(collision.gameObject, true);

                    Rigidbody.velocity *= CollisionPenalty;
                    HeightForce *= CollisionPenalty;

                    GameManager.Instance.GameField?.SplatterPaint(new float2(transform.position.x, transform.position.y), 1.2f, 8, 15, 2, 6);
                    BloodTrail.ActivateTrail(1f, 2f);
                    Instantiate(BloodParticles, collision.transform.parent.position, Quaternion.identity);
                    KilledEnemy?.Invoke(collision.transform.parent.gameObject);
                }
                else
                {
                    HitEnemy?.Invoke(collision.gameObject, false);
                }
            }
        };
    }

    protected void Start()
    {

    }

    private bool requestedZoom = false;
    protected void Update()
    {
        if (Debugging)
        {
            DebugDraw.Line(transform.position, transform.position + new Vector3(Velocity.x, Velocity.y, 0), Color.red);
            DebugDraw.Line(transform.position, transform.position + new Vector3(0, Velocity.z, 0), Color.blue);
            DebugDraw.Line(transform.position, transform.position + new Vector3(0, Height, 0), Color.yellow);
        }

        // zooming meme
        if (VelocityLenSq >= RequiredCollisionVelocity && !requestedZoom)
        {
            requestedZoom = true;
            GameManager.Instance?.BallCamera?.ZoomTo(GameManager.Instance.BallCamera.DefaultStartZoom * 0.8f, 0.333f);
            LeanTween.scaleY(gameObject, originalScale.y * 0.8f, 0.333f).setEase(LeanTweenType.easeInOutSine);
        }
        else if (requestedZoom && GameManager.Instance?.BallCamera != null && GameManager.Instance?.BallCamera?.VirtualCamera.m_Lens.OrthographicSize != GameManager.Instance?.BallCamera?.DefaultStartZoom)
        {
            LeanTween.scaleY(gameObject, originalScale.y, 0.333f).setEase(LeanTweenType.easeInOutSine);
            GameManager.Instance?.BallCamera?.ZoomTo(GameManager.Instance.BallCamera.DefaultStartZoom, 0.666f);
            requestedZoom = false;
        }
    }

    protected void FixedUpdate()
    {
        if (!Frozen)
        {
            DisableCollisionWhenAirborne();

            if (HeightForce != 0 || !IsGrounded())
            {
                ApplyHeightForce();
            }

            ApplyRotation();
        }

        ApplyModelHeight();
        ApplyShadowScale();
    }

    protected void ApplyRotation()
    {
        if (ModelParent != null)
        {
            var velocity = new float3(Rigidbody.velocity.x, Rigidbody.velocity.y, HeightForce * SpinHeightMultiplier);
            var forward = math.normalizesafe(velocity);
            ModelParent.Rotate(forward * Time.deltaTime * SpinSpeed * Rigidbody.velocity.magnitude);
        }
    }

    protected void ApplyHeightForce()
    {
        float acceleration = (HeightForce - Gravity) * Time.deltaTime;
        // Calculate the change in position (displacement) using the formula: displacement = initialVelocity * deltaTime + 0.5 * acceleration * deltaTime^2
        float displacement = HeightForce * Time.deltaTime + 0.5f * acceleration * Time.deltaTime * Time.deltaTime;

        Height += displacement;

        // Decay the force due to gravity
        HeightForce -= Gravity * Time.deltaTime;

        if (IsGrounded())
        {
            Height = 0.0f;
            HeightForce = 0.0f;
        }
    }

    protected void DisableCollisionWhenAirborne()
    {
        // Disable collision with a specific layer
        if (IsAirborne && Height > PlayerHeight)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
        }
    }

    protected void ApplyModelHeight()
    {
        if (ModelPivot != null)
            ModelPivot.localPosition = new Vector3(ModelPivot.localPosition.x, Height, ModelPivot.localPosition.z);
    }

    protected void ApplyShadowScale()
    {
        const float MaxShadowScale = 5.0f;
        if (ShadowPivot != null)
            ShadowPivot.localScale = new Vector3(1, 1, 1) * math.min((1 + Height * ShadowScale), MaxShadowScale);
    }
}
