using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [Button("Apply Force Test")]
    public void ApplyForceTest()
    {
        ApplyForce(new float3(2, 2, 4));
    }

    [Header("Movement")]

    // Height of the ball
    [Range(0.0f, 5.0f)]
    public float Height = 0.5f;

    [Range(0.0f, 9.8f)]
    public float Gravity = 9.8f;

    [Header("Refs")]

    public Transform ModelPivot;

    public Transform ShadowPivot;

    public Rigidbody2D Rigidbody { get; private set; }

    public Collider2D Collider { get; private set; }

    public float HeightForce { get; private set; } = 0.0f;

    public bool IsAirborne => !IsGrounded();

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
    }

    protected void Start()
    {
    }

    protected void Update()
    {
        LockZAxis();
        DisableCollisionWhenAirborne();

        if (HeightForce != 0 || !IsGrounded())
            ApplyHeightForce();

        ApplyModelHeight();
        ApplyShadowScale();
    }

    protected void LockZAxis()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
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
        Collider.enabled = IsGrounded();
    }

    protected void ApplyModelHeight()
    {
        if (ModelPivot != null)
            ModelPivot.localPosition = new Vector3(ModelPivot.localPosition.x, Height, ModelPivot.localPosition.z);
    }

    protected void ApplyShadowScale()
    {
        const float ShadowScale = 0.25f;
        if (ShadowPivot != null)
            ShadowPivot.localScale = new Vector3(1, 1, 1) * (1 + Height * ShadowScale);
    }
}
