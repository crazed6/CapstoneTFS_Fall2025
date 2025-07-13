using UnityEngine;
//Diego's Script
public class SpikeTrap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has died! (Spike Trap Triggered)");
        }
    }
}
