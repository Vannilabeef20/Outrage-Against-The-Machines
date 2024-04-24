using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        [field:SerializeField, ReadOnly] public PlayerAttackState CurrentAttackState { get; private set; }

        [ReadOnly] public PlayerAttackState queuedAttackState;


        [ReadOnly] public List<PlayerAttackSO> attackList;

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
                if (PlayerCombos[i].attacks.Length <= attackList.Count)
                {
                    continue;
                }
                if (PlayerCombos[i].input[attackList.Count].ToString() == context.action.name)
                {
                    if(stateMachine.CurrentState != this)
                    {
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
                CurrentAttackState.Enter();
            }
            else if (CurrentAttackState.IsComplete)
            {
                CurrentAttackState.Exit();
                CurrentAttackState = queuedAttackState;
                queuedAttackState = null;
                if(CurrentAttackState != null)
                {
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
    }
}
