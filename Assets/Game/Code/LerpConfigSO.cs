using UnityEngine;
using DG.Tweening;

namespace Game
{
	[CreateAssetMenu(fileName = "PlayerHealthUIConfigSO", menuName = "Player/Health UI Config")]
	public class LerpConfigSO : ScriptableObject
	{
		[field: SerializeField] public float Delay { get; private set; }
		[field: SerializeField] public float Duration { get; private set; }
		[field: SerializeField] public Ease Ease { get; private set; }
	}
}