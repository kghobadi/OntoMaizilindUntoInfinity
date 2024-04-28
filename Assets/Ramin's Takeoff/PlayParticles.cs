using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticles : MonoBehaviour
{
    ParticleSystem ps;

    void OnEnable()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Play();
    }
}