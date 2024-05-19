using System.Collections;
using UnityEngine;

namespace Game
{
    public interface IDamageble
    {
        public void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght);
    }
}
