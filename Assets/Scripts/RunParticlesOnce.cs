using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunParticlesOnce : MonoBehaviour
{
    public ParticleSystem ParticleSystem;

    public void Awake()
    {
        ParticleSystem = GetComponent<ParticleSystem>();
        ParticleSystem.Play();
        Destroy(gameObject, ParticleSystem.main.duration);
    }
}
