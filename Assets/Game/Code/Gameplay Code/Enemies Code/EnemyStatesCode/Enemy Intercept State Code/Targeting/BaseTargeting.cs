using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Class to be inherited by all enemy targeting behaviour scripts.
    /// <para>Can be used to select any of its child classes with the "SubclassSelector" property drawer.</para>
    /// </summary>
    [System.Serializable]
    public abstract class BaseTargeting
    {
        /// <summary>
        /// Gets the appropiate player target based on the current target sorting implementation.
        /// </summary>
        /// <param name="currentPosition">This entity's current position.</param>
        /// <returns>A target GameObject.</returns>
        public abstract Transform GetTarget(Vector3 currentPosition);
    }
}
