using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has died! (Laser Grid Triggered)");
        }
    }
}
