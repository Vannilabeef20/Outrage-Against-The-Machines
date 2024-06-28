using System;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Base class to be inherited by all states.
    /// </summary>
    public abstract class BaseState : MonoBehaviour
    {
        #region BASE STATE ATTRIBUTES

        [Header("#BASE STATE# INHERITED"), HorizontalLine(2f, EColor.Red)]
        [Tooltip("The AnimationClip that may play during this state.")]
        [field: SerializeField] protected AnimationClip StateAnimation;
        /// <summary>
        /// This state's name.<br/>
        /// Set by getting current Type name.
        /// </summary>
        public virtual string Name { get {return GetType().Name; } private set {value = GetType().Name;} }
        #region PROGRESS RELATED ATTRIBUTES

        [Tooltip("The point in time (seconds since the game has been running) this state has started.")]
        [SerializeField, ReadOnly] protected float startTime;

        /// <summary>
        /// How long (seconds) this state has been running.
        /// </summary>
        protected float UpTime => Time.time - startTime;

        [Tooltip("Measures the current progress of the state in normalized time." +
            "\nCalculus abstraction: (HowLongThisStateHasBeenRunning / ExpectedStateDuration)." +
            "\nNOT ALL STATES USE THIS, SOME USE JUST *ISCOMPLETE*")]
        [SerializeField, ReadOnly, Range(0f, 1f)] protected float progress;

        /// <summary>
        /// Whether this state has run its course.<br/>
        /// When true this state's state machine may transition to another state.
        /// </summary>
        [field: Tooltip("Whether this state has run its course\n" +
            "When true this state's state machine may transition to another state.")]
        [field: SerializeField, ReadOnly] public bool IsComplete { get; protected set; }

        #endregion

        #endregion

        #region ABSTRACT METHODS

        /// <summary>
        /// This method will be run as soon as the state machine transitions to this state.
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// This method will be run as soon as the state machine transitions out of this state.
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// This method will run on each "Update" while it is the current state in the state machine
        /// </summary>
        public abstract void Do();

        /// <summary>
        /// This method will run on each "FixedUpdate" while it is the current state in the state machine
        /// </summary>
        public abstract void FixedDo();

        /// <summary>
        /// This method will check whether the state is complete or not, may run on "Do" or "FixedDo".
        /// </summary>
        protected abstract void ValidateState();
        #endregion
    }

}
