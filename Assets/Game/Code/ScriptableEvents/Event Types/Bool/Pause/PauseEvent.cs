using UnityEngine;

namespace Game
{
   [CreateAssetMenu(fileName = "New Pause Event", menuName = "SO Events/Pause Event")]
   public class PauseEvent : BaseGameEvent<bool> 
    {
        public override void Raise(object sender, bool data)
        {
            base.Raise(sender, data);
            if (data)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}