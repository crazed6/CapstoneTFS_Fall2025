using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    
    public int damageAmount = 10;
    [SerializeField] private int maxHealth = 100;
    private int _health = 1;
    public bool IsDead => _health <= 0;
    private bool isDead = false;
    public CheckpointSystem CheckpointSystem; 

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

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }
    }

    public void HealDamage(int heal)
    {
        _health += heal;
        if (_health >= 100)
        {
            _health = 100;
        }
    }

    void Die()
    {
        if (isDead == true)
        {
            Debug.Log("Player has died!");
            CheckpointSystem.Respawn();
            health = 100;
            isDead = false;
        }
        
        // Mark player as de
        
        // Optionally, disable playerusi movement or play a death animation here
        
          // Allow player to die again
    }

    // Start is called before the first frame update
    void Start()
    {

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
    