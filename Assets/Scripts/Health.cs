using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    [SerializeField] private int maxHealth = 100;
    public int damageAmount = 10;
    public Transform respawnPoint; //Allows for Respawn Point to be selected in the Inspector
    public float respawnDelayTime = 5f; //A slight delay before the Player Respawns
    private bool isDead = false;
    private int _health = 1;
    public bool IsDead => _health <= 0;


    public int health
    {
        get => _health;
        set
        {
            _health = value;

            if (_health > maxHealth)
                _health = maxHealth;

            Debug.Log("health value is { _health}");
        }
    }

    public void TakeDamage (int damage) ////Script just to call for when the player will take Damage
    {
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }
    }

    public void HealDamage (int heal) //Script just to call for when the player will be healed
    {
        _health += heal;
        if (_health >= 100)
        {
            _health = 100;
        }
    }

    void Die() //Uhhh, yes.
    {
        isDead = true;  // Mark player as dead
        Debug.Log("Player has died!");
        // Optionally, disable player movement or play a death animation here
        StartCoroutine(Respawn());
    }

    System.Collections.IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelayTime); // Wait for delay
        Debug.Log("Respawning...");

        // Reset player position and health
        transform.position = respawnPoint.position;
        health = 100;
        isDead = false;  // Allow player to die again

        // Optionally re-enable movement or other components
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            // Call the TakeDamage function on the player
            Health playerHealth = GetComponent<Health>();  // Assuming the player has a Health component attached
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);  // Pass the damage amount to the player's TakeDamage function
            }
        }
    }

}

