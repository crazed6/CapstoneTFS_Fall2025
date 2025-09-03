using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public abstract class EnemyVFXController : MonoBehaviour
{
    [Header("Death VFX")]
    [SerializeField] protected ParticleSystem[] deathParticles;
    [SerializeField] protected string dissolveProperty = "_DissolveAmount";
    [SerializeField] protected float dissolveDuration = 2f;

    protected Renderer[] renderers;
    protected MaterialPropertyBlock mpb;
    protected int dissolvePropertyID;
    protected Animator animator; // enemy animator reference

    protected virtual void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
        dissolvePropertyID = Shader.PropertyToID(dissolveProperty);

        animator = GetComponentInChildren<Animator>();
    }

    public virtual void PlayDeathVFX()
    {
        // freeze animation on current frame
        if (animator != null)
            animator.speed = 0f;

        // play particles
        foreach (var ps in deathParticles)
        {
            if (ps != null)
                ps.Play();
        }

        // start dissolve
        StartCoroutine(DissolveRoutine());
    }

    protected virtual IEnumerator DissolveRoutine()
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
