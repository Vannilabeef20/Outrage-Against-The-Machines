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

        [SerializeField] protected AnimationClip StateAnimation;
        public virtual string Name { get {return GetType().Name; } private set {value = GetType().Name;} }
        #region PROGRESS RELATED ATTRIBUTES

        [SerializeField, ReadOnly] protected float startTime;
        protected float UpTime => Time.time - startTime;

        [SerializeField, ReadOnly, Range(0f, 1f)] protected float progress;
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
