using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSFEffectExample : MonoBehaviour
{
    public MSFEffectsGroup group;
    public int CurIndex = 0;
    public ParticleSystem CurParticle;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            CurIndex -= 1;
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            CurIndex += 1;
        }
        if(CurIndex >= group.Particles.Count)
        {
            CurIndex = 0;
        }
        if(CurIndex < 0)
        {
            CurIndex = group.Particles.Count - 1;
        }
        if(Input.GetKeyDown(KeyCode.Space)|| (Input.GetMouseButtonDown(0)))
        {
            PlayEffects();
        }
    }
    public void PlayEffects()
    {
        if(CurParticle!=null)
        {
            Destroy(CurParticle.gameObject);
        }
        CurParticle = Instantiate(group.Particles[CurIndex]);
    }
}
