using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class EnemyInterceptState : EnemyState
    {
#pragma warning disable
        [field: Header("INTERCEPT STATE"), HorizontalLine(2f, EColor.Yellow)]
#pragma warning restore
        public override string Name {get => "Intercept";}
        [Header("TARGETING"), HorizontalLine(2f, EColor.Green)]
        [SerializeReference, SubclassSelector] BaseTargeting targetingBehaviour;
        [SerializeField, ReadOnly] Transform target;
        [SerializeField, Min(0f)] float targetingRepeatInterval;
        [SerializeField, ReadOnly] float targetingTimer;

        [field: Header("PATHFINDING"), HorizontalLine(2f, EColor.Blue)]
        [SerializeField, Min(0f)] float pathfindingRepeatInterval;
        [SerializeField, ReadOnly] float pathfindingTimer;
        [field: SerializeReference, SubclassSelector] public BasePathfinding PathfindingBehaviour { get; private set; }

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
            stateMachine.animator.Play(StateAnimation.name);
            startTime = Time.time;
            target = targetingBehaviour.GetTarget(stateMachine.body.position);
        }
        public override void Do()
        {
            ValidateState();
            targetingTimer += Time.deltaTime;
            pathfindingTimer += Time.deltaTime;
        }

        public override void FixedDo()
        {
            if (target == null) return;

            if (targetingTimer > targetingRepeatInterval)
            {
                target = targetingBehaviour.GetTarget(stateMachine.body.position);
                targetingTimer = 0;
            }
            if (target.transform.position.x + 0.1f < transform.position.x)
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else if (target.transform.position.x - 0.1f > transform.position.x)
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            if(pathfindingTimer >= pathfindingRepeatInterval)
            {
                movementDirection = PathfindingBehaviour.GetMovementDirection(target.transform.position, stateMachine.IsInsidePlayZone);
            }
            stateMachine.body.velocity = new Vector3(movementDirection.x * speed.x,
                movementDirection.y * speed.y, movementDirection.z * speed.z) + stateMachine.ContextVelocity;
            Debug.DrawLine(stateMachine.body.position, stateMachine.body.position + (stateMachine.body.velocity.normalized * 2), Color.yellow);
        }

        public override void Exit()
        {
            targetingTimer = 0;
        }

        protected override void ValidateState()
        {
            if (!stateMachine.IsInsidePlayZone) return;

            if (!stateMachine.attack.CheckForAttacks()) return;

            stateMachine.nextState = stateMachine.attack;
            IsComplete = true;
            return;
        }
    }
}
