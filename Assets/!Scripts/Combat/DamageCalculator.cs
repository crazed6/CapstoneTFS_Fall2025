using UnityEngine;

public class DamageCalculator : MonoBehaviour
{
    public static DamageSettings Settings { get; private set; }

    [SerializeField] private DamageSettings damageSettings;

    void Awake()
    {
        if (damageSettings == null)
        {
            Debug.LogError("DamageSettings not assigned in the inspector.");
            return;
        }

        Settings = damageSettings;
    }

    public static float CalculateDamage(float speed)
    {
        if (Settings == null)
        {
            Debug.LogError("DamageSettings not initialized.");
            return 0f;
        }

        float clampedSpeed = Mathf.Clamp(speed, Settings.minDashSpeed, Settings.maxDashSpeed);
        float t = (clampedSpeed - Settings.minDashSpeed) / (Settings.maxDashSpeed - Settings.minDashSpeed);
        return Mathf.Lerp(Settings.minDashDamage, Settings.maxDashDamage, t);

    }

    public static float GetJavelinDamage()
    {
        if (Settings == null)
        {
            Debug.LogError("DamageSettings not initialized.");
            return 0f;
        }

        return Settings.javelinDamage;
    }



}
