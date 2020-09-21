using UnityEngine;

[ExecuteInEditMode]
public class BulletTrailEffect : MonoBehaviour
{
    public void Start()
    {
        Debug.Log("Start");

        var psys = GetComponent<ParticleSystem>();
        psys.Play();

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[psys.main.maxParticles];
        int actualCount = psys.GetParticles(particles);
        float trailLength = psys.shape.radius;
        for (int i = 0; i < actualCount; i++)
        {
            particles[i].remainingLifetime *= Vector3.Distance(transform.position, particles[i].position) / trailLength;
            Debug.Log($"particles[{i}].startLifetime = {particles[i].remainingLifetime}");
        }

        psys.SetParticles(particles);
    }
}
