using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public abstract class BaseState : MonoBehaviour
    {
        [field: Header("BASE STATE INHERITED"), HorizontalLine(2f, EColor.Red)]

        [SerializeField, TextArea] private string Comment;
        [field: SerializeField, ReadOnly] public bool IsComplete { get; protected set; }
        public virtual string Name { get => GetType().Name; private set { value = GetType().Name; } }

        [SerializeField, ReadOnly] protected float startTime;

        [field: SerializeField] protected AnimationClip StateAnimation { get; set; }
        protected float UpTime => Time.time - startTime;

        [SerializeField, ReadOnly, Range(0f,1f)] protected float progress;

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Do();
        public abstract void FixedDo();
        protected abstract void ValidateState();

    }
}
