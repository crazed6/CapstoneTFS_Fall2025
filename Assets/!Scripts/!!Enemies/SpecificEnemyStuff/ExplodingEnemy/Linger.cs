using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Linger : MonoBehaviour
{
    [Header("Launch Settings")]
    public float launchForce = 20f;
    public float upwardForce = 1f;
    public float gravityMultiplier = 5f;
    public float gravityRampSpeed = 2f;
    public AudioClip warningSound;
    public float timeBeforeExplode = 2f;

    [HideInInspector]
    public Vector3 targetPosition;

    private Rigidbody rb;
    private bool hasLaunched = false;
    private float airborneTime = 0f;
    private AudioSource audioSource;
    private ExplodingEnemy explodingEnemy;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody not found.");
            return;
        }

        audioSource = GetComponent<AudioSource>();
        explodingEnemy = GetComponent<ExplodingEnemy>();

        rb.useGravity = true;
        rb.isKinematic = false;
        airborneTime = 0f;
        hasLaunched = false;

        LaunchAtTarget();
    }

    void FixedUpdate()
    {
        if (hasLaunched)
        {
            airborneTime += Time.fixedDeltaTime;

            float extraGravity = gravityMultiplier * airborneTime;
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }
    }

    void LaunchAtTarget()
    {
        if (hasLaunched) return;
        hasLaunched = true;

        Vector3 toTarget = (targetPosition - transform.position).normalized;
        Vector3 launchVector = toTarget * launchForce + Vector3.up * upwardForce;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(launchVector, ForceMode.VelocityChange);

        if (warningSound != null)
        {
            audioSource.PlayOneShot(warningSound);
        }

        StartCoroutine(DelayedExplode());
    }

    IEnumerator DelayedExplode()
    {
        yield return new WaitForSeconds(timeBeforeExplode);

        if (explodingEnemy != null)
        {
            explodingEnemy.SendMessage("Explode");
        }
    }
}
