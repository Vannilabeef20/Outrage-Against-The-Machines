using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(fileName = "PlayerAttack", menuName = "Player/Attack")]
    public class PlayerAttackSO : ScriptableObject
    {
        [field: SerializeField] public InputActionAsset PlayerActionMap { get; private set; }
        [field: SerializeField] public AnimationClip Animation { get; private set; }
        [field: SerializeField] public AudioClip Sound { get; private set; }
        [field: SerializeField] public float[] AudioPitches { get; private set; } = {1};
        [field: SerializeField] public AnimationCurve VelocityCurve { get; private set; }
        [field: SerializeField] public float MaxVelocity { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float StunDuration { get; private set; }
        [field: SerializeField] public float KnockbackStrenght { get; private set; }

    }
}

