using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Collider))]
public class CheckpointVFXController : MonoBehaviour
{
    [Header("Checkpoint VFX")]
    [SerializeField] private VisualEffect swirlVFXPrefab;   // Prefab for the swirl burst
    [SerializeField] private Transform swirlSpawnPoint;     // Where to spawn the swirl
    [SerializeField] private ParticleSystem idleParticles;  // Optional: base looping idle particles

    private VisualEffect activeSwirlVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlaySwirlVFX();
        }
    }

    public void PlaySwirlVFX()
    {
        if (swirlVFXPrefab != null && swirlSpawnPoint != null)
        {
            activeSwirlVFX = Instantiate(
                swirlVFXPrefab,
                swirlSpawnPoint.position,
                swirlSpawnPoint.rotation,
                swirlSpawnPoint // parent so it follows checkpoint
            );

            activeSwirlVFX.Play();

            Destroy(activeSwirlVFX.gameObject, 5f); // adjust lifetime if needed
        }
    }

    public void StopSwirlVFX()
    {
        if (activeSwirlVFX != null)
        {
            activeSwirlVFX.Stop();
            Destroy(activeSwirlVFX.gameObject);
            activeSwirlVFX = null;
        }
    }

    public void PlayIdleParticles()
    {
        if (idleParticles != null && !idleParticles.isPlaying)
            idleParticles.Play();
    }

    public void StopIdleParticles()
    {
        if (idleParticles != null && idleParticles.isPlaying)
            idleParticles.Stop();
    }
}
