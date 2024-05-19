using UnityEngine;
using DG.Tweening;

namespace Game
{
	[CreateAssetMenu(fileName = "PlayerHealthUIConfigSO", menuName = "Player/Health UI Config")]
	public class PlayerHealthUIConfigSO : ScriptableObject
	{
		[field: SerializeField] public float HealthLerpDelay { get; private set; }
		[field: SerializeField] public float HealthLerpDuration { get; private set; }
		[field: SerializeField] public Ease HealthLerpEase { get; private set; }
	}
}