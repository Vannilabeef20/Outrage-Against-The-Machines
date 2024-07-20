using System.Collections;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Mediates damage dealing.
    /// </summary>
    public interface IDamageble
    {
        /// <summary>
        /// Deals damage and applies knockback to this entity.
        /// </summary>
        /// <param name="damageDealerPos">The position of the one dealing the damage.<br/>
        /// Necessary for calculating the knockback direction.</param>
        /// <param name="damage">How many damage points will be dealt to this entity.</param>
        /// <param name="stunDuration">The amount of seconds this entity will be stunned for.</param>
        /// <param name="knockbackStrenght">How fast this entity will move during the stun.</param>
        public void TakeDamage(Vector3 damageDealerPos, float damage, float stunDuration, float knockbackStrenght);
    }
}
