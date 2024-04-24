using UnityEngine;
using TMPro;

namespace Game
{
    public class LifeCount : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tmpro;

        public void UpdateLifeCount(int newLifeNumber)
        {
            tmpro.text = newLifeNumber.ToString();
        }
    }
}

