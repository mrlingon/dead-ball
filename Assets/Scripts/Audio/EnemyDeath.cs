using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public FMODUnity.EventReference DeathEvent;
    FMOD.Studio.EventInstance death;

    // Start is called before the first frame update
    void Start()
    {
        PlayDeathSound();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayDeathSound()
    {
        death = FMODUnity.RuntimeManager.CreateInstance(DeathEvent);
        death.start();
        death.release();
    }
}
