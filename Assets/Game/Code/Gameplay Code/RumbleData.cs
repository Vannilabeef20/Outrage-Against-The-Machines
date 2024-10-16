using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	[Serializable]
	public class RumbleData
	{
		[field: SerializeField] public bool Realtime { get; private set; }
		[field: SerializeField] public EPlayer Target { get; private set; }
		[field: SerializeField, Min(0)] public float Duration { get; private set; }
		[field: SerializeField] public AnimationCurve LowCurve { get; private set; }
		[field: SerializeField] public AnimationCurve HighCurve { get; private set; }

	}

    [Flags]
	public enum EPlayer
    {
		All = -1,
		Player_1,
		Player_2,
		Player_3,
    }
}