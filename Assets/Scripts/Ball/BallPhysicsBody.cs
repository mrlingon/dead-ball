using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Refs")]

    public Transform ModelPivot;

    public Transform ModelParent;

    public Transform ShadowPivot;

    [Header("Debug")]
    public bool Debugging = true;

    public Rigidbody2D Rigidbody { get; private set; }

    public Collider2D Collider { get; private set; }

    private PhysicsEvents2D PhysicsEvents;

    public float HeightForce { get; private set; } = 0.0f;

    // Returns the velocity of the ball.
    public float3 Velocity => new float3(Rigidbody.velocity.x, Rigidbody.velocity.y, HeightForce);

    // Returns the sqr lenght of the velocity.
    public float VelocityLenSq => math.lengthsq(Velocity);

    public bool IsAirborne => !IsGrounded();

    public event Action<GameObject> HitEnemy;

    public bool IsGrounded()
    {
        const float GroundedDistance = 0.0001f;
        return Height <= GroundedDistance;
    }

    public void ApplyForce(float3 force)
    {
        if (!IsGrounded())
        {
            Debug.LogWarning("Cannot apply force when airborne");
            return;
        }

        // We need to apply the force to the rigidbody
        Rigidbody.AddForce(new Vector2(force.x, force.y), ForceMode2D.Impulse);

        // We need to apply the force to the height
        HeightForce += force.z;
    }

    protected void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();

        TryGetComponent(out PhysicsEvents);

        PhysicsEvents.CollisionEnter += (collision) =>
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                HeightForce = 0;
            }
        };

        PhysicsEvents.TriggerEnter += (collision) =>
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (Debugging)
                {
                    DebugDraw.Circle(transform.position, 0.5f, Color.red, 3.0f);
                }

                HitEnemy?.Invoke(collision.gameObject);

                // TODO: dont do this hehe :)
                Debug.Log(VelocityLenSq);

                if (VelocityLenSq >= 300.0f)
                {
                    Destroy(collision.transform.parent.gameObject);
                }
            }
        };
    }

    protected void Start()
    {

    }

    protected void Update()
    {
        if (Debugging)
        {
            DebugDraw.Line(transform.position, transform.position + new Vector3(Velocity.x, Velocity.y, 0), Color.red);
            DebugDraw.Line(transform.position, transform.position + new Vector3(0, Velocity.z, 0), Color.blue);
            DebugDraw.Line(transform.position, transform.position + new Vector3(0, Height, 0), Color.yellow);
        }
    }

    protected void FixedUpdate()
    {
        DisableCollisionWhenAirborne();

        if (HeightForce != 0 || !IsGrounded())
            ApplyHeightForce();

        ApplyRotation();
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
