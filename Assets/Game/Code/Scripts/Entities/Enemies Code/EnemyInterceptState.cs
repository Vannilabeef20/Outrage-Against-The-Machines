using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class EnemyInterceptState : EnemyState
    {
        public override string Name {get => "Intercept";}
        [Header("TARGET SORTING")]
        [SerializeField] private BaseTargeting targeting;
        [SerializeField, ReadOnly] private GameObject target;
        [SerializeField] private float sortingRepeatInterval;
        [SerializeField, ReadOnly] private float sortingTimer;
        [Header("MOVEMENT")]
        [SerializeField] private BasePathfinding pathfinding;
        [SerializeField, ReadOnly] private Vector3 movementDirection;
        [SerializeField] private Vector3 speed;

        public override void Do()
        {
            ValidateState();
            sortingTimer += Time.deltaTime;          
        }

        public override void FixedDo()
        {
            if (sortingTimer > sortingRepeatInterval)
            {
                target = targeting.GetTarget(stateMachine.body.position,
                    GameManager.Instance.PlayerObjectArray);
                sortingTimer = 0;
            }
            if (target.transform.position.x < transform.position.x)
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            movementDirection = pathfinding.GetMovementDirection(target.transform.position);
            stateMachine.body.velocity = new Vector3(movementDirection.x * speed.x,
                movementDirection.y * speed.y, movementDirection.z * speed.z);
        }

        public override void Enter()
        {
            stateMachine.animator.Play(StateAnimation.name);
            startTime = Time.time;
            target = targeting.GetTarget(stateMachine.body.position,
                GameManager.Instance.PlayerObjectArray);
        }

        public override void Exit()
        {
            sortingTimer = 0;
        }

        protected override void ValidateState()
        {
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
