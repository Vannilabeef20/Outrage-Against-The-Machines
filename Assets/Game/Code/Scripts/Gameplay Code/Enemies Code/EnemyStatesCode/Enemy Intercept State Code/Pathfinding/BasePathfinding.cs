using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public abstract class BasePathfinding : MonoBehaviour
    {
        [SerializeField] protected Rigidbody body;
        [SerializeField, ReadOnly] protected Vector3 targetPosition;
        public abstract Vector3 GetMovementDirection(Vector3 targetPosition ,bool IsOnScreen);
    }
}
