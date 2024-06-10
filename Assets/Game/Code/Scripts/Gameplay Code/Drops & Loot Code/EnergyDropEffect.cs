using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class EnergyDropEffect : BaseItemDropEffect
	{
		[SerializeField] private float specialCharge;
		public override void ApplyEffect(GameObject targetPlayer)
		{
			PlayerAttackingState attackingState = targetPlayer.GetComponentInChildren<PlayerStateMachine>().Attacking;
			if(attackingState == null)
            {
				Debug.Log("No attackingState");
				return;
            }
			attackingState.AddSpecialCharges(specialCharge, true);
		}
	}
}