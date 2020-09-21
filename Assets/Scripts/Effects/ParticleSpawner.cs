using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner
{
    public static GameObject GunTrailPrefab = Resources.Load<GameObject>("Prefabs/GunTrail");


    public static ParticleSystem CreateGunTrail(float length, float thickness, Vector3 startPos, Vector3 targetPos) 
    {
        return CreateGunTrail(length, thickness, startPos, Quaternion.FromToRotation(Vector3.right, targetPos - startPos));
    }

    public static ParticleSystem CreateGunTrail(float length, float thickness, Vector3 startPos = default, Quaternion rot = default) 
    {
        GameObject gunTrail = GameObject.Instantiate(GunTrailPrefab, startPos, rot);

        ParticleSystem particle = gunTrail.GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule shape = particle.shape;

        // Adjust the shape of the particle effect
        shape.radius = length;
        shape.position = Vector3.right * length;
        shape.scale = Vector3.right + (new Vector3(0f, 1f, 1f) * thickness);
        
        // Adjust the density of the particles so they're uniform across the length
        var burstArray = new ParticleSystem.Burst[particle.emission.burstCount];
        short minParticles = (short)(3 * length);
        short maxParticles = (short)(4.5 * length);
        particle.emission.SetBurst(0, new ParticleSystem.Burst(0f, minParticles, maxParticles));

        return particle;
    }
}
