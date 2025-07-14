using UnityEngine;

public class WallSpikeCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit the Player!");
        }
    }
}
