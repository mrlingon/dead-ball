using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyController;

public class EnemyWallah : MonoBehaviour
{
    private EnemyState currentState = EnemyState.RUN_TOWARDS;

    [Header("Events")]
    public FMODUnity.EventReference WallahEvent;

    FMOD.Studio.EventInstance Wallah;

    FMOD.Studio.EventDescription EnemyWallahDescription;
    FMOD.Studio.PARAMETER_DESCRIPTION enemyStateDesc;
    FMOD.Studio.PARAMETER_ID enemyStateID;
    FMOD.Studio.PARAMETER_DESCRIPTION murderCountDesc;
    FMOD.Studio.PARAMETER_ID murderCountID;

    void Start()
    {
        InitializeParameters();
    }

    void Update()
    {
        WallahState();
    }

    void InitializeParameters()
    {
        EnemyWallahDescription = FMODUnity.RuntimeManager.GetEventDescription(WallahEvent);
        EnemyWallahDescription.getParameterDescriptionByName("EnemyState", out enemyStateDesc);
        enemyStateID = enemyStateDesc.id;

        FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("MurderCount", out murderCountDesc);
        murderCountID = murderCountDesc.id;
    }

    void WallahState()
    {
        if (currentState == EnemyState.HAS_BALL)
            Wallah.setParameterByIDWithLabel(enemyStateID, "Has Ball");
        else if (currentState == EnemyState.RUN_AWAY)
            Wallah.setParameterByIDWithLabel(enemyStateID, "Running Away");
        else
            Wallah.setParameterByIDWithLabel(enemyStateID, "Running Towards");
    }
}
