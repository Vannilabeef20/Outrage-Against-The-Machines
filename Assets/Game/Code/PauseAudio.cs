using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
	[RequireComponent(typeof(PauseEventListener))]
	public class PauseAudio : MonoBehaviour
	{
        [SerializeField] StudioEventEmitter[] audioEmitters;
        public void SetPaused(bool pause)
		{
			foreach (StudioEventEmitter emitter in audioEmitters)
            {
				emitter.EventInstance.setPaused(pause);
			}
		}

		#region BUTTONS

		[Button("PAUSE PLAYBACK", EButtonEnableMode.Playmode)]
		void PausePlayback()
		{
			SetPaused(true);
		}
		[Button("UNPAUSE PLAYBACK", EButtonEnableMode.Playmode)]
		void UnpausePlayback()
		{
			SetPaused(false);
		}
        #endregion
    }
}