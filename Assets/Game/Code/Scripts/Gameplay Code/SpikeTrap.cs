using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [SelectionBase]
    public class SpikeTrap : MonoBehaviour
    {
        [SerializeField] Animator[] animators;
        [SerializeField] AnimationClip spikeAnimation;
        [SerializeField] float animationDuration;
        [SerializeField, ReadOnly] float timer;
        [SerializeField] AnimationFrameEvent[] frameEvents;
        [SerializeField] float activationRange;

        private void Awake()
        {
            foreach(var frameEvent in frameEvents)
            {
                frameEvent.Setup(spikeAnimation, animationDuration);
            }
        }
        private void Update()
        {
            //Return and pause animation if not within the activation range
            if (Mathf.Abs(GameManager.Instance.MainCamera.transform.position.x - transform.position.x) > activationRange)
            {
                foreach (var animator in animators)
                {
                    animator.speed = 0;
                }
                return;
            }
            else
            {
                foreach (var animator in animators)
                {
                    animator.speed = 1;
                }
            }
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

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            #region DRAW ACTIVATION RANGE ZONE
            Vector3 point1Pos = Vector3.zero;
            Vector3 point2Pos = Vector3.zero;

            point1Pos.x = transform.position.x - activationRange;
            point2Pos.x = transform.position.x - activationRange;
            point1Pos.y = transform.position.y + 7;
            point2Pos.y = transform.position.y - 7;
            point1Pos.z = transform.position.z;
            point2Pos.z = transform.position.z;
            Debug.DrawLine(point1Pos, point2Pos, Color.yellow); //Activation range Left Line


            point1Pos.x = transform.position.x + activationRange;
            point2Pos.x = transform.position.x + activationRange;
            point1Pos.y = transform.position.y + 7;
            point2Pos.y = transform.position.y - 7;
            point1Pos.z = transform.position.z;
            point2Pos.z = transform.position.z;
            Debug.DrawLine(point1Pos, point2Pos, Color.yellow); //Activation range RightLine

            point1Pos = transform.position;
            point2Pos = transform.position;
            point1Pos.x = transform.position.x + activationRange;
            point2Pos.x = transform.position.x - activationRange;
            Debug.DrawLine(point1Pos, point2Pos, Color.green); //Activation range MiddleLine
            #endregion
        }
#endif
    }
}

