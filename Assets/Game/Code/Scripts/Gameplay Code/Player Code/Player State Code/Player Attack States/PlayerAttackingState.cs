using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Game
{
    public class PlayerAttackingState : PlayerState
    {
        public override string Name { get { return CurrentAttackState != null ?
                    CurrentAttackState.playerAttack.name : null; } }
        public Dictionary<PlayerAttackSO, PlayerAttackState> PlayerAttackStatesDictionary { get; private set; }
            = new Dictionary<PlayerAttackSO, PlayerAttackState>();
        [field:SerializeField, Expandable] public PlayerComboSO[] PlayerCombos { get; private set; }

        [SerializeField] private BoxCollider[] hitboxes;

        public AudioSource attackAudioSource;

        [field:SerializeField, ReadOnly] public PlayerAttackState CurrentAttackState { get; private set; }

        [ReadOnly] public PlayerAttackState queuedAttackState;

        [ReadOnly] public List<PlayerAttackSO> attackList;
        [field: SerializeField] public float MaxSpecialChargeAmount { get; private set; }

        [ProgressBar("Special Charge Amount","MaxSpecialChargeAmount", EColor.Blue)] public float specialChargeAmount;

        [SerializeField] private Image[] specialImages;

        public override void Setup(PlayerStateMachine playerStateMachine) // called on awake
        {
            base.Setup(playerStateMachine);
            HashSet<PlayerAttackSO> playerAttackStates = new();
            foreach(var combo in PlayerCombos)
            {
                foreach(PlayerAttackSO attack in combo.attacks)
                {
                    playerAttackStates.Add(attack);
                }
            }
            foreach(var attackSO in playerAttackStates)
            {
                GameObject stateObject = new GameObject (attackSO.name + " state");
                stateObject.transform.parent = transform;
                stateObject.AddComponent<PlayerAttackState>();
                PlayerAttackState state = stateObject.GetComponent<PlayerAttackState>();
                state.Initialize(stateMachine, this, attackSO);
                PlayerAttackStatesDictionary.Add(attackSO, state);
            }
            UpdateSpecialBar(0f);
            CurrentAttackState = null;
            queuedAttackState = null;
        }

        public override void Do()
        {
            if(ValidateStateBool())
            {
                return;
            }
            TransitionState();
            if(CurrentAttackState == null)
            {
                return;
            }
            CurrentAttackState.Do();
        }

        public override void FixedDo()
        {
            if (CurrentAttackState == null)
            {
                return;
            }
            CurrentAttackState.FixedDo();
        }

        public override void Enter()
        {
            startTime = Time.time;
            IsComplete = false;
        }

        public override void Exit()
        {
            DisableAttackHitboxes();
            CurrentAttackState = null;
            queuedAttackState = null;
            attackList.Clear();
            IsComplete = false;
        }

        protected override void ValidateState()
        {
            if(CurrentAttackState == null && queuedAttackState == null)
            {
                stateMachine.nextState = stateMachine.Idle;
                attackList.Clear();
                IsComplete = true;
            }
        }

        private bool ValidateStateBool()
        {
            if (CurrentAttackState == null && queuedAttackState == null)
            {
                stateMachine.nextState = stateMachine.Idle;
                attackList.Clear();
                IsComplete = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ValidateAttack(InputAction.CallbackContext context)
        {
            if (queuedAttackState != null) //return if attack already queued
            {
                return;
            }
            for (int i = 0; i < PlayerCombos.Length; i++) //check if theres available attack in a combo
            {
                if (PlayerCombos[i].attacks.Length <= attackList.Count) //Check if attack list is bigger than the [i] combo
                {
                    continue; //Bigger, skip this one
                }
                if (PlayerCombos[i].input[attackList.Count].ToString() == context.action.name) //Check if the attempeted input is in this combo
                {
                    if (PlayerCombos[i].attacks[attackList.Count].IsSpecial && specialChargeAmount < PlayerCombos[i].attacks[attackList.Count].SpecialCost) // Check if it is a special attack && there are enough charges
                    {
                        //It is a special and does not have enough charges, Abort!
                        return;
                    }
                    UpdateSpecialBar(-PlayerCombos[i].attacks[attackList.Count].SpecialCost);                   
                    if(stateMachine.CurrentState != this) //Check if the state machine is already in the attacking state
                    {
                        //Its not, transition pls
                        stateMachine.nextState = this;
                        stateMachine.overrideStateCompletion = true;
                    }                   
                    queuedAttackState = PlayerAttackStatesDictionary[PlayerCombos[i].attacks[attackList.Count]];
                    break;
                }
            }
        }
        private void TransitionState()
        {
            if (CurrentAttackState == null)
            {
                CurrentAttackState = queuedAttackState;
                queuedAttackState = null;
                FlipCharacter();
                CurrentAttackState.Enter();
            }
            else if (CurrentAttackState.IsComplete)
            {
                CurrentAttackState.Exit();
                CurrentAttackState = queuedAttackState;
                queuedAttackState = null;
                if(CurrentAttackState != null)
                {
                    FlipCharacter();
                    CurrentAttackState.Enter();
                }
            }
        }
        public void DisableAttackHitboxes()
        {
            foreach (var hitbox in hitboxes)
            {
                hitbox.enabled = false;
            }
        }
        
        private void FlipCharacter()
        {
            if (stateMachine.InputDirection.x > 0)
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); //Flip Right
            }
            else if (stateMachine.InputDirection.x < 0)
            {
                stateMachine.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0)); //Flip left
            }
        }

        public void UpdateSpecialBar(float damageCharges)
        {
            specialChargeAmount = Mathf.Clamp(specialChargeAmount + damageCharges, stateMachine.playerInput.playerIndex, MaxSpecialChargeAmount);
            float newSpecialPercent = specialChargeAmount / MaxSpecialChargeAmount;
            float perChargePercent = MaxSpecialChargeAmount / specialImages.Length / MaxSpecialChargeAmount;
            for (int j = 1; j < specialImages.Length + 1; j++)
            {
                float low = (j - 1) * perChargePercent;
                float high = j * perChargePercent;
                if(newSpecialPercent <= low)
                {
                    specialImages[j-1].fillAmount = 0;
                }
                else if(newSpecialPercent >= high)
                {
                    specialImages[j-1].fillAmount = 1;
                }
                else
                {
                    specialImages[j-1].fillAmount = newSpecialPercent.Map(low, high);
                }
            }
        }
    }
}
