using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticles : MonoBehaviour
{
    public static DeathParticles Instance;
    ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        Instance = this;
    }

    public void Play(Vector3 position)
    {
        transform.position = position;
        ps.Play();
    }
}
