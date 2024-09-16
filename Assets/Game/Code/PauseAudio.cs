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
		[SerializeField] bool reversed;

		[Header("REFERENCES"), HorizontalLine(2F, EColor.Red)]

		[Tooltip("Emitters to be subject to pausing.")]
		[SerializeField] StudioEventEmitter[] audioEmitters;

		bool first = true;
		/// <summary>
		/// Sets all referenced audioEmitters paused or unpaused
		/// according with the given pause value.  
		/// </summary>
		/// <param name="pause">bool value tha defines whether the
		/// emmiters should be paused or unpaused.</param>
		public void SetPaused(bool pause)
		{
			if(reversed)
            {
				foreach (StudioEventEmitter emitter in audioEmitters)
				{
					emitter.EventInstance.getPaused(out bool isPaused);
					if (isPaused == false && pause == true) 
                    {
						emitter.EventInstance.setPaused(false);
						if (first)
                        {
							first = false;
							emitter.Play();
                        }
                    }
					else if (isPaused == true && pause == false) emitter.EventInstance.setPaused(true);
				}
			}
			else
            {
				foreach (StudioEventEmitter emitter in audioEmitters)
				{
					emitter.EventInstance.getPaused(out bool isPaused);
					if (isPaused == false && pause == true) emitter.EventInstance.setPaused(true);
					else if (isPaused == true && pause == false) emitter.EventInstance.setPaused(false);
				}
			}
		}

		#region BUTTONS

		/// <summary>
		/// Pauses all referenced audioEmitters.
		/// </summary>
		[Button("PAUSE PLAYBACK", EButtonEnableMode.Playmode)]
		void PausePlayback()
		{
			SetPaused(true);
		}
		/// <summary>
		/// Unpauses all referenced audioEmitters.
		/// </summary>
		[Button("UNPAUSE PLAYBACK", EButtonEnableMode.Playmode)]
		void UnpausePlayback()
		{
			SetPaused(false);
		}
		#endregion
	}
}