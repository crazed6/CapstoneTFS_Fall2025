using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(BaseEnemy))]
public class DamageableEnemy : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Health UI")]
    public HealthBar healthBar;
    public float hideDelay = 3.0f; //Delay before hiding the health bar

    private Coroutine hideHealthBar;

    //[Header("Animator")]
    //private Animator anim;
    //public string hurtTrigger = "TakeDamage";
    //public string dieTrigger = "Die";


    void Awake()
    { 
        currentHealth = maxHealth;

        if (healthBar == null) healthBar = GetComponentInChildren<HealthBar>();

        //initially invisible
        if (healthBar != null) 
            healthBar.SetHealth(1f); //Set to full health
            healthBar.gameObject.SetActive(false);


        //animator = GetComponent<Animator>(); Set up once we have visuals

    }

    public void TakeDamage(float damage, GameObject source = null)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

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
        Debug.Log($"{gameObject.name} is dead.");

        //trigger death animation
        //if (anim != null) anim.SetTrigger("Die");
        
        if (healthBar != null) healthBar.gameObject.SetActive(false);


        //Add cleanup logic here - disable AI movement, pool object, etc.

        Destroy(gameObject, 2.0f); //Delay if anims are ~2seconds
    }


    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            float normalized = currentHealth / maxHealth;
            healthBar.SetHealth(normalized);

            bool shouldShow = currentHealth < maxHealth && currentHealth > 0;

            if (shouldShow)
            {
                healthBar.gameObject.SetActive(true);

                if (hideHealthBar != null)
                    StopCoroutine(hideHealthBar);

                hideHealthBar = StartCoroutine(HideHealthBarAfterDelay(hideDelay));
            }
            else
            {
                healthBar.gameObject.SetActive(false);
            }
        }
    }


    private IEnumerator HideHealthBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (healthBar != null && currentHealth > 0) healthBar.gameObject.SetActive(false);
    }

}
