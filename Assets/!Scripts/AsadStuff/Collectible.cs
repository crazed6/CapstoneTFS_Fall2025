using UnityEngine;

public abstract class Collectible : MonoBehaviour //Public abstract
{
    public abstract void Collect();
    public CollectibleManager CollectibleManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
            Destroy(gameObject);
        }
    }
}
