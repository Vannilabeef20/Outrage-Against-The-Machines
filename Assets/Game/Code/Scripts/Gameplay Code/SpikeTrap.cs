using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class SpikeTrap : MonoBehaviour
    {
        [SerializeField] private Animator[] animators;
        [SerializeField] private AnimationClip spikeAnimation;
        [SerializeField] private float animationDuration;
        [SerializeField, ReadOnly] private float timer;
        [SerializeField] private AnimationFrameEvent[] frameEvents;

        private void Awake()
        {
            foreach(var frameEvent in frameEvents)
            {
                frameEvent.Setup(spikeAnimation, animationDuration);
            }
        }
        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > animationDuration)
            {
                timer = 0;
                foreach (var frameEvent in frameEvents)
                {
                    frameEvent.Reset();
                }
            }
            foreach (var frameEvent in frameEvents)
            {
                frameEvent.Update(timer);
            }
            foreach (var animator in animators)
            {
                animator.Play(spikeAnimation.name, 0, timer.Map(0, animationDuration));
            }
        }

    }
}
