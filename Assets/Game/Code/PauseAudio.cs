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

		/// <summary>
		/// Sets all referenced audioEmitters paused or unpaused
		/// according with the given pause value.  
		/// </summary>
		/// <param name="pauseEvent">bool value tha defines whether the
		/// emmiters should be paused or unpaused.</param>
		public void SetPaused(bool pauseEvent)
		{
			foreach (StudioEventEmitter emitter in audioEmitters)
            {
				if (emitter == null)
				{
					this.LogError($"{gameObject.name} has a null emitter in pause");
					continue;
				}

				bool pause = pauseEvent ^ reversed;
				emitter.EventInstance.setPaused(pause);
			}		
		}

		#region BUTTONS
#pragma warning disable
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
#pragma warning restore
		#endregion
	}
}