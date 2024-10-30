using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    public class MK2InterceptState : BossState
    {
        public override string Name { get => "MK2 Intercept"; }

        [Header("INTERCEPT STATE"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField] StudioEventEmitter movementEmitter;

        [field: Header("PATHFINDING"), HorizontalLine(2f, EColor.Blue)]
        [field: SerializeReference, SubclassSelector] public BasePathfinding PathfindingBehaviour { get; private set; }
        [Space]
        [SerializeField, Min(0f)] float pathfindingRepeatInterval;
        [SerializeField, ReadOnly] float pathfindingTimer;

        [Header("MOVEMENT"), HorizontalLine(2f, EColor.Indigo)]
        [SerializeField] Vector3 speed;
        [SerializeField, ReadOnly] Vector3 movementDirection;

        private void Awake()
        {
            if (PathfindingBehaviour != null)
            {
                PathfindingBehaviour.Setup();
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (PathfindingBehaviour != null)
            {
                PathfindingBehaviour.OnGizmo();
            }
        }
#endif
        public override void Enter()
        {
            IsComplete = false;
            stateMachine.animator.Play(StateAnimation.name);
            startTime = Time.time;
            movementEmitter.Play();
        }
        public override void Do()
        {
            ValidateState();
            pathfindingTimer += Time.deltaTime;
        }

        public override void FixedDo()
        {
            if (stateMachine.Target == null) return;

            //Flip
            if (stateMachine.Target.transform.position.x + 0.1f < transform.position.x)
            {
                stateMachine.Parent.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else if (stateMachine.Target.transform.position.x - 0.1f > transform.position.x)
            {
                stateMachine.Parent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }

            //Refresh Path
            if (pathfindingTimer >= pathfindingRepeatInterval)
            {
                movementDirection = PathfindingBehaviour.GetMovementDirection
                    (stateMachine.Target.transform.position, stateMachine.IsInsidePlayzone);
                pathfindingTimer = 0;
            }

            //Set Speed
            stateMachine.body.velocity = new Vector3(movementDirection.x * speed.x,
                movementDirection.y * speed.y, movementDirection.z * speed.z) + stateMachine.ContextVelocity;

#if UNITY_EDITOR
            //Draw velocity line
            Debug.DrawLine(stateMachine.body.position, stateMachine.body.position + (stateMachine.body.velocity.normalized * 2), Color.yellow);
#endif
        }

        public override void Exit()
        {
            movementEmitter.Stop();
        }

        protected override void ValidateState()
        {
            //if (!stateMachine.IsInsidePlayzone) return;

            if (!stateMachine.mk2Attack.CheckForAndSetAttack()) return;

            stateMachine.nextState = stateMachine.mk2Attack;
            IsComplete = true;
            return;
        }
    }
}
