using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Collider2D))]
public class PhysicsEvents2D : MonoBehaviour
{
    public event Action<Collision2D> CollisionEnter;
    public event Action<Collision2D> CollisionExit;

    public event Action<Collider2D> TriggerEnter;
    public event Action<Collider2D> TriggerExit;

    public Collider2D Collider { get; private set; }

    protected void Awake()
    {
        Collider = GetComponent<Collider2D>();
    }

    protected void OnTriggerEnter2D(Collider2D other) => TriggerEnter?.Invoke(other);
    protected void OnTriggerExit2D(Collider2D other) => TriggerExit?.Invoke(other);

    protected void OnCollisionEnter2D(Collision2D other) => CollisionEnter?.Invoke(other);
    protected void OnCollisionExit2D(Collision2D other) => CollisionExit?.Invoke(other);

}
