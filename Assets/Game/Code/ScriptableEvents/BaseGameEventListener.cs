using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Listens for Event "E" of Type "T" and invokes Unity Event Response "UER".
    /// </summary>
    /// <typeparam name="T">Data "Type".</typeparam>
    /// <typeparam name="E">Event ScriptableObject of Type "T".</typeparam>
    /// <typeparam name="UER">Unity Event Response.</typeparam>
    public abstract class BaseGameEventListener<T, E, UER> : MonoBehaviour,
        IGameEventListener<T> where E : BaseGameEvent<T> where UER : UnityEvent<T>
    {
        [Tooltip("Event to Listen To.")]
        [SerializeField] E gameEvent;
        public E GameEvent { get { return gameEvent; } set { gameEvent = value; } }

        [Tooltip("The Unity Events that will be invoked in response.")]
        [SerializeField] UER unityEventResponse;

        private void OnEnable()
        {
            if (gameEvent == null)
            {
                return;
            }
            GameEvent.RegisterListener(this);
        }
        private void OnDisable()
        {
            if (gameEvent == null)
            {
                return;
            }
            GameEvent.UnregisterListener(this);
        }

        /// <summary>
        /// Invokes "UER" once "E" is raised.
        /// </summary>
        /// <param name="item"></param>
        public void OnEventRaised(T item)
        {
            unityEventResponse?.Invoke(item);
        }
    }
}
