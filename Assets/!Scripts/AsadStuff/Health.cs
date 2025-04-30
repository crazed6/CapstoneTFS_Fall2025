using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


[System.Serializable]
public class PlayerHealthData
{
    public int currentHealth;
}

public class Health : MonoBehaviour
{
    
    public int damageAmount = 10;
    [SerializeField] private int maxHealth = 100;
    private int _health = 1;
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
        //Debug.Log($"Player took {DamageInfo.amount} damage from {DamageInfo.source}. Current health: {_health}"); //just used to show health in the console

        timeSinceLastDamage = 0f; // Reset the timer when taking damage
        canRegenerate = false; // Disable regeneration when taking damage

        if (_health <= 0)
        {
            isDead = true;
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
        Debug.Log($"Player healed {heal} health. Current health: {_health}"); //just used to show health in the console
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
        
        // Mark player as dead
        
        // Optionally, disable players movement or play a death animation here
        
          // Allow player to die again
    }

    // Start is called before the first frame update
    void Start()
    {

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
                playerHealth.TakeDamage(damageAmount);  // Pass the damage amount to the player's TakeDamage function

            }

        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            // Call the TakeDamage function on the player
            Health playerHealth = GetComponent<Health>();  // Assuming the player has a Health component attached
            if (playerHealth != null)
            {
                playerHealth.HealDamage(damageAmount);  // Pass the damage amount to the player's TakeDamage function

            }

        }

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
                    HealDamage(regenerationAmount);
                    Debug.Log("Regenerating health: " + regenerationAmount);
                    regenTimer = 0f;
                }
            }
        }
    }



    // DamageInfo struct to hold damage information

    public struct DamageInfo
    {
        public int amount;
        public string source; // Could be "Goblin", "Fireball", "Trap", etc.
        public GameObject attacker; // Optional: reference to enemy GameObject

        public DamageInfo(int amount, string source, GameObject attacker = null)
        {
            this.amount = amount;
            this.source = source;
            this.attacker = attacker;
        }
    }

    //Example script to call damage, script would have to be placed on enemies.
    private void DealDamageToPlayer(GameObject player)
    {
        DamageInfo damage = new DamageInfo(10, "Worker", gameObject);
        player.GetComponent<Health>().TakeDamage(damage.amount);
    }

    //Example script for projectile
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DamageInfo damage = new DamageInfo(10, "Missile", gameObject);
            other.GetComponent<Health>().TakeDamage(damage.amount);
            Destroy(gameObject); // Destroy the projectile after dealing damage 
        }
    }

    //Worker Projectile - 
    //Exploder - Dash Into (player dashing into enemy)
    //Exploder - Inside Radius (More damage than outer radius, instakill)
    //Exploder - Medium Radius (Deal maybe 50% damage)
    //Exploder - Outside Radius (Less damage than middle radius, maybe 20%)
    //Heavy Enemy Lob Attack - (Maybe 50% damage)
    //Heavy Enemy Slam Attack - (Maybe 80%)

    //Traps Damage


    //previous way to save Health to File and load features

    //public void SaveHealthToFile()
    //{
    //    PlayerHealthData data = new PlayerHealthData { currentHealth = _health };
    //    string json = JsonUtility.ToJson(data);
    //    File.WriteAllText(Application.persistentDataPath + "/playerHealth.json", json);
    //}

    //public void LoadHealthFromFile()
    //{
    //    string path = Application.persistentDataPath + "/playerHealth.json";
    //    if (File.Exists(path))
    //    {
    //        string json = File.ReadAllText(path);
    //        PlayerHealthData data = JsonUtility.FromJson<PlayerHealthData>(json);
    //        _health = data.currentHealth;
    //    }
    //}


}
