using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class EnemyInterceptState : EnemyState
    {
        [field: Header("INTERCEPT STATE"), HorizontalLine(2f, EColor.Yellow)]
        public override string Name {get => "Intercept";}
        [Header("TARGETING"), HorizontalLine(2f, EColor.Green)]
        [SerializeField] private BaseTargeting targeting;
        [SerializeField, ReadOnly] private GameObject target;
        [SerializeField, Min(0f)] private float targetingRepeatInterval;
        [SerializeField, ReadOnly] private float targetingTimer;
        [Header("PATHFINDING"), HorizontalLine(2f, EColor.Blue)]
        [SerializeField] private BasePathfinding pathfinding;
        [SerializeField, Min(0f)] private float pathfindingRepeatInterval;
        [SerializeField, ReadOnly] private float pathfindingTimer;
        [Header("MOVEMENT"), HorizontalLine(2f, EColor.Indigo)]
        [SerializeField, ReadOnly] private Vector3 movementDirection;
        [SerializeField] private Vector3 speed;

        public override void Do()
        {
            ValidateState();
            targetingTimer += Time.deltaTime;
            pathfindingTimer += Time.deltaTime;
        }

        public override void FixedDo()
        {
            if(target == null)
            {
                return;
            }
            if (targetingTimer > targetingRepeatInterval)
            {
                target = targeting.GetTarget(stateMachine.body.position);
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
                movementDirection = pathfinding.GetMovementDirection(target.transform.position, stateMachine.IsOnScreen);
            }
            stateMachine.body.velocity = new Vector3(movementDirection.x * speed.x,
                movementDirection.y * speed.y, movementDirection.z * speed.z);
            Debug.DrawLine(stateMachine.body.position, stateMachine.body.position + (stateMachine.body.velocity.normalized * 2), Color.yellow);
        }

        public override void Enter()
        {
            stateMachine.animator.Play(StateAnimation.name);
            startTime = Time.time;
            target = targeting.GetTarget(stateMachine.body.position);
        }

        public override void Exit()
        {
            targetingTimer = 0;
        }

        protected override void ValidateState()
        {
            if(!stateMachine.IsOnScreen)
            {
                return;
            }
            EnemyAttack enemyAttack = stateMachine.attack.GetAppropriateAttack();
            if (enemyAttack != null)
            {
                stateMachine.attack.currentAttack = enemyAttack;
                stateMachine.nextState = stateMachine.attack;
                IsComplete = true;
                return;
            }
        }
    }
}
