using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public abstract class BaseItemDropEffect : MonoBehaviour
	{
		public abstract void ApplyEffect(GameObject targetPlayer);
	}
}