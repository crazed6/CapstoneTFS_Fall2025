using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXController : MonoBehaviour
{
    [Header("Death VFX")]
    [SerializeField] private ParticleSystem[] deathParticles;
    [SerializeField] private string dissolveProperty = "_DissolveAmount";
    [SerializeField] private float dissolveDuration = 2f;

    [Header("Attack VFX")]
    [SerializeField] private VisualEffect rockThrowChargeUpPrefab;
    [SerializeField] private Transform rockThrowChargeUpPoint;
    [SerializeField] private VisualEffect slamVFXPrefab;
    [SerializeField] private Transform slamVFXPoint;

    private Renderer[] renderers;
    private MaterialPropertyBlock mpb;
    private int dissolvePropertyID;

    private VisualEffect activeChargeUpVFX;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
        dissolvePropertyID = Shader.PropertyToID(dissolveProperty);
    }

    public void PlayDeathVFX()
    {
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

    public void PlayRockThrowChargeUpVFX()
    {
        if (rockThrowChargeUpPrefab != null && rockThrowChargeUpPoint != null)
        {
            activeChargeUpVFX = Instantiate(
                rockThrowChargeUpPrefab,
                rockThrowChargeUpPoint.position,
                rockThrowChargeUpPoint.rotation,
                rockThrowChargeUpPoint
            );

            activeChargeUpVFX.Play();
        }
    }

    public void StopRockThrowChargeUpVFX()
    {
        if (activeChargeUpVFX != null)
        {
            activeChargeUpVFX.Stop();
            Destroy(activeChargeUpVFX.gameObject);
            activeChargeUpVFX = null;
        }
    }

    public void PlaySlamVFX()
    {
        if (slamVFXPrefab != null && slamVFXPoint != null)
        {
            VisualEffect slamVFX = Instantiate(
                slamVFXPrefab,
                slamVFXPoint.position,
                slamVFXPoint.rotation
            );
            slamVFX.Play();

            Destroy(slamVFX.gameObject, 5f); // cleanup after effect finishes
        }
    }
}
