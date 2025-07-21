//Ritwik -_-
using UnityEngine;

public struct KnockbackData
{
    public Vector3 sourcePosition;
    public float force;
    public float duration;
    public float upwardForce;
    public bool overrideVelocity;

    public KnockbackData(Vector3 source, float force, float duration, float upwardForce = 0.5f, bool overrideVel = true)
    {
        this.sourcePosition = source;
        this.force = force;
        this.duration = duration;
        this.upwardForce = upwardForce;
        this.overrideVelocity = overrideVel;
    }
}
