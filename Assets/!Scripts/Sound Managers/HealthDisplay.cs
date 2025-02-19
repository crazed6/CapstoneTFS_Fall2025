using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    // Reference to the TextMeshPro component
    public TMP_Text HealthText;

    // Reference to the player's Health script
    public Health playerHealth;

    void Start()
    {
        // Optionally, find the player's Health script if it's not assigned
        if (playerHealth == null)
        {
            playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();  // Assuming the player has the "Player" tag
        }

        // Make sure HealthText is set in the Inspector or dynamically assigned
        if (HealthText == null)
        {
            Debug.LogError("HealthText is not assigned!");
        }
    }

    void Update()
    {
        // Only update the health display if the HealthText and playerHealth are not null
        if (HealthText && playerHealth != null)
        {
            HealthText.text = $"Health: {playerHealth.health}";  // Access the health property
        }
    }
}
