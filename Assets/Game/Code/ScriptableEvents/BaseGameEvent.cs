using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Scriptable Object Event of Type "T".
    /// </summary>
    /// <typeparam name="T">Data "Type".</typeparam>
    public class BaseGameEvent<T> : ScriptableObject
    {
        [SerializeField] bool isTimeScaleIndependent;
        List<IGameEventListener<T>> eventListeners = new();

        /// <summary>
        /// Sends "sender" and "data" to all listeners.
        /// </summary>
        /// <param name="sender">The object which sent the Raise request.</param>
        /// <param name="data">The data of Type "T" to be sent to all listeners on Raise.</param>
        public virtual void Raise(object sender, T data)
        {
            if(eventListeners.Count < 1)
            {
                return;
            }
            if (Time.timeScale == 0 && !isTimeScaleIndependent)
            {
                return;
            }
            for (int i = eventListeners.Count - 1; i >= 0; i--)
            {
                eventListeners[i].OnEventRaised(data);
            }
        }

        /// <summary>
        /// Adds "listener" to the "eventListerners" list.
        /// </summary>
        /// <param name="listener">Listener to be added to the list.</param>
        public void RegisterListener(IGameEventListener<T> listener)
        {
            if (!HasListener(listener))
            {
                eventListeners.Add(listener);
            }
        }

        /// <summary>
        /// Removes "listener" from the "eventListerners" List.
        /// </summary>
        /// <param name="listener">Listener to be removed from the list.</param>
        public void UnregisterListener(IGameEventListener<T> listener)
        {
            if (HasListener(listener))
            {
                eventListeners.Remove(listener);
            }
        }

        public bool HasListener(IGameEventListener<T> listener)
        {
            return eventListeners.Contains(listener);
        }

        [Button("Log all listeners", EButtonEnableMode.Playmode)]
        public void LogListeners()
        {
            foreach(IGameEventListener<T> listener in eventListeners)
            {
                Debug.Log(listener);
            }
        }
    }
}
