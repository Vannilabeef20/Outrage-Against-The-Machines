using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class EnemyDeathState : EnemyState
    {
        public override string Name { get => "Death"; }

        [field: Header("DEATH STATE"), HorizontalLine(2f, EColor.Yellow)]

        [SerializeField] float duration;
        [SerializeField] AnimationFrameEvent[] frameEvents;
        [SerializeField] LootTable lootTable;

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
            MachineAnimator.Play(StateAnimation.name, 0, progress);
            foreach (var frame in frameEvents)
            {
                frame.Update(UpTime);
            }
            ValidateState();
        }

        public override void FixedDo()
        {
            Velocity = stateMachine.ContextVelocity;
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
            if (UpTime <= duration) return;

            IsComplete = true;
            NextState = stateMachine.intercept;
            GameObject drop = lootTable.PickRandomDrop();
            if (drop != null)
            {
                Instantiate(drop, MachinePosition, Quaternion.identity);
            }
            foreach (var frame in frameEvents)
            {
                frame.Reset();
            }
            if (Parent == null) this.LogError($"{stateMachine.name}'s parent object is null");
            Destroy(Parent);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            lootTable.ValidateTable();
        }
#endif
    }
}
