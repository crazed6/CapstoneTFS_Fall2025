using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;



[System.Serializable]
public class PlayerHealthData
{
    public int currentHealth;
}

public class Health : MonoBehaviour
{
    
    public int testDamageAmount = 10;
    [SerializeField] private int maxHealth = 100;
    public int MaxHealth => maxHealth; // Read-only property to access max health
    private int _health = 50;
    public bool IsDead => _health <= 0;
    private bool isDead = false;
    public CheckpointSystem CheckpointSystem;

    //for health regen
    public float regenerationDelay = 5f;
    public int regenerationAmount = 1;
    public float regenerationInterval = 1f;

    private float timeSinceLastDamage = 0f;
    private float regenTimer = 0f;
    private bool canRegenerate = false;

    //for I-Frames
    private bool isInvincible = false;
    public float invincibilityDuration = 10f;

    private FadetoBlack fadetoBlack; // Reference to the fade effect script


    public int health
    {
        get => _health;
        set
        {
            _health = value;

            if (_health > maxHealth)
                _health = maxHealth;

            Debug.Log($"health value is { _health}");
        }
    }

    public void PlayerTakeDamage(DamageData damage) //previously (int damage)
    {
        if (isInvincible) return; // Ignore damage if invincible

        //Do calculations for damage here based on damage type and data
        //Combine DamageReceive and Health scripts into one script

        int amount = damage.profile.damageAmount; // Get the amount of damage from the DamageProfile
        DamageType type = damage.profile.damageType; // Get the type of damage from the DamageProfile

        _health -= (int)amount; //previously damage like in function declaration

        Debug.Log($"Player took {(int)amount} damage from {damage.source}. Current health: {_health}"); //just used to show health in the console

        timeSinceLastDamage = 0f; // Reset the timer when taking damage
        canRegenerate = false; // Disable regeneration when taking damage

        if (_health <= 0)
        {
            isDead = true;
            PlayerDie();
            return;
        }

        // Start invincibility
        StartCoroutine(InvincibilityFrames());
    }

    private IEnumerator InvincibilityFrames() //Invincibility frames
    {
        isInvincible = true;
        Debug.Log("Player is now invincible for " + invincibilityDuration + " seconds.");
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    public void PlayerHealDamage(int heal)
    {
        _health += heal;
        if (_health >= 100)
        {
            _health = 100;
        }
        Debug.Log($"Player healed {heal} health. Current health: {_health}"); //just used to show health in the console
    }

    public void PlayerDie()
    {
        if (isDead == true)
        {
            Debug.Log("Player has died!");

            // Trigger fade to black effect
            FadetoBlack fadeEffect = FindFirstObjectByType<FadetoBlack>(FindObjectsInactive.Include);
            if (fadeEffect != null)
            {
                fadeEffect.gameObject.SetActive(true); // Ensure the fade effect object is active
                fadeEffect.StartFadeToBlack();
            }
            else
            {
                Debug.LogWarning("FadetoBlack component not found in the scene.");
            }

            //CheckpointSystem.ShowGameOverPanel(); // Show the game over panel

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        ResetHealth(); // Initialize health to max health
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // Call the TakeDamage function on the player
            Health playerHealth = GetComponent<Health>();  // Assuming the player has a Health component attached
            if (playerHealth != null)
            {
                playerHealth.DebugDamage(testDamageAmount);  // Pass the damage amount to the player's TakeDamage function

            }

        }

        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    // Call the TakeDamage function on the player
        //    Health playerHealth = GetComponent<Health>();  // Assuming the player has a Health component attached
        //    if (playerHealth != null)
        //    {
        //        playerHealth.PlayerHealDamage(testDamageAmount);  // Pass the damage amount to the player's TakeDamage function

        //    }

        //}

        //Regeneration
        if (!IsDead && health < maxHealth)
        {
            timeSinceLastDamage += Time.deltaTime;
            if (timeSinceLastDamage >= regenerationDelay)
            {
                canRegenerate = true;
            }
            if (canRegenerate)
            {
                regenTimer += Time.deltaTime;
                if (regenTimer >= regenerationInterval)
                {
                    PlayerHealDamage(regenerationAmount);
                    Debug.Log("Regenerating health: " + regenerationAmount);
                    regenTimer = 0f;
                }
            }
        }
    }

    public void DebugDamage(int testDamageAmount)
    {
        // This function is for testing purposes to apply damage directly
        _health -= testDamageAmount;
        Debug.Log($"[Debug] Player took {testDamageAmount} damage. Current health: {_health}");
        if (_health <= 0)
        {
            PlayerDie();
        }
    }

    public void ResetHealth()
    {
        // Reset health to max health
        _health = maxHealth;
        isDead = false;
        canRegenerate = true;
        Debug.Log("Player health reset to " + _health);
    }

    //Example Script for inside Enemy Attack script

    public DamageProfile attackProfile; // Reference to the DamageProfile ScriptableObject
    private void DealDamage(GameObject target)
    {
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            DamageData data = new DamageData(gameObject, attackProfile); // Create a new DamageData instance, and plug here
            targetHealth.PlayerTakeDamage(data);
        }
    }

}
