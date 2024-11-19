using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class EletricTrap : MonoBehaviour
	{

        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]

        [SerializeField] Transform mainCamTransform;
        [SerializeField] Animator animator;
        [SerializeField] AnimationClip trapAnimation;


        [Header("PARAMETERS"), HorizontalLine(2f, EColor.Orange)]

        [SerializeField] float activationRange;
        [SerializeField] float animationDuration;
        [SerializeField] AnimationFrameEvent[] frameEvents;


        [Header("VARIABLES"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField, ReadOnly] float timer;

        private void Awake()
        {
            mainCamTransform = Camera.main.transform;
            foreach (var frameEvent in frameEvents)
            {
                frameEvent.Setup(trapAnimation, animationDuration);
            }
        }
        private void Update()
        {
            //Return and pause animation if not within the activation range
            if (Mathf.Abs(mainCamTransform.position.x - transform.position.x) > activationRange)
            {
                animator.speed = 0;
                return;
            }

            animator.speed = 1;

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
            animator.Play(trapAnimation.name, 0, timer.Map(0, animationDuration));
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