using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [CreateAssetMenu(fileName = "PlayerAttack", menuName = "Player/Attack")]
    public class PlayerAttackSO : ScriptableObject
    {
        [field: Header("PARAMETERS"), HorizontalLine(2f, EColor.Red)]
        [field: SerializeField] public AnimationClip Animation { get; private set; }

        [field: SerializeField] public float Damage { get; private set; }

        /// <summary>
        /// The desired lenght(seconds) for the attack.
        /// </summary>
        [field: SerializeField, Min(0)] public float Duration { get; private set; }

        /// <summary>
        /// Defines the motion of the character during the animation.
        /// </summary>
        [field: SerializeField] public AnimationCurve VelocityCurve { get; private set; }

        /// <summary>
        /// How fast the character when the animation curve evaluates 1.
        /// </summary>
        [field: SerializeField] public float MaxVelocity { get; private set; }

        /// <summary>
        /// How long(seconds) the other entity will get stunned for after getting hit.
        /// </summary>
        [field: SerializeField] public float StunDuration { get; private set; }

        [field: SerializeField] public float KnockbackStrenght { get; private set; }


        [field: Header("SPECIAL"), HorizontalLine(2f, EColor.Orange)]
        [field: SerializeField] public bool IsSpecial { get; private set; }
        [field: SerializeField, ShowIf("AtkIsSpecial")] public float SpecialCost { get; private set; }


        [field: Header("AUDIO"), HorizontalLine(2f, EColor.Yellow)]
        [field: SerializeField] public AudioClip Sound { get; private set; }
        [field: SerializeField] public float[] AudioPitches { get; private set; } = { 1 };


        [field: Header("RUMBLE"), HorizontalLine(2f, EColor.Green)]
        [field: SerializeField, Range(0f, 1f)] public float RumbleLowFrequency { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float RumbleHighFrequency { get; private set; }
        [field: SerializeField, Min(0)] public float RumbleDuration { get; private set; }
    }
}

