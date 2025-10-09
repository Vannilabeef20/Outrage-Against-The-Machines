using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Game
{
	public class SkipCutscene : MonoBehaviour
	{
        [SerializeField] PlayableDirector director;
        bool Lock;

        private void Update()
        {
            if (Lock) return;

            if (director.state != PlayState.Playing) return;

            if(Input.anyKeyDown)
            {
                director.time = director.duration;
                Lock = true;
            }
        }
    }
}