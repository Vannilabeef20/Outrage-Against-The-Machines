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
		[field: SerializeField, ReadOnly, AllowNesting] public EPlayer Target { get; private set; }

		[field: SerializeField, ReadOnly] public bool IsRealtime { get; private set; }
		[ReadOnly] public bool isPaused;
		[SerializeField, ReadOnly, AllowNesting] float timer;
		[SerializeField, ReadOnly, AllowNesting] float duration;
		[SerializeField, ReadOnly, AllowNesting, Range(0, 1)] float progress;

		[SerializeField, ReadOnly, AllowNesting] AnimationCurve lowCurve;
		[SerializeField, ReadOnly, AllowNesting] AnimationCurve highCurve;

		[field: ReadOnly] public float LowFreq { get; private set; }
		[field: ReadOnly] public float HighFreq { get; private set; }
		[field: ReadOnly] public bool Done { get; private set; }

		public Rumble(string name, AnimationCurve lowCurve, AnimationCurve highCurve, float duration, EPlayer target, bool realtime)
        {
            this.Name = name;
            this.lowCurve = lowCurve;
            this.highCurve = highCurve;
            this.duration = duration;
            this.Target = target;
			this.IsRealtime = realtime;
        }

        public void Sample(float deltaTime)
        {
			timer += deltaTime;

			progress = timer.Map(0, duration);

			LowFreq = lowCurve.Evaluate(progress);
			HighFreq = highCurve.Evaluate(progress);

			if (timer >= duration) Done = true;
		}
	}
}