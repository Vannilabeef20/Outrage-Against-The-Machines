using UnityEngine;

namespace Game
{
	/// <summary>
	/// Tells a "fallingBox" and its trap how fast it will fall.<br/>
	/// Workaround meant to eliminate the necessity to change the speed value in both the box prefab and trap.
	/// </summary>
	[CreateAssetMenu(fileName = "FallingBoxSpeedSO", menuName = "New FallingBoxSpeedSO")]
	public class FallingBoxSpeedSO : ScriptableObject
	{
		/// <summary>
		/// How fast this box will fall.
		/// </summary>
		[field: Tooltip("How fast this box will fall.")]
        [field: SerializeField] public float FallingSpeed { get; private set; }
	}
}