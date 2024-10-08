using System;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [Serializable]
    public class EnemyAttack
    {
        [field: SerializeField, Expandable] public EnemyAttackSO Config { get; private set; }
        [field: SerializeField] public AnimationFrameEvent[] FrameEvents { get; private set; }

        public void SetupFrameEvents(AnimationClip clip)
        {
            foreach (AnimationFrameEvent frameEvent in FrameEvents)
            {
                frameEvent.Setup(clip, Config.Duration);
            }
        }
    }
}