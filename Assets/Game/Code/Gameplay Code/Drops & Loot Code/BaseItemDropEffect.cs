using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Abstract class to be inherited by all Item Drop Effects.
    /// <para>Can be used to select any child class.</para>
    /// </summary>
    [System.Serializable]
    public abstract class BaseItemDropEffect
    {
        /// <summary>
        /// Applies its corresponding pickup effect to the "targetPlayer".
        /// </summary>
        /// <param name="targetPlayer"></param>
        public abstract void ApplyEffect(GameObject targetPlayer);
    }
}