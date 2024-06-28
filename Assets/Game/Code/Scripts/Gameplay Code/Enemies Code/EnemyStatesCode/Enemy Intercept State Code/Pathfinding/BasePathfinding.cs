using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [System.Serializable]
    public abstract class BasePathfinding
    {
        [Header("BASE PATHFINDING INHERITED"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] protected Rigidbody body;
        [SerializeField, AllowNesting, ReadOnly] protected Vector3 targetPosition;

        /// <summary>
        /// Makes the necessary initial calculus for the pathfinding to function properly.
        /// <para>Should be placed either on Awake or Start.</para>
        /// </summary>
        public abstract void Setup();

        /// <summary>
        /// Draws all implemented debug related visuals (Debug, Gizmos, Handles).
        /// <para>Should be placed either on OnDrawGizmos or OndrawGizmosSelected.</para>
        /// </summary>
        public abstract void OnGizmo();

        /// <summary>
        /// Gets the appropriate movement direction for the enemy to move and get to a player position.
        /// </summary>
        /// <param name="targetPosition">The current target player position.</param>
        /// <param name="IsOnScreen">Whether this enemy is on screen.<br/>
        /// Prevents certain behaviours to run while outside view.</param>
        /// <returns>The direction to a player.</returns>
        public abstract Vector3 GetMovementDirection(Vector3 targetPosition ,bool IsOnScreen);
    }
}
