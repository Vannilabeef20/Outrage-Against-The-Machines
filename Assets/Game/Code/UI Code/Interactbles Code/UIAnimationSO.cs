using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "UIAnimationSO", menuName = "Misc/New UIAnimationSO")]
	public class UIAnimationSO : ScriptableObject
	{
		[field: SerializeField] public AnimationCurve SelectCurve {get; private set;}
		[field: SerializeField] public float SelectDuration	{get; private set;} = 0.25f;
		[field: SerializeField] public AnimationCurve ClickCurve {get; private set;}
		[field: SerializeField] public float ClickDuration {get; private set;} = 0.25f;
	}
}