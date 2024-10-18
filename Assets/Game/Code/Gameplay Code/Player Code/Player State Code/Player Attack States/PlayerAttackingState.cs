using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using FMODUnity;

namespace Game
{
    /// <summary>
    /// Player State/StateMachine, Handles Player Attacks.
    /// </summary>
    public class PlayerAttackingState : PlayerState
    {
        /// <summary>
        /// #override# The name of the current attack state.
        /// </summary>
        public override string Name { get { return CurrentAttackState != null ?
                    CurrentAttackState.PlayerAttack.name : null; } }
        public Dictionary<PlayerAttackSO, PlayerAttackState> PlayerAttackStatesDictionary { get; private set; }
            = new Dictionary<PlayerAttackSO, PlayerAttackState>();

        [SerializeField] GameObject attackStatesParent;

        /// <summary>
        /// Array for all available combos.
        /// </summary>
        [field:SerializeField, Expandable] public PlayerComboSO[] PlayerCombos { get; private set; }

        [SerializeField] BoxCollider[] hitboxes;

        public StudioEventEmitter attackEmitter;

        [field:SerializeField, ReadOnly] public PlayerAttackState CurrentAttackState { get; private set; }

        [ReadOnly] public PlayerAttackState queuedAttackState;

        [ReadOnly] public List<PlayerAttackSO> attackList;
        [field: SerializeField] public float MaxSpecialChargeAmount { get; private set; }

        [ProgressBar("Special Charge amount","MaxSpecialChargeAmount", EColor.Blue)] public float specialChargeAmount;

        [SerializeField] IntFloatEvent specialChargeEvent;

        [SerializeField] ParticleSystem specialParticleSystem;


        public override void Setup(PlayerStateMachine playerStateMachine) // called on awake
        {
            base.Setup(playerStateMachine);
            PlayerAttackState[] playerAttacks = attackStatesParent.GetComponentsInChildren<PlayerAttackState>();
            foreach(var attack in playerAttacks)
            {
                attack.Setup(playerStateMachine);
                PlayerAttackStatesDictionary.Add(attack.PlayerAttack, attack);
            }
            UpdateSpecialBar();
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
            if(!IsComplete)
            CurrentAttackState.Exit();

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
                if (PlayerCombos[i].ComboAttacks.Length <= attackList.Count) //Check if attack list is bigger than the [i] combo
                {
                    continue; //Bigger, skip this one
                }
                if (PlayerCombos[i].ComboAttacks[attackList.Count].Input.ToString() == context.action.name) //Check if the attempeted Inputs is in this combo
                {
                    if (PlayerCombos[i].ComboAttacks[attackList.Count].Attack.IsSpecial && specialChargeAmount < PlayerCombos[i].ComboAttacks[attackList.Count].Attack.SpecialCost) // Check if it is a special attack && there are enough charges
                    {
                        //It is a special and does not have enough charges, Skip!
                        continue;
                    }
                    int matchNumber = 0; 
                    for (int j = 0; j < attackList.Count; j++) //Check how many Attacks are the same
                    {
                        if (PlayerCombos[i].ComboAttacks[j].Attack == attackList[j])
                        {
                            matchNumber++;
                        }
                    }
                    if(matchNumber != attackList.Count)
                    {
                        continue; //Not all Attacks are the same, Abort!
                    }

                    //From this point on the attack is validated

                    if(PlayerCombos[i].ComboAttacks[attackList.Count].Attack.IsSpecial)
                    AddSpecialCharges(-PlayerCombos[i].ComboAttacks[attackList.Count].Attack.SpecialCost);                   
                    if(stateMachine.CurrentState != this) //Check if the state machine is already in the attacking state
                    {
                        //Its not, transition pls
                        stateMachine.nextState = this;
                        stateMachine.overrideStateCompletion = true;
                    }                   
                    queuedAttackState = PlayerAttackStatesDictionary[PlayerCombos[i].ComboAttacks[attackList.Count].Attack];
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
        
        public void FlipCharacter()
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

        public void SetSpecialCharges(float newChargeAmount, bool pickUp = false)
        {
            specialChargeAmount = Mathf.Clamp(newChargeAmount, stateMachine.playerInput.playerIndex, MaxSpecialChargeAmount);
            if (pickUp)
            {
                specialParticleSystem.Play();
            }
            UpdateSpecialBar();
        }

        public void AddSpecialCharges(float chargeAmount, bool pickUp = false)
        {
            specialChargeAmount = Mathf.Clamp(specialChargeAmount + chargeAmount, stateMachine.playerInput.playerIndex, MaxSpecialChargeAmount);
            if(pickUp)
            {
                specialParticleSystem.Play();
            }
            UpdateSpecialBar();
        }

        public void UpdateSpecialBar()
        {
            float newSpecialPercent = specialChargeAmount / MaxSpecialChargeAmount;
            specialChargeEvent.Raise(this, new IntFloat(stateMachine.playerInput.playerIndex, newSpecialPercent));
        }

        //[Button("Generate/Regenerate attack states", EButtonEnableMode.Editor)]
        private void GenerateAttackStates()
        {
            GameObject[] children = attackStatesParent.GetComponentsInChildren<GameObject>();
            foreach (GameObject child in children)
            {
                Destroy(child);
            }

            HashSet<PlayerAttackSO> attackSOs = new();
            foreach(PlayerComboSO combo in PlayerCombos)
            {
                for(int i = 0; i < combo.ComboAttacks.Length; i++)
                {
                    attackSOs.Add(combo.ComboAttacks[i].Attack);
                }
            }

            foreach(PlayerAttackSO attackSO in attackSOs)
            {
                GameObject attackStateObject = Instantiate(new GameObject(), attackStatesParent.transform);
                PlayerAttackState state = attackStateObject.AddComponent<PlayerAttackState>();
                //state.PlayerAttack = attackSO; make it readonly
                attackStateObject.name = attackSO.name;
            }
        }
    }
}
