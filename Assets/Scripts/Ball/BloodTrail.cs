using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

public class BloodTrail : MonoBehaviour
{
    BallPhysicsBody ballPhysicsBody;

    void Awake()
    {
        TryGetComponent(out ballPhysicsBody);
    }

    private float currentRadius = 0;

    private int counter = 0;

    public void ActivateTrail(float startRadius, float duration)
    {
        if (GameManager.Instance.GameField == null)
            return;

        currentRadius = startRadius;

        LeanTween.value(gameObject, currentRadius, 0, duration).setOnUpdate((float val) => {
            if (ballPhysicsBody?.IsAirborne ?? true)
                return;

            counter++;

            const int MaxCounter = 6;

            if (counter >= MaxCounter) {
                counter = 0;
                GameManager.Instance.GameField.SplatterPaint(new float2(transform.position.x, transform.position.y), val, 4, 8);
            }
        });
    }
}
