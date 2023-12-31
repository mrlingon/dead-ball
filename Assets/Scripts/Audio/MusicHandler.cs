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
    FMOD.Studio.PARAMETER_DESCRIPTION isGrabbedDesc;
    FMOD.Studio.PARAMETER_ID isGrabbedID;

    [Header("Parameters")]
    [Range(0, 100)]
    public int murderCount;

    public bool isGrabbed = false;
    private bool checkIsGrabbed = false;

    void Awake()
    {
        if (GameManager.Instance.MusicHandler != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.Instance.MusicHandler = this;
    }



    //Start and Update
    void Start()
    {
        InitializeEvents();
        InitializeParameters();

        GameManager.Instance.OnCatchedBall += () => isGrabbed = true;
        GameManager.Instance.OnReleasedBall += () => isGrabbed = false;
        GameManager.Instance.Scores.OnKillAdded += (i) => murderCount = GameManager.Instance.Scores.Kills * 3;

        Music.start();
    }

    public void StopMusic()
    {
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StartMusic()
    {
        Music.start();
    }

    void Update()
    {
        MurderCountUpdate();
        IsGrabbedUpdate();
    }

    //Events and parameters
    void InitializeEvents()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance(MusicEvent);
    }

    public void InitializeParameters()
    {
        FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("MurderCount", out murderCountDesc);
        murderCountID = murderCountDesc.id;

        FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("IsGrabbed", out isGrabbedDesc);
        isGrabbedID = isGrabbedDesc.id;
    }

    //Methods for changing things
    void MurderCountUpdate()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByID(murderCountID, murderCount);
    }

    void IsGrabbedUpdate()
    {
        if (isGrabbed != checkIsGrabbed)
        {
            if (isGrabbed)
            {
                FMODUnity.RuntimeManager.StudioSystem.setParameterByID(isGrabbedID, 1f);
            }

            if (!isGrabbed)
            {
                FMODUnity.RuntimeManager.StudioSystem.setParameterByID(isGrabbedID, 0f);
            }

            checkIsGrabbed = !checkIsGrabbed;
        }


    }
}
