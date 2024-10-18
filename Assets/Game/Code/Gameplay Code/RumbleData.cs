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
		[field: SerializeField, Min(0)] public float Duration { get; private set; }
		[field: SerializeField] public AnimationCurve LowCurve { get; private set; }
		[field: SerializeField] public AnimationCurve HighCurve { get; private set; }
	}
}