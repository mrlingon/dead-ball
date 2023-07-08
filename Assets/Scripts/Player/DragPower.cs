using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DragPower : MonoBehaviour
{
    public int MaxPower;

    public float RechargeRate;

    public PlayerHoldDrag holdDrag;

    [HideInInspector]
    public float currentPower;

    public float PowerLeft => MaxPower - currentPower;

    public float PowerUsage =>  (currentPower - math.length(holdDrag.Drag));

    [HideInInspector]
    public bool active = true;

    void Awake() {
        currentPower = MaxPower;
    }

    // Start is called before the first frame update
    void Start()
    {
        holdDrag.Released += (drag) => {
            currentPower -= math.length(drag);
        };
    }

    public void AddPower(float power)
    {
        currentPower = math.min(MaxPower, currentPower + power);
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        holdDrag.Current = (int)currentPower;

        if (currentPower < MaxPower && !holdDrag.IsDragging)
            currentPower += RechargeRate*Time.deltaTime;
    }

    public void Activate() {
        active = true;
        currentPower = MaxPower;
        holdDrag.Current = (int)currentPower;
    }

    public void Deactivate() {
        active = false;
    }
}
