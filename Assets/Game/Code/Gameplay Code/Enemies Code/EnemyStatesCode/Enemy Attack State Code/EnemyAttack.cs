using System;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [Serializable]
    public class EnemyAttack
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AnimationClip Animation { get; private set; }
        [field: SerializeField] public float TriggerRange { get; private set; }
        [field: SerializeField] public AnimationCurve VelocityCurve { get; private set; }
        [field: SerializeField] public float Velocity { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float StunDuration { get; private set; }
        [field: SerializeField] public float KnockbackStrenght { get; private set; }
        [field: SerializeField] public bool HasProjectile { get; private set; }
        [field: SerializeField, ShowIf("HasProjectile"), AllowNesting] public GameObject ProjectilePrefab { get; private set; }
        [field: SerializeField, ShowIf("HasProjectile"), AllowNesting] public Transform ProjectileSpawnTransform { get; private set; }
        [field: SerializeField] public AnimationFrameEvent[] FrameEvents { get; private set; }

        public void SetupFrameEvents(AnimationClip clip)
        {
            foreach (AnimationFrameEvent frameEvent in FrameEvents)
            {
                frameEvent.Setup(clip, Duration);
            }
        }
    }
}