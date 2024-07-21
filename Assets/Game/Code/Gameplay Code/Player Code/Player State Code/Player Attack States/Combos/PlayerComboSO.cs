using System;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    [CreateAssetMenu(fileName = "PlayerComboAttack", menuName = "Player/PlayerComboAttack")]
    public class PlayerComboSO : ScriptableObject
    {
        [field: SerializeField] public PlayerComboAttack[] ComboAttacks { get; private set; }
#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateNames();
        }

        void UpdateNames()
        {
            for(int i = 0; i < ComboAttacks.Length; i++)
            {
                if(ComboAttacks[i].Attack != null)
                ComboAttacks[i].Name = $"{i} | {ComboAttacks[i].Attack.name}";
            }
        }
#endif
    }

    [Serializable]
    public class PlayerComboAttack
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public EPlayerInput Input { get; private set; }
        [field: SerializeField, Expandable] public PlayerAttackSO Attack { get; private set; }

        [SerializeField] PlayerState permittedPreviousState;
    }
}
