using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ParticleSystem system = ParticleSpawner.CreateGunTrail(5, Vector3.right * 20, Quaternion.AngleAxis(-90, Vector3.up));
        ParticleSystem system = ParticleSpawner.CreateGunTrail(5, 1f, Vector3.right * 20, Vector3.forward + (Vector3.right * 20));
        system.Play();
    }
}
