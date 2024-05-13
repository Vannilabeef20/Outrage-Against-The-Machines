using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{   public class BaseGameEvent<T> : ScriptableObject
    {
        public bool isTimeScaleIndependent;
        public EDebugSubjectFlags debugFlag;
        private readonly List<IGameEventListener<T>> eventListeners = new List<IGameEventListener<T>>();
    
        public virtual void Raise(object sender,T item)
        {
            if(Time.timeScale == 0 && !isTimeScaleIndependent)
            {
                return;
            }

            for (int i = eventListeners.Count - 1; i >= 0; i--)
            {
                eventListeners[i].OnEventRaised(item);
            }
        }      

        public void RegisterListener(IGameEventListener<T> listener)
        {
            if(!eventListeners.Contains(listener))
            {
                eventListeners.Add(listener);
            }
        }

        public void UnregisterListener(IGameEventListener<T> listener)
        {
            if (eventListeners.Contains(listener))
            {
                eventListeners.Remove(listener);
            }
        }
       
    }
}
