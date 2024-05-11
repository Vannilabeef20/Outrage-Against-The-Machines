using UnityEngine;
using DG.Tweening;

namespace Game
{
	[CreateAssetMenu(fileName = "PlayerSpecialUIConfigSO", menuName = "Player/Special UI Config")]
	public class PlayerSpecialUIConfigSO : ScriptableObject
	{
		[field:SerializeField] public float SpecialLerpDelay { get; private set; }
		[field: SerializeField] public float SpecialLerpDuration{ get; private set; }
		[field: SerializeField] public Ease SpecialLerpEasing { get; private set; }
	}
}