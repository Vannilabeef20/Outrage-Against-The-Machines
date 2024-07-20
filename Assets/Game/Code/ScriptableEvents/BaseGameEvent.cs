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
        [SerializeField, Expandable] DebugSO debugSO;
        [SerializeField] bool isTimeScaleIndependent;
        [SerializeField, ReadOnly] List<string> listenerNames = new();
        List<IGameEventListener<T>> eventListeners = new();
        [SerializeField] T testValue;

        private void OnDisable()
        {
            listenerNames.Clear();
        }

        /// <summary>
        /// Sends "sender" and "data" to all listeners.
        /// </summary>
        /// <param name="sender">The object which sent the Raise request.</param>
        /// <param name="data">The data of Type "T" to be sent to all listeners on Raise.</param>
        public virtual void Raise(object sender, T data)
        {
            if (eventListeners.Count < 1) return;

            if (Time.timeScale == 0 && !isTimeScaleIndependent) return;

            for (int i = eventListeners.Count - 1; i >= 0; i--)
            {
                eventListeners[i].OnEventRaised(data);
            }
            EventLog(sender, data);
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
                listenerNames.Add(listener.ToString());
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
                listenerNames.Remove(listener.ToString());
            }
        }

        private void EventLog(object sender, T data)
        {
            debugSO.Log(sender, $"<color=#8c53c6>{name} </color><color=white>was raised!</color> Data: {data.ToString()}", EDebugSubjectFlags.CustomEvents);
        }

        public bool HasListener(IGameEventListener<T> listener)
        {
            return eventListeners.Contains(listener);
        }

        [Button("TEST DEFAULT RAISE LOG", EButtonEnableMode.Always)]
        public void TestDefaultLog()
        {
            EventLog(this, default);
        }

        [Button("RAISE TEST VALUE", EButtonEnableMode.Playmode)]
        public void TestRaise()
        {
            Raise(this, testValue);
        }
    }
}
