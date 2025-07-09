
using UnityEngine.UI; //JK - added for Image component
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    // Reference to the TextMeshPro component **JK - do we need text? 
    //public TMP_Text HealthText;

    // Reference to the player's Health script
    public Health playerHealth;
    public Image healthFillImage; //JK - adding from class re: Hisham
    [SerializeField] private int maxHealth = 100; //Set this to whatever in inspector

    void Start()
    {
        // Optionally, find the player's Health script if it's not assigned
        if (playerHealth == null)
        {
            playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();  // Assuming the player has the "Player" tag
        }

        // Make sure health fill image is set in the Inspector or dynamically assigned
        if (healthFillImage == null)
        {
            Debug.LogError("Image not assigned!");
        }
    }

    void Update()
    {
        // Only update the health display if the healthFillImage and playerHealth are not null
        if (playerHealth != null && healthFillImage != null)
        {
            float fillAmount = (float)playerHealth.health / maxHealth;
            healthFillImage.fillAmount = fillAmount; // JK - Update the health fill image
        }
    }
}
