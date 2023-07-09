using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public FMODUnity.EventReference DeathEvent;
    FMOD.Studio.EventInstance death;

    public void PlayDeathSound()
    {
        death = FMODUnity.RuntimeManager.CreateInstance(DeathEvent);
        death.start();
        death.release();
    }
}
