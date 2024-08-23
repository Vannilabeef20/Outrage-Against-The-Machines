using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	[CreateAssetMenu(fileName = "AudioVolumeCurveSO", menuName = "Misc/New AudioVolumeCurveSO")]
	public class AudioVolumeCurveSO : ScriptableObject
	{
        [field: SerializeField] public AnimationCurve Curve { get; private set; }		
	}
}