using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsEvents2D))]
public class BallSensor : MonoBehaviour
{

    public event Action BallEnter;
    public event Action BallExit;
    public LayerMask targetLayer;
    private EnemyController enemyController;
    private PhysicsEvents2D events2D;

    public float runAwaySize = 2f;
    public float chaseSize = 1f;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
        events2D = GetComponent<PhysicsEvents2D>();
        events2D.TriggerEnter += (other) =>
        {
            if (targetLayer == (targetLayer | (1 << other.gameObject.layer))) BallEnter?.Invoke();
        };
        events2D.TriggerExit += (other) =>
        {
            if (targetLayer == (targetLayer | (1 << other.gameObject.layer))) BallExit?.Invoke();
        };
    }

    void Update()
    {
        if (enemyController.currentState == EnemyController.EnemyState.RUN_AWAY)
        {
            events2D.Collider.transform.localScale = new Vector3(runAwaySize, runAwaySize, runAwaySize);
        }
        if (enemyController.currentState == EnemyController.EnemyState.RUN_TOWARDS)
        {
            events2D.Collider.transform.localScale = new Vector3(chaseSize, chaseSize, chaseSize);
        }
    }

}
