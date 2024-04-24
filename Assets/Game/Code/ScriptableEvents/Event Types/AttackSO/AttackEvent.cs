using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Attack Event", menuName = "Game Events/Attack Event")]
    public class AttackEvent : BaseGameEvent<PlayerAttackSO> { }
}
