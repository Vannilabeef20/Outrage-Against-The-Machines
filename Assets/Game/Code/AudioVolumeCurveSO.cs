using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	[CreateAssetMenu(fileName = "AudioVolumeCurveSO", menuName = "New AudioVolumeCurveSO")]
	public class AudioVolumeCurveSO : ScriptableObject
	{
        [field: SerializeField, CurveRange(0f, -100f, 1f, 100)] public AnimationCurve Curve { get; private set; }		
	}
}