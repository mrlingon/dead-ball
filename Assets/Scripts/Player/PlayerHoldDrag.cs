using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHoldDrag : MonoBehaviour
{
    public int Current = 50;

    public bool Debugging = true;

    public event Action<PlayerKickMode, float2> Released;

    public event Action Cancelled;

    public event Action<PlayerKickMode> StartDrag;

    public bool IsDragging => recording;

    public float2 Drag => DragPoints.Origin - DragPoints.Current;

    public (float2 Origin, float2 Current) DragPoints;

    public (float3 Origin, float3 Current) DragPointsInWorld
        => (CamToWorldPos(DragPoints.Origin), CamToWorldPos(DragPoints.Current));

    private bool recording;

    private bool locked => !GameManager.Instance.Player?.CanControl ?? false;

    private bool hasPressed = false;
    public enum PlayerKickMode
    {
        Lob,
        Kick,
    }

    public void OnLobInput(InputAction.CallbackContext context)
    {
        if (locked) return;
        if (context.action.triggered && context.action.ReadValue<float>() != 0 &&
            context.action.phase == InputActionPhase.Performed)
        {
            TriggerPressed(PlayerKickMode.Lob);
        } else if (context.action.triggered && context.action.ReadValue<float>() == default &&
            context.action.phase == InputActionPhase.Performed)
        {
            if (!recording) return;
            TriggerReleased(PlayerKickMode.Lob);
        }
    }

    public void OnKickInput(InputAction.CallbackContext context)
    {
        if (locked) return;
        if (context.action.triggered && context.action.ReadValue<float>() != 0 &&
            context.action.phase == InputActionPhase.Performed)
        {
            TriggerPressed(PlayerKickMode.Kick);
        } else if (context.action.triggered && context.action.ReadValue<float>() == default &&
            context.action.phase == InputActionPhase.Performed)
        {
            if (!recording) return;
            TriggerReleased(PlayerKickMode.Kick);
        }
    }

    void Awake()
    {
        DragPoints.Origin = float2.zero;
        DragPoints.Current = float2.zero;
    }

    void TriggerPressed(PlayerKickMode Mode)
    {
        recording = true;
        DragPoints.Origin = Mouse.current.position.ReadValue();
        DragPoints.Current = DragPoints.Origin;
        StartDrag?.Invoke(Mode);
    }

    void TriggerReleased(PlayerKickMode mode)
    {
        recording = false;

        Released?.Invoke(mode, Drag);

        DragPoints.Origin = float2.zero;
        DragPoints.Current = float2.zero;
    }

    public void Reset()
    {
        recording = false;
        Cancelled?.Invoke();
        DragPoints.Origin = float2.zero;
        DragPoints.Current = float2.zero;
    }

    float3 CamToWorldPos(float2 pos)
    {
        var camPos = Camera.main.ScreenToWorldPoint(new float3(pos, 0));
        return new float3(camPos.x, camPos.y, 0);
    }

    void Update()
    {
        if (!recording) return;

        DragPoints.Current = Mouse.current.position.ReadValue();

        if (math.length(Drag) > Current)
        {
            DragPoints.Current = DragPoints.Origin + -(math.normalize(Drag) * Current);
        }

        if (Debugging)
        {
            DebugDraw.Line(DragPointsInWorld.Origin, DragPointsInWorld.Current, Color.yellow);
            DebugDraw.Circle(DragPointsInWorld.Origin, 0.25f, Color.cyan);
            DebugDraw.Circle(DragPointsInWorld.Current, 0.25f, Color.red);
        }
    }
}
