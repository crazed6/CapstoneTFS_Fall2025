using UnityEngine;

public class SpeedParticleController : MonoBehaviour
{
    public CharacterController playerController; // Reference to player controller
    public ParticleSystem speedParticles;        // Reference to the particle system

    [Header("Speed Threshold")]
    public float speedThreshold = 45f;           // Speed to start particles

    private bool particlesPlaying = false;

    void Start()
    {
        if (playerController == null)
            playerController = CharacterController.instance;

        if (speedParticles == null)
            speedParticles = GetComponent<ParticleSystem>();

        if (speedParticles == null)
            Debug.LogError("SpeedParticleController: No ParticleSystem found!");
    }

    void Update()
    {
        if (playerController == null || speedParticles == null)
            return;

        // Get horizontal player speed
        Vector3 velocity = playerController.rb.linearVelocity;
        Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);
        float speed = horizontalVelocity.magnitude;

        // Start particles if speed exceeds threshold and not playing yet
        if (speed >= speedThreshold && !particlesPlaying)
        {
            speedParticles.Play();
            particlesPlaying = true;
        }
        // Stop particles if speed falls below threshold and particles are playing
        else if (speed < speedThreshold && particlesPlaying)
        {
            speedParticles.Stop();
            particlesPlaying = false;
        }
    }
}