using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	/// <summary>
	/// Stores a curve for volume control.
	/// </summary>
	[CreateAssetMenu(fileName = "AudioVolumeCurveSO", menuName = "Misc/New AudioVolumeCurveSO")]
	public class AudioVolumeCurveSO : ScriptableObject
	{
        [field: SerializeField] public AnimationCurve Curve { get; private set; }		
	}
}