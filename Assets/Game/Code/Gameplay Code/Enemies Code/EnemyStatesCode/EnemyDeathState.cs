using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class EnemyDeathState : EnemyState
    {
        public override string Name { get => "Death"; }
        [field: Header("DEATH STATE"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField] private float duration;
        [SerializeField] private AnimationFrameEvent[] frameEvents;
        [SerializeField] private LootTable lootTable;

        private void Start()
        {
            foreach (var frame in frameEvents)
            {
                frame.Setup(StateAnimation, duration);
            }
            lootTable.ValidateTable();
        }
        public override void Do()
        {
            progress = UpTime.Map(0, duration);
            stateMachine.animator.Play(StateAnimation.name, 0, progress);
            foreach (var frame in frameEvents)
            {
                frame.Update(UpTime);
            }
            ValidateState();
        }

        public override void FixedDo()
        {
            stateMachine.body.velocity = stateMachine.ContextVelocity;
        }

        public override void Enter()
        {
            IsComplete = false;
            startTime = Time.time;
            stateMachine.attackhitbox.enabled = false;
            stateMachine.hurtBox.enabled = false;
            stateMachine.animator.Play(StateAnimation.name);
        }

        public override void Exit() { }

        protected override void ValidateState()
        {
            if (UpTime <= StateAnimation.length) return;

            IsComplete = true;
            stateMachine.nextState = stateMachine.intercept;
            GameObject drop = lootTable.PickRandomDrop();
            if (drop != null)
            {
                Instantiate(drop, stateMachine.transform.position, Quaternion.identity);
            }
            foreach (var frame in frameEvents)
            {
                frame.Reset();
            }
            Destroy(stateMachine.gameObject);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            lootTable.ValidateTable();
        }
#endif
    }
}
