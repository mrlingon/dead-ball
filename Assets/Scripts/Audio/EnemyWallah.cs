using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyController;

public class EnemyWallah : MonoBehaviour
{
    [Header("Events")]
    public FMODUnity.EventReference WallahEvent;

    FMOD.Studio.EventInstance Wallah;

    FMOD.Studio.EventDescription EnemyWallahDescription;
    FMOD.Studio.PARAMETER_DESCRIPTION enemyStateDesc;
    FMOD.Studio.PARAMETER_ID enemyStateID;
    /*FMOD.Studio.PARAMETER_DESCRIPTION murderCountDesc;
    FMOD.Studio.PARAMETER_ID murderCountID;*/

    //FMODUnity.StudioEventEmitter wallah;

    private EnemyState enemyState;

    void Start()
    {
        InitializeEvents();
        InitializeParameters();

        Wallah.start();
    }

    void Update()
    {
        WallahState();
    }

    void InitializeEvents()
    {
        Wallah = FMODUnity.RuntimeManager.CreateInstance(WallahEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Wallah, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    void InitializeParameters()
    {
        EnemyWallahDescription = FMODUnity.RuntimeManager.GetEventDescription(WallahEvent);
        EnemyWallahDescription.getParameterDescriptionByName("EnemyState", out enemyStateDesc);
        enemyStateID = enemyStateDesc.id;

        /*FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("MurderCount", out murderCountDesc);
        murderCountID = murderCountDesc.id;*/
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
}
