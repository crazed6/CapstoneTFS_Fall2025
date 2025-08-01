using System;
using UnityEngine;

public class EnemyDamageComponent : MonoBehaviour
{
    public event Action OnDeath; //Event to notify when the enemy dies

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Health UI")]
    public HealthBar healthBar;
    public float hideDelay = 15.0f; //Delay before hiding the health bar
    public float timeSinceLastDamage = 0f; //Time since last damage taken

    private Coroutine hideHealthBar;

    private bool isDead = false; //Flag to check if the enemy is already dead

    //[Header("Animator")]
    //private Animator anim;
    //public string hurtTrigger = "TakeDamage";
    //public string dieTrigger = "Die";


    private void Update()
    {
        if (healthBar != null && currentHealth < maxHealth && currentHealth > 0)
        {
            timeSinceLastDamage += Time.deltaTime;

            if (timeSinceLastDamage >= hideDelay)
            {
                if(healthBar.gameObject.activeSelf)
                {
                    healthBar.gameObject.SetActive(false);
                    Debug.LogWarning("Hiding health bar after delay");
                }
            }
        }
    }

    private void Awake()
    { 
        currentHealth = maxHealth;

        if (healthBar == null) healthBar = GetComponentInChildren<HealthBar>();

        //initially invisible
        if (healthBar != null)
        { 
            healthBar.SetHealth(1f); //Set to full health
            healthBar.gameObject.SetActive(false);
        }

        //animator = GetComponent<Animator>(); Set up once we have visuals

    }

    public void TakeDamage(float damage, GameObject source = null)
    {
        if (isDead) return; //Prevent further damage if already dead

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        timeSinceLastDamage = 0f; //Reset timer on damage taken

        Debug.Log($"{gameObject.name} took {damage} damage from {source?.name ?? "Unknown"} | HP: {currentHealth}/{maxHealth}");

        //trigger damage animation
        //if (anim != null) anim.SetTrigger("TakeDamage");

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
        
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        Debug.Log($"{gameObject.name} healed {amount}. HP: {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        isDead = true; //Set dead flag to prevent further actions

        Debug.Log($"{gameObject.name} is dead.");

        //trigger death animation
        //if (anim != null) anim.SetTrigger("Die");


        // Kaylani's addition : Change the material color to red when the enemy dies
        // Get the Renderer component and change the material's color to red.
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Loop through each renderer that was found.
        foreach (Renderer rend in renderers)
        {
            rend.material.color = Color.red;
        }
        // ---------------------


        if (healthBar != null) healthBar.gameObject.SetActive(false);

        //Add cleanup logic here - disable AI movement, pool object, etc.

        Destroy(gameObject, 2.0f); //Delay if anims are ~2seconds
        OnDeath?.Invoke();
    }


    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            float normalized = currentHealth / maxHealth;
            healthBar.SetHealth(normalized);

            //Show health bar if not already visible
            if (!healthBar.gameObject.activeSelf && currentHealth > 0 && currentHealth < maxHealth)
            {
                healthBar.gameObject.SetActive(true);
                Debug.LogWarning("Showing health bar");
            }
        }
    }

    //private IEnumerator HideHealthBarAfterDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    if (healthBar != null && currentHealth > 0) healthBar.gameObject.SetActive(false);
    //}

    //Josh code here
    public void TakeDamage2(DamageData data)
    {
        Debug.Log($"{gameObject.name} is taking {data.profile.damageAmount} {data.profile.damageType} damage from {data.source?.name ?? "Unknown"} (via DamageData)");
        TakeDamage(data.profile.damageAmount, data.source);
    }
    //Josh code ends
}
