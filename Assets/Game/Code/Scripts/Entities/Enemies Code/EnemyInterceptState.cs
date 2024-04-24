using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class EnemyInterceptState : EnemyState
    {
        public override string Name {get => "Intercept";}
        [Header("TARGET SORTING")]
        [SerializeField, ReadOnly] private GameObject[] playerGameobjects;
        [SerializeField, ReadOnly] private GameObject closestPlayer;
        [SerializeField] private float sortingRepeatInterval;
        [SerializeField, ReadOnly] private float sortingTimer;
        [Header("MOVEMENT")]
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
                FindNearstPlayer();
                sortingTimer = 0;
            }
            if (closestPlayer.transform.position.x < transform.position.x)
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            movementDirection = closestPlayer.transform.position - transform.position;
            movementDirection.Normalize();
            stateMachine.body.velocity = new Vector3(movementDirection.x * speed.x,
                movementDirection.y * speed.y, movementDirection.z * speed.z);
        }

        public override void Enter()
        {
            stateMachine.animator.Play(StateAnimation.name);
            startTime = Time.time;
            FindNearstPlayer();
            InvokeRepeating(nameof(FindNearstPlayer), 0, sortingRepeatInterval);
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

        private void FindNearstPlayer()
        {
            playerGameobjects = GameManager.Instance.PlayerObjectArray;
            playerGameobjects = playerGameobjects.OrderBy(player => Vector3.Distance
            (stateMachine.transform.position, player.transform.position)).ToArray<GameObject>();
            closestPlayer = playerGameobjects[0];
        }
    }
}
