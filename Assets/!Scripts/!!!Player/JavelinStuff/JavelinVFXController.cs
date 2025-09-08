using UnityEngine;

public class JavelinVFXController : MonoBehaviour
{
    [Header("Trail VFX References")]
    [Tooltip("Assign all your weapon trail GameObjects or ParticleSystems here")]
    public GameObject[] trailObjects;

    /// <summary>
    /// Enable/disable all weapon trails.
    /// </summary>
    public void SetTrailsActive(bool active)
    {
        if (trailObjects == null) return;

        foreach (var obj in trailObjects)
        {
            if (obj == null) continue;

            // If they are particle systems, handle properly
            var ps = obj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                if (active)
                {
                    ps.Play(true);
                }
                else
                {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
            else
            {
                // Otherwise just toggle the GameObject
                obj.SetActive(active);
            }
        }
    }
}
