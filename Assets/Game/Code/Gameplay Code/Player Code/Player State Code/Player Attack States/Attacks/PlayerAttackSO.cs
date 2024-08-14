using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [CreateAssetMenu(fileName = "PlayerAttack", menuName = "Player/Attack")]
    public class PlayerAttackSO : ScriptableObject
    {
        [SerializeField, ShowAssetPreview(256,265)] Sprite preview;

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
        /// How fast the character is when the animation curve evaluates 1.
        /// </summary>
        [field: SerializeField] public float MaxVelocity { get; private set; }

        /// <summary>
        /// How long(seconds) the other entity will get stunned for after getting hit.
        /// </summary>
        [field: SerializeField, Min(0)] public float StunDuration { get; private set; }

        [field: SerializeField] public float KnockbackStrenght { get; private set; }


        [field: Header("SPECIAL"), HorizontalLine(2f, EColor.Orange)]
        [field: SerializeField] public bool IsSpecial { get; private set; }
        [field: SerializeField, ShowIf("IsSpecial")] public float SpecialCost { get; private set; }


        [field: Header("AUDIO"), HorizontalLine(2f, EColor.Yellow)]
        [field: SerializeField] public float[] AudioPitches { get; private set; } = { 1 };
        [field: SerializeField] public string EventParameter { get; private set; }
        [field: SerializeField] public string EventLabel { get; private set; }


        [field: Header("RUMBLE"), HorizontalLine(2f, EColor.Green)]
        [field: SerializeField, Range(0f, 1f)] public float RumbleLowFrequency { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float RumbleHighFrequency { get; private set; }
        [field: SerializeField, Min(0)] public float RumbleDuration { get; private set; }
    }
}

