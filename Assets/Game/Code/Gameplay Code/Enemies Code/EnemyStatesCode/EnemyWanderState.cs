using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
	public class EnemyWanderState : EnemyState
	{
		public override string Name { get => "Wander"; }

        [Header("WANDER STATE"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField] StudioEventEmitter movementEmitter;
        [Space]
        [SerializeField] int encounterNumber;
        [Space]
        [SerializeField] float speed;
        [SerializeField] float clearDistance;
        [SerializeField, MinMaxSlider(0f,5f)] Vector2 waitTime;
        [SerializeField] Transform[] wanderPoints;
        [Space]
        [SerializeField, ReadOnly] int currentPointIndex;
        [SerializeField, ReadOnly] float distance;
        [Space]
        [SerializeField, ReadOnly] bool waiting;
        [SerializeField, ReadOnly] float waitTimer;


        Rigidbody Body => stateMachine.body;

#if UNITY_EDITOR
        [Header("DEBUG (THIS WILL BE STRIPPED ON BUILD"), HorizontalLine(2f, EColor.Green)]
        [SerializeField] Color arrowHeadColor;
        [SerializeField] Color arrowColor;
#endif



        public override void Do() 
        {
            if(waiting)
            {
                movementEmitter.Stop();
                waitTimer -= Time.deltaTime;
                if (waitTimer > 0) return;
                waiting = false;
                movementEmitter.Play();
            }
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

            if (distance <= clearDistance)
            {
                currentPointIndex++;
                waitTimer = UnityEngine.Random.Range(waitTime.x, waitTime.y);
                waiting = true;
            }

            if (currentPointIndex >= wanderPoints.Length) currentPointIndex = 0;

            if (waiting)
            {
                Body.velocity = Vector3.zero;
                return;
            }

            Body.velocity = (wanderPoints[currentPointIndex].position - Body.position).normalized * speed;

            Body.position = Body.position.ToXYY();
        }

        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            stateMachine.animator.Play(StateAnimation.name);
            movementEmitter.Play();
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