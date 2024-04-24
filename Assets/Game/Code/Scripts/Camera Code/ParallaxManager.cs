using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    public class ParallaxManager : MonoBehaviour
    {

        [SerializeField] private bool parallaxEnabled = true;
        [SerializeField, ReadOnly] private Transform cameraTransform;
        [Tooltip("Cached variable, how much the main camera has moved since the last LateUpdate.")]
        [SerializeField, ReadOnly] private Vector2 cameraDelta;
        [Tooltip("Cached variable.")]
        [SerializeField, ReadOnly] private Vector2 cameraCurrentPosition;
        [Tooltip("Camera position at the previous LateUpdate.")]
        [SerializeField, ReadOnly] private Vector2 cameraLastPosition;

        [SerializeField] private ApplyParallax[] parallaxes;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
            cameraLastPosition = cameraTransform.position;
        }

        private void LateUpdate()
        {
            if (!parallaxEnabled)
            {
                return;
            }
            cameraCurrentPosition = cameraTransform.position;
            cameraDelta = cameraCurrentPosition - cameraLastPosition;
            foreach (var parallax in parallaxes)
            {
                parallax.ApplyParallaxMovement(cameraDelta, cameraCurrentPosition);
            }
            cameraLastPosition = cameraTransform.position;
        }
    }
}
