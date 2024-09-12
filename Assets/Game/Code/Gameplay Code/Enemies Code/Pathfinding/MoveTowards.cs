using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class MoveTowards : BasePathfinding
    {
        public override void Setup()
        {

        }
        public override void OnGizmo()
        {

        }
        public override Vector3 GetMovementDirection(Vector3 targetPosition, bool IsOnScreen)
        {
           return (targetPosition - body.position).normalized;
        }
    }
}
