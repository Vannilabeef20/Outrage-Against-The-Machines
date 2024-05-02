using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class MoveRangedCntxStrg : MoveContextSteering
	{
        [SerializeField] private float distanceToKeep;
        [SerializeField] private float distanceRange;
        [SerializeField, ReadOnly] private float distance;
        [SerializeField, ReadOnly] private Vector3 direction;
        public override Vector3 CalculateTargetDirection(Vector3 targetPosition)
        {
            distance = Vector3.Distance(body.position, targetPosition);
            if (distance > distanceToKeep)
            {
                //Moving towards
                direction = targetPosition - body.position;
            }
            else if(distance < distanceToKeep - distanceRange)
            {
                //Moving away
                direction = -(new Vector3(targetPosition.x, targetPosition.y, targetPosition.z) - body.position);
            }
            else
            {
                //Inside range
                direction = new Vector3(body.position.x, targetPosition.z, targetPosition.z) - body.position;
            }
            return direction;
        }
    }
}