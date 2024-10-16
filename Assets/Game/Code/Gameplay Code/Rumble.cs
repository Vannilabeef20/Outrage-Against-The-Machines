using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	[Serializable]
	public class Rumble
	{
		[field: SerializeField, ReadOnly, AllowNesting] public string Name { get; private set; }

		[SerializeField, ReadOnly, AllowNesting] bool realtime;
		[SerializeField, ReadOnly, AllowNesting] AnimationCurve lowCurve;
		[SerializeField, ReadOnly, AllowNesting] AnimationCurve highCurve;
		[SerializeField, ReadOnly, AllowNesting, Min(0)] float duration;
		[SerializeField, ReadOnly, AllowNesting] float timer;
		[SerializeField, ReadOnly, AllowNesting, Range(0, 1)] float progress;
		[field: SerializeField, ReadOnly, AllowNesting] public EPlayer Target { get; private set; }
		[field: ReadOnly] public float LowFreq { get; private set; }
		[field: ReadOnly] public float HighFreq { get; private set; }
		[field: ReadOnly] public bool Done { get; private set; }

		public Rumble(string name, AnimationCurve lowCurve, AnimationCurve highCurve, float duration, EPlayer target)
        {
            this.Name = name;
            this.lowCurve = lowCurve;
            this.highCurve = highCurve;
            this.duration = duration;
            Target = target;
        }

        public void Sample(float deltaTime)
        {
			timer += deltaTime;

			if (timer >= duration)
            {
				Done = true;
				return;
            }

			progress = timer.Map(0, duration);

			LowFreq = lowCurve.Evaluate(progress);
			HighFreq = highCurve.Evaluate(progress);
		}
	}
}