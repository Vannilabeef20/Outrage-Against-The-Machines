using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "FallingBoxSpeedSO", menuName = "New FallingBoxSpeedSO")]
	public class FallingBoxSpeedSO : ScriptableObject
	{
        [field: SerializeField] public float FallingSpeed { get; private set; }
	}
}