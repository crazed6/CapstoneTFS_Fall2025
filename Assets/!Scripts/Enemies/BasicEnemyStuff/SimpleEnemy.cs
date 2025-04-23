using UnityEngine;

public class SimpleEnemy : BaseEnemy
{
    protected override void Idle()
    {
        base.Idle();
    }

    protected override void Patrol()
    {
        base.Patrol();
    }

    protected override void Chase()
    {
        base.Chase();
    }

    protected override void Attack()
    {
        base.Attack();
        Debug.Log($"{gameObject.name} performs a melee attack!");
    }
}
