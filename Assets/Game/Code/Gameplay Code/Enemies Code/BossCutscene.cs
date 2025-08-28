using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinemachine;

namespace Game
{
	public class BossCutscene : MonoBehaviour
	{
        [SerializeField] CinemachineVirtualCamera cutsceneCamera;
        [SerializeField] BoolEvent inputEvent;
		[SerializeField] PlayableDirector director;
        [SerializeField] LayerMask playerMask;
        bool Lock;

        private void OnTriggerEnter(Collider other)
        {
            if (!playerMask.ContainsLayer(other.gameObject.layer)) return;

            Play();
        }

        [Button]
        private void Play()
        {
            if (Lock) return;

            Lock = true;
            inputEvent.Raise(this, false);
            cutsceneCamera.Priority = 40;
            director.Play();
        }

        public void Finish()
        {
            cutsceneCamera.Priority = 0;
            inputEvent.Raise(this, true);
        }
    }
}