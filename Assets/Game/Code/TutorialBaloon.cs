using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class TutorialBaloon : MonoBehaviour
	{
        Animator balloonAnimator;
        BoxCollider detectionZone;
        [SerializeField] Collider[] detectedPlayerColliders;
        [SerializeField] bool active;
		[SerializeField] LayerMask playerLayer;

        private void Awake()
        {
            balloonAnimator = GetComponent<Animator>();
            detectionZone = GetComponent<BoxCollider>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!playerLayer.ContainsLayer(other.gameObject.layer)) return;
            Detect();
        }

        void Detect()
        {
            detectedPlayerColliders = Physics.OverlapBox(transform.TransformPoint(detectionZone.center),
                detectionZone.size/2, detectionZone.transform.rotation, playerLayer);

            active = detectedPlayerColliders.Length > 0;

            balloonAnimator.SetBool("Active", active);
        }

    }
}