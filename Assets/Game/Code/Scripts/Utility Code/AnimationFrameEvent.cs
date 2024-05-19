using System;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace Game
{
    [Serializable]
    public class AnimationFrameEvent
    {
        [field: SerializeField, ReadOnly, AllowNesting] public AnimationClip Anim { get; private set; }
        [field: SerializeField, ReadOnly, AllowNesting] public float Ratio { get; private set; }
        [field: SerializeField, ReadOnly, AllowNesting] public float TriggerTime { get; private set; }
        [field: SerializeField, ReadOnly, AllowNesting] public bool HasBeenTriggered { get; private set; }
        [field: SerializeField, Min(0)] public int TriggerFrame { get; private set; }
        [field: SerializeField] public UnityEvent Actions { get; private set; }

        public void Setup(AnimationClip clip, float animationDuration)
        {
            Anim = clip;
            Ratio = animationDuration / Anim.length;
            TriggerTime = TriggerFrame / Anim.frameRate * Ratio;
            HasBeenTriggered = false;
        }
        public void Reset()
        {
            HasBeenTriggered = false;
        }

        public void Update(float elapsedTime)
        {
            if (HasBeenTriggered)
            {
                return;
            }
            if (elapsedTime < TriggerTime)
            {
                return;
            }
            HasBeenTriggered = true;
            Actions.Invoke();
        }
    }
}