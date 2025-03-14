using UnityEngine;

public class DroneDetection : MonoBehaviour
{
    private ChaserDrone droneScript;

    void Start()
    {
        droneScript = GetComponentInParent<ChaserDrone>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            droneScript.ActivateDrone(); 
        }
    }
}
