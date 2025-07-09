using UnityEngine;

public enum DamageType
{
    Melee,
    Ranged,
    Explosion,
    Trap
}

public struct DamageData
{
    public GameObject source;
    public DamageProfile profile; //DamageProfile is a ScriptableObject that contains damage type and amount

    public DamageData(GameObject source, DamageProfile profile)
    {
        this.source = source;
        this.profile = profile;

    }
}
