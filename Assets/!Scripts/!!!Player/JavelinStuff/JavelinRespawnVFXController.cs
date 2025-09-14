using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class JavelinRespawnVFXController : MonoBehaviour
{
    [Header("Dissolve Settings")]
    [SerializeField] private string dissolveProperty = "_DissolveAmount";
    [SerializeField] private float dissolveDuration = 2f;

    [Header("Trail VFX")]
    [Tooltip("Assign all your particle trail GameObjects here")]
    [SerializeField] private GameObject[] trailObjects;

    [Header("Spawn VFX")]
    [Tooltip("Assign a child Visual Effect under the back stick")]
    [SerializeField] private VisualEffect spawnVFXChild;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private Renderer[] renderers;
    private MaterialPropertyBlock mpb;
    private int dissolvePropertyID;
    private Coroutine dissolveRoutine;

    // State tracking
    private bool isCurrentlyHidden = false;
    private bool isRespawning = false;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        mpb = new MaterialPropertyBlock();
        dissolvePropertyID = Shader.PropertyToID(dissolveProperty);

        // Debug check
        if (renderers.Length == 0)
        {
            Debug.LogWarning($"[JavelinRespawnVFXController] No renderers found on {gameObject.name}!");
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[JavelinRespawnVFXController] Initialized with {renderers.Length} renderers");
        }
    }

    /// <summary>
    /// Instantly sets the dissolve to a value (0 = visible, 1 = invisible)
    /// </summary>
    public void SetDissolveInstant(float value)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[JavelinRespawnVFXController] SetDissolveInstant({value}) called at {Time.time}");
        }

        foreach (var rend in renderers)
        {
            if (rend == null) continue;

            // Get current property block
            rend.GetPropertyBlock(mpb);

            // Set dissolve property
            mpb.SetFloat(dissolvePropertyID, value);

            // Apply property block
            rend.SetPropertyBlock(mpb);

            if (enableDebugLogs)
            {
                Debug.Log($"[JavelinRespawnVFXController] Renderer '{rend.name}' dissolve set to {value}");
            }
        }
    }

    /// <summary>
    /// Hide instantly by setting dissolve to 1 and turning off trails
    /// </summary>
    public void HideInstant()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[JavelinRespawnVFXController] HideInstant() called at {Time.time} - Current state: hidden={isCurrentlyHidden}, respawning={isRespawning}");
        }

        // Stop any ongoing dissolve routine
        if (dissolveRoutine != null)
        {
            StopCoroutine(dissolveRoutine);
            dissolveRoutine = null;
            if (enableDebugLogs) Debug.Log("[JavelinRespawnVFXController] Stopped existing dissolve routine");
        }

        // Update state
        isRespawning = false;
        isCurrentlyHidden = true;

        // Force dissolve to maximum (invisible)
        SetDissolveInstant(1f);
        SetTrailsActive(false);

        // Additional safety: disable renderers entirely as backup
        foreach (var rend in renderers)
        {
            if (rend != null)
            {
                rend.enabled = false;
            }
        }

        if (enableDebugLogs)
        {
            Debug.Log("[JavelinRespawnVFXController] HideInstant() completed - renderers disabled");
        }
    }

    /// <summary>
    /// Animate dissolve from 1 ? 0 over dissolveDuration seconds, play spawn VFX, enable trails at the end
    /// </summary>
    public void Respawn()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[JavelinRespawnVFXController] Respawn() called at {Time.time} - Current state: hidden={isCurrentlyHidden}, respawning={isRespawning}");
        }

        // Stop any existing routine
        if (dissolveRoutine != null)
        {
            StopCoroutine(dissolveRoutine);
            if (enableDebugLogs) Debug.Log("[JavelinRespawnVFXController] Stopped existing dissolve routine for respawn");
        }

        dissolveRoutine = StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (enableDebugLogs) Debug.Log("[JavelinRespawnVFXController] Starting respawn routine");

        isRespawning = true;
        float elapsed = 0f;

        // Re-enable renderers
        foreach (var rend in renderers)
        {
            if (rend != null)
            {
                rend.enabled = true;
            }
        }

        // Start fully dissolved
        SetDissolveInstant(1f);

        // Turn off trails at start
        SetTrailsActive(false);

        // Play spawn VFX at start
        if (spawnVFXChild != null)
        {
            spawnVFXChild.gameObject.SetActive(true);
            spawnVFXChild.Play();
        }

        // Animate dissolve
        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dissolveDuration);
            float dissolveValue = Mathf.Lerp(1f, 0f, t);
            SetDissolveInstant(dissolveValue);
            yield return null;
        }

        // Fully visible
        SetDissolveInstant(0f);

        // Enable trails
        SetTrailsActive(true);

        // Optionally stop the spawn VFX so it can be reused
        if (spawnVFXChild != null)
        {
            spawnVFXChild.Stop();
            spawnVFXChild.gameObject.SetActive(false);
        }

        // Update state
        isCurrentlyHidden = false;
        isRespawning = false;
        dissolveRoutine = null;

        if (enableDebugLogs) Debug.Log("[JavelinRespawnVFXController] Respawn routine completed");
    }

    /// <summary>
    /// Enable/disable all trail objects
    /// </summary>
    private void SetTrailsActive(bool active)
    {
        if (trailObjects == null) return;

        foreach (var obj in trailObjects)
        {
            if (obj == null) continue;

            var ps = obj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                if (active) ps.Play(true);
                else ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            else
            {
                obj.SetActive(active);
            }
        }
    }

    // Public method to check current state
    public bool IsHidden => isCurrentlyHidden;
    public bool IsRespawning => isRespawning;

    // Method to force check dissolve values (for debugging)
    [ContextMenu("Debug Dissolve Values")]
    public void DebugDissolveValues()
    {
        foreach (var rend in renderers)
        {
            if (rend == null) continue;

            rend.GetPropertyBlock(mpb);
            float dissolveValue = mpb.GetFloat(dissolvePropertyID);
            Debug.Log($"Renderer '{rend.name}': dissolve = {dissolveValue}, enabled = {rend.enabled}");
        }
    }
}