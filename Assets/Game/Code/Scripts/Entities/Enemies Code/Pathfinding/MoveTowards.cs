using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MoveTowards : BasePathfinding
    {
        public override Vector3 GetMovementDirection(Vector3 targetPosition)
        {
           return (targetPosition - body.position).normalized;
        }
    }
}
