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
        List<IGameEventListener<T>> eventListeners = new();

        [Header("PARAMS"), HorizontalLine(2f, EColor.Red)]
        [SerializeField] bool isTimeScaleIndependent;

        [Header("DEBUG"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField] T testValue;
        [SerializeField, ReadOnly] List<string> listenerNames = new();

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
            EventLog(sender, data);

            if (eventListeners.Count < 1) return;

            if (Time.timeScale == 0 && !isTimeScaleIndependent) return;

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

        private void EventLog(object sender, T data, bool isTest = false)
        {
            if (!isTest)
            {
                this.Log($"<color=#8c53c6>{name} </color><color=white>was raised!</color>" +
                    $" Data: {data.ToString()}", EDebugSubjectFlags.CustomEvents);
            }

            else
            {
                this.Log($"<color=#8c53c6>{name} </color><color=white>was raised!</color>" +
                    $" Data: {data.ToString()}", EDebugSubjectFlags.Testing);
            }
        }

        public bool HasListener(IGameEventListener<T> listener)
        {
            return eventListeners.Contains(listener);
        }

        [Button("TEST LOG", EButtonEnableMode.Always)]
        public void TestDefaultLog()
        {
            EventLog(this, default, true);
        }   

        [Button("RAISE 'TEST VALUE'", EButtonEnableMode.Playmode)]
        public void TestRaise()
        {
            Raise(this, testValue);
        }
    }
}
