using UnityEngine;
//Diego's Script

public class WallSpikeCollision : MonoBehaviour
{
    private bool trapIsActive = false;

    // Called by WallThorns to update state
    public void SetTrapActive(bool active)
    {
        trapIsActive = active;
    }

    void OnTriggerEnter(Collider other)
    {
        // Only react when trap is active
        if (!trapIsActive)
            return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit the Player!");
        }
    }
}
