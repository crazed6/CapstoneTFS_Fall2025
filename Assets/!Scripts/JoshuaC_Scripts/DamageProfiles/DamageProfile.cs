using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageProfile", menuName = "Combat/Damage Profile", order = 0)]

public class DamageProfile : ScriptableObject
{

    public DamageType damageType;
    public int damageAmount;

}
