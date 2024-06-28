using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Mimics a conveyor belt behaviour by calculating a force in a given direction.<br/>
    /// Other objects may take this force value and apply to their movement.
    /// </summary>
    public class ConveyorBelt : MonoBehaviour
    {
        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]

        [Tooltip("Point where the force comes from.")]
        [SerializeField] Transform FromPoint;

        [Tooltip("Point which the force leads to.")]
        [SerializeField] Transform ToPoint;

        [field: Header("PARAMETERS & VARIABLES"), HorizontalLine(2f, EColor.Orange)]
        [Tooltip("How strong the force applied will be.")]
        [field: SerializeField] float ContextSpeedStrenght;

        [Tooltip("The direction of the force.\n" +
            "Calculated based on the direction between FromPoint and ToPoint.")]
        [SerializeField, ReadOnly] Vector3 ContextSpeedDirection;

        /// <summary>
        /// The actual resulting force value.
        /// <para>Calculated by multiplying ContextSpeedDirection and ContextSpeedStrenght.</para>
        /// </summary>
        [Tooltip("The actual resulting force value.\n" +
            "Calculated by multiplying ContextSpeedDirection and ContextSpeedStrenght.")]
        [field: SerializeField, ReadOnly] public Vector3 ContextSpeed { get; private set; }

        private void Awake()
        {
            CalculateContextSpeed();
        }
        /// <summary>
        /// Calculates the resulting speed entities whithin this gameobject's collider may apply to their movement.
        /// </summary>
        private void CalculateContextSpeed()
        {
            ContextSpeedDirection = (ToPoint.position - FromPoint.position).normalized;
            ContextSpeed = ContextSpeedDirection * ContextSpeedStrenght;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(ToPoint == null)
            {
                return;
            }
            if(FromPoint == null)
            {
                return;
            }
            Helper.DrawPointArrow(FromPoint.position, ToPoint.position, Color.blue, Color.magenta);
        }
        private void OnValidate()
        {
            CalculateContextSpeed();
        }
#endif
    }
}
