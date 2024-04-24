using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "PlayerCombo", menuName = "Player/Combo")]
    public class PlayerComboSO : ScriptableObject
    {
        [Tooltip("Attacks that belong to this combo, in execution order")]
        public PlayerAttackSO[] attacks;
        public EPlayerInput[] input;
    }

    public enum EPlayerInput
    {
        Punch,
        Kick,
        Grab,
        Special
    }
}
