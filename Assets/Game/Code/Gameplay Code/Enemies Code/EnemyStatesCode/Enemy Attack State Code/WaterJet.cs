using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class WaterJet : MonoBehaviour
	{
        [SerializeField] Animator animator;
		[SerializeField] AnimationClip jetClip;
		[SerializeField] float duration;
        [SerializeField, ReadOnly] bool playing;
        [SerializeField, ReadOnly] float timer;
        [SerializeField, Range(0f,1f), ReadOnly] float completion;

        private void Update()
        {
            if (!playing) return;

            timer += Time.deltaTime;
            completion = timer.Map(0, duration);
            animator.Play(jetClip.name, 0, completion);
        }
        public void Play()
        {
            playing = true;
            timer = 0f;
            completion = 0f;
        }

        public void Stop()
        {
            playing = false;
            animator.Play(jetClip.name, 0, 1);
        }
    }
}