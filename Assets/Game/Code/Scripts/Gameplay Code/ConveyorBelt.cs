using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class ConveyorBelt : MonoBehaviour
    {
        [field: SerializeField] public float ContextSpeedStrenght { get; private set; }
        [field: SerializeField, ReadOnly] public Vector3 ContextSpeed { get ; private set; }

        [SerializeField] private Transform FromPoint;
        [SerializeField] private Transform ToPoint;

        private void Awake()
        {
            CalculateContextSpeed();
        }
        private Vector3 CalculateContextSpeed()
        {
            ContextSpeed = (ToPoint.position - FromPoint.position).normalized * ContextSpeedStrenght;
            return ContextSpeed;
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
