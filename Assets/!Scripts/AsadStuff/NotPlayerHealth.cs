using UnityEngine;
using UnityEngine.UI;

public class NotPlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthSlider;
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            TakeDamage(10f);
            Debug.Log("C press deals 10 damage");
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Heal(10f);
            Debug.Log("V press regenerates 10 health");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            TakeDamage(5f);
            Debug.Log("B press deals 5 damage");

        }
    
    }


    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthSlider.value = currentHealth;
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthSlider.value = currentHealth;
    }
}
