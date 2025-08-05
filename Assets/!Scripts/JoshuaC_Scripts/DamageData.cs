using UnityEngine;

public enum DamageType
{
    Melee,
    Ranged,
    Explosion,
    Trap,
    Death
}

public struct DamageData
{
    public GameObject source;
    public DamageProfile profile; //DamageProfile is a ScriptableObject that contains damage type and amount

    // New addition for custom damage value, used for testing Speed's custom damage value
    public float customDamage;

    public DamageData(GameObject source, DamageProfile profile)
    {
        this.source = source;
        this.profile = profile;

        //new addition for testing Speed's custom damage value
        this.customDamage = -1.0f; // Default value for custom damage

    }
}
