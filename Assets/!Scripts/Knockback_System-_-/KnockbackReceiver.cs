//Ritwik -_-
using UnityEngine;
using Cysharp.Threading.Tasks;

public class KnockbackReceiver : MonoBehaviour
{
    private Rigidbody rb;
    private CharacterController controller;
    private bool isBeingKnocked = false;

    [Header("Debug")]
    public bool showDirection = false;
    private Vector3 lastForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    public void ApplyKnockback(KnockbackData data)
    {
        if (isBeingKnocked) return;

        Vector3 direction = (transform.position - data.sourcePosition).normalized;
        direction.y = 0f;
        direction.Normalize();

        Vector3 forceVector = direction * data.force;
        forceVector.y = data.force * data.upwardForce;
        lastForce = forceVector;

        KnockbackSequence(forceVector, data.duration, data.overrideVelocity).Forget();
    }

    private async UniTaskVoid KnockbackSequence(Vector3 force, float duration, bool overrideVelocity)
    {
        isBeingKnocked = true;

        if (overrideVelocity)
            rb.linearVelocity = Vector3.zero;

        rb.linearVelocity = force;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            await UniTask.Yield();
        }

        isBeingKnocked = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (showDirection)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + lastForce.normalized * 3f);
        }
    }
}
