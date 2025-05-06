using UnityEngine;

[CreateAssetMenu(fileName = "DamageSettings", menuName = "Combat/Damage Settings")]
public class DamageSettings : ScriptableObject
{

    [Header("Dash Damage")]
    public float minDashSpeed = 5f;
    public float maxDashSpeed = 20f;
    public float minDashDamage = 10f;
    public float maxDashDamage = 40f;

    [Header("Javelin Damage")]
    public float javelinDamage = 50f;

}
