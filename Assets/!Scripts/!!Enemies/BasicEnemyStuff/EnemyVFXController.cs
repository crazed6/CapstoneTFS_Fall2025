using System.Collections;
using UnityEngine;

public class EnemyVFXController : MonoBehaviour
{
    [Header("Death VFX")]
    [SerializeField] private ParticleSystem[] deathParticles; // multiple supported
    [SerializeField] private string dissolveProperty = "_DissolveAmount";
    [SerializeField] private float dissolveDuration = 2f;

    private Renderer[] renderers;
    private MaterialPropertyBlock mpb;
    private int dissolvePropertyID;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
        dissolvePropertyID = Shader.PropertyToID(dissolveProperty);
    }

    public void PlayDeathVFX()
    {
        // Play all particles assigned
        foreach (var ps in deathParticles)
        {
            if (ps != null)
                ps.Play();
        }

        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        float timer = 0f;

        while (timer < dissolveDuration)
        {
            timer += Time.deltaTime;
            float dissolveValue = Mathf.Lerp(0f, 1f, timer / dissolveDuration);

            foreach (Renderer rend in renderers)
            {
                rend.GetPropertyBlock(mpb);
                mpb.SetFloat(dissolvePropertyID, dissolveValue);
                rend.SetPropertyBlock(mpb);
            }

            yield return null;
        }
    }
}