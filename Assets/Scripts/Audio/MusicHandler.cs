using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    [Header("Events")]
    public FMODUnity.EventReference MusicEvent;

    //Instanceing events
    FMOD.Studio.EventInstance Music;

    //Paramters
    FMOD.Studio.PARAMETER_DESCRIPTION murderCountDesc;
    FMOD.Studio.PARAMETER_ID murderCountID;

    [Range(0, 100)]
    public int murderCount;

    void Start()
    {
        InitializeEvents();
        InitializeParameters();

        Music.start();
    }

    void Update()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByID(murderCountID, murderCount);
    }

    void InitializeEvents()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/Music");
    }

    void InitializeParameters()
    {
        FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("MurderCount", out murderCountDesc);
        murderCountID = murderCountDesc.id;
    }
}
