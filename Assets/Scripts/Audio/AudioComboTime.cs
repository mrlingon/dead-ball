using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioComboTime : MonoBehaviour
{
    [Header("Events")]
    public FMODUnity.EventReference ComboEvent;

    //Instance events
    FMOD.Studio.EventInstance Combo;

    //Parameters
    FMOD.Studio.PARAMETER_DESCRIPTION comboDesc;
    FMOD.Studio.PARAMETER_ID comboID;

    // Start is called before the first frame update
    void Start()
    {
        Combo = FMODUnity.RuntimeManager.CreateInstance(ComboEvent);

        FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("Combo", out comboDesc);
        comboID = comboDesc.id;

        GameManager.Instance.Scores.OnComboAdded += (_) => PlayCombo();
    }

    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByID(comboID, GameManager.Instance.Scores.Combo);
    }

    void PlayCombo()
    {
        Combo = FMODUnity.RuntimeManager.CreateInstance(ComboEvent);
        Combo.start();
        Combo.release();
    }
}
