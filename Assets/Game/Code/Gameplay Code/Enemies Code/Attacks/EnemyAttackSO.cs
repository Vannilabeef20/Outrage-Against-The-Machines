using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	[CreateAssetMenu(fileName = "EnemyAttackSO", menuName = "Misc/New EnemyAttackSO")]
	public class EnemyAttackSO : ScriptableObject
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
    }
}