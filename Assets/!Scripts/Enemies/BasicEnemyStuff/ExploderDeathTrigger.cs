using UnityEngine;

[RequireComponent(typeof(EnemyDamageComponent))]
public class ExploderDeathTrigger : MonoBehaviour
{
    EnemyDamageComponent damageComponent;

    private void Start()
    {
        damageComponent = GetComponent<EnemyDamageComponent>();

        damageComponent.OnDeath += HandleDeath;
    }

    private void HandleDeath()
    {
       
    }
}
