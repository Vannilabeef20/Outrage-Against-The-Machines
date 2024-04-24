using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class EnemyDeathState : EnemyState
    {
        public override string Name { get => "Death"; }
        [field: Header("DEATH STATE"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private LootTable lootTable;

        private void Start()
        {
            lootTable.ValidateTable();
        }
        public override void Do()
        {
            ValidateState();
        }

        public override void FixedDo()
        {

        }

        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            stateMachine.attackhitbox.enabled = false;
            stateMachine.hurtBox.enabled = false;
            stateMachine.audioSource.PlayOneShot(deathSound);
            stateMachine.animator.Play(StateAnimation.name);
        }

        public override void Exit()
        {
            GameObject drop = lootTable.PickRandomDrop();
            if(drop != null)
            {
                Instantiate(drop, stateMachine.transform.position, Quaternion.identity);
            }
            stateMachine.gameObject.SetActive(false);
        }

        protected override void ValidateState()
        {
            if(UpTime > StateAnimation.length)
            {
                stateMachine.nextState = stateMachine.intercept;
                IsComplete = true;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            lootTable.ValidateTable();
        }
#endif
    }
}
