using UnityEngine;
using TMPro;

public class LookAtPlayerTextSmooth : MonoBehaviour
{
    public GameObject player;              // Reference to the player
    public TextMeshPro textMeshPro;        // The TextMeshPro component (3D Text)
    public float rotationSpeed = 5f;       // The speed at which the text rotates to face the player

    private Renderer textRenderer;

    void Start()
    {
        // Get the Renderer from the TextMeshPro component
        textRenderer = textMeshPro.GetComponent<Renderer>();
        textRenderer.enabled = false;  // Initially hide the text
    }

    void Update()
    {
        // Calculate the distance between the player and this object (where the script is attached)
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // If the player is within range, show the text
        if (distance <= 50f)  // You can adjust the distance threshold
        {
            textRenderer.enabled = true;
        }
        else
        {
            textRenderer.enabled = false;
        }

        // If the text is visible, make it always look at the player
        if (textRenderer.enabled)
        {
            LookAtPlayer();
        }
    }

    void LookAtPlayer()
    {
        // Calculate the direction from the text to the player
        Vector3 directionToPlayer = player.transform.position - textMeshPro.transform.position;

        // Calculate the target rotation using LookAt
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Correct the backwards issue by rotating the text 180 degrees on the Y-axis
        targetRotation *= Quaternion.Euler(0, 180, 0);  // This flips the text correctly

        // Smoothly rotate the text towards the target rotation
        textMeshPro.transform.rotation = Quaternion.Slerp(textMeshPro.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}