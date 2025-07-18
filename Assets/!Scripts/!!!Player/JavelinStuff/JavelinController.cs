//Ritwik
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Reflection;

public class JavelinController : MonoBehaviour
{
    [Header("Arc Settings")]
    public float speed = 150f;
    public float gravityStrength = 30f;
    public float lifetime = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 720f;

    [Header("AoE Settings")]
    public float aoeRadius = 3f;
    public LayerMask damageLayers;
    public int damageAmount = 25;
    public bool showDebugGizmo = true;

    private Vector3 velocity;
    private bool isFlying = false;
    private bool hasExploded = false;
    private bool isDestroyed = false; // New flag to detect self-destruction -_-

    public void SetDirection(Vector3 direction)
    {
        velocity = direction.normalized * speed;
        transform.rotation = Quaternion.LookRotation(velocity);
        isFlying = true;
        MoveAlongArc().Forget();
    }

    public async UniTaskVoid MoveAlongArc()
    {
        try
        {
            while (!isDestroyed)
            {
                velocity += Vector3.down * gravityStrength * Time.deltaTime;
                transform.position += velocity * Time.deltaTime;

                if (velocity != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(velocity.normalized);

                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);

                await UniTask.Yield();
            }
        }
        catch (MissingReferenceException)
        {
            // Object already destroyed, do nothing -_-
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isDestroyed = true; // Prevent further async access -_-
        
        Destroy(gameObject);
    }

    private void Explode()
    {
        hasExploded = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius, damageLayers);

        foreach (var hit in hits)
        {
            // Check all MonoBehaviours on the object
            foreach (var component in hit.GetComponents<MonoBehaviour>())
            {
                MethodInfo damageMethod = component.GetType().GetMethod("TakeDamage",
                    BindingFlags.Public | BindingFlags.Instance);

                if (damageMethod != null &&
                    damageMethod.GetParameters().Length == 1 &&
                    damageMethod.GetParameters()[0].ParameterType == typeof(int))
                {
                    damageMethod.Invoke(component, new object[] { damageAmount });
                    break; // Only apply damage once per target
                }
            }
        }

        // Optional: VFX

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmo) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
