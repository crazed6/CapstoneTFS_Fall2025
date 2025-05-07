using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MasterStylizedFX/EffectGroup")]
public class MSFEffectsGroup : ScriptableObject
{
    public List<ParticleSystem> Particles;
}
