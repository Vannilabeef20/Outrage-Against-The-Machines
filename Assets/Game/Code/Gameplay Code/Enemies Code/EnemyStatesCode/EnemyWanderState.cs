using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class EnemyWanderState : EnemyState
	{
		public override string Name { get => "Wander"; }

        [SerializeField] int encounterNumber;
        [Space]
        [SerializeField] float speed;
        [Space]
        [SerializeField] Transform[] wanderPoints;
        [SerializeField, ReadOnly] int currentPointIndex;
        [Space]
        [SerializeField] float clearDistance;
        [SerializeField, ReadOnly] float distance;
        Rigidbody Body => stateMachine.body;

#if UNITY_EDITOR
        [Header("DEBUG (THIS WILL BE STRIPPED ON BUILD"), HorizontalLine]
        [SerializeField] Color arrowHeadColor;
        [SerializeField] Color arrowColor;
#endif



        public override void Do() 
        {
            if (wanderPoints[currentPointIndex].position.x + 0.1f < transform.position.x)
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else if (wanderPoints[currentPointIndex].position.x - 0.1f > transform.position.x)
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }


        public override void FixedDo()
        {
            distance = Vector3.Distance(Body.position, wanderPoints[currentPointIndex].position);

            if (distance <= clearDistance) currentPointIndex++;

            if (currentPointIndex >= wanderPoints.Length) currentPointIndex = 0;

            Body.velocity = (wanderPoints[currentPointIndex].position - Body.position).normalized * speed;

            Body.position = Body.position.ToXYY();
        }

        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            stateMachine.animator.Play(StateAnimation.name);
        }

        public override void Exit() { }
        protected override void ValidateState() { }

        public void SetChase(int encounter)
        {
            if (encounter != encounterNumber) return;

            stateMachine.nextState = stateMachine.intercept;
            IsComplete = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < wanderPoints.Length; i++)
            {
                if(wanderPoints[i] == wanderPoints[wanderPoints.Length - 1])
                {
                    Helper.DrawPointArrow(wanderPoints[i].position, wanderPoints[0].position, arrowColor, arrowHeadColor);
                }
                else
                Helper.DrawPointArrow(wanderPoints[i].position, wanderPoints[i + 1].position, arrowColor, arrowHeadColor);
            }
        }
#endif
    }
}