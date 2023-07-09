using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using static EnemyController;

public class EnemyWallah : MonoBehaviour
{
    [Header("Events")]
    public FMODUnity.EventReference WallahEvent;

    FMOD.Studio.EventInstance Wallah;

    FMOD.Studio.EventDescription EnemyWallahDescription;
    FMOD.Studio.PARAMETER_DESCRIPTION enemyStateDesc;
    FMOD.Studio.PARAMETER_ID enemyStateID;

    private EnemyState enemyState;

    public BallSensor ballSensor;

    void Start()
    {

        InitializeEvents();
        InitializeParameters();

        ballSensor = GetComponentInChildren<BallSensor>();
        if (ballSensor)
        {
            ballSensor.BallEnter += () =>
            {
                Wallah.start();
            };
            ballSensor.BallExit += () =>
            {
                Wallah.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            };
        }

        Wallah.start();

    }

    void Update()
    {
        WallahState();
    }

    void InitializeEvents()
    {
        Wallah = FMODUnity.RuntimeManager.CreateInstance(WallahEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Wallah, GetComponent<Transform>(), GetComponent<Rigidbody2D>());
    }

    void InitializeParameters()
    {
        EnemyWallahDescription = FMODUnity.RuntimeManager.GetEventDescription(WallahEvent);
        EnemyWallahDescription.getParameterDescriptionByName("EnemyState", out enemyStateDesc);
        enemyStateID = enemyStateDesc.id;
    }

    void WallahState()
    {
        EnemyController enemyController = this.GetComponent<EnemyController>();
        enemyState = enemyController.currentState;

        if (enemyState == EnemyState.RUN_TOWARDS)
            Wallah.setParameterByIDWithLabel(enemyStateID, "Running Towards");
        else if (enemyState == EnemyState.RUN_AWAY)
            Wallah.setParameterByIDWithLabel(enemyStateID, "Running Away");
        else
            Wallah.setParameterByIDWithLabel(enemyStateID, "Has Ball");
    }

    public void stopOnDeath()
    {
                Wallah.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                Wallah.release();

    }
}
