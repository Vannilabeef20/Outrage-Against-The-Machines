using Game;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class DamageRelay : MonoBehaviour, IDamageble
{
    [SerializeField, Required] GameObject target;
    public void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght)
    {
        target.GetComponent<IDamageble>().TakeDamage(damageDealerPos, damage, stunDuration, knockbackStrenght);
    }
}
