using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    public class MK1InterceptState : BossState
    {
        public override string Name { get => "MK1 Intercept"; }

        [Header("INTERCEPT STATE"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField] float duration = 1;

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
            if(PathfindingBehaviour != null)
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
            startTime = Time.time;
            movementEmitter.Play();
        }
        public override void Do()
        {
            pathfindingTimer += Time.deltaTime;
            progress = UpTime.Map(0, duration);
            stateMachine.animator.Play(StateAnimation.name, 0, progress);
            ValidateState();
        }

        public override void FixedDo()
        {
            if (stateMachine.Target == null) return;

            stateMachine.Flip();

            //Refresh Path
            if(pathfindingTimer >= pathfindingRepeatInterval)
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
            if (stateMachine.mk1Attack.CheckForAndSetAttack())
            {
                stateMachine.nextState = stateMachine.mk1Attack;
                IsComplete = true;
            }
            else if (UpTime >= duration)
            {
                stateMachine.nextState = stateMachine.mk1Intercept;
                IsComplete = true;
            }
        }
    }
}
