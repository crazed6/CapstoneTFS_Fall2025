using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public float health = 100f;
    public Vector3 playerPosition;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        playerPosition = transform.position;
    }

    public void ApplyDamage(float amount)
    {
        health = Mathf.Max(0, health - amount);
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(100f, health + amount);
    }
}
