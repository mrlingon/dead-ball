using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset InputAction;
    public float Speed = 4.0f;
    public bool CanControl { get; private set; }

    private InputAction MoveAction;
    private Rigidbody2D Rigidbody;

    protected void Awake()
    {
        if (GameManager.Instance.Player != null)
        {
            Destroy(gameObject);
            return;
        }

        Rigidbody = GetComponent<Rigidbody2D>();

        gameObject.transform.SetParent(null);

        GameManager.Instance.Player = this;
        DontDestroyOnLoad(gameObject);
    }

    protected void Start()
    {
        MoveAction = InputAction.FindAction("Gameplay/Move");
        MoveAction.Enable();
    }

    protected void FixedUpdate()
    {
        var move = MoveAction.ReadValue<Vector2>();
        var movement = move * Speed;
        // var speed = movement.sqrMagnitude;

        Rigidbody.MovePosition(Rigidbody.position + movement * Time.deltaTime);
    }

    public void ToggleControl(bool canControl)
    {
        CanControl = canControl;
        if (CanControl) {
            MoveAction.Enable();
        } else {
            MoveAction.Disable();
        }
    }
}
