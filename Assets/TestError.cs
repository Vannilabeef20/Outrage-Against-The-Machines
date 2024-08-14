using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class TestError : MonoBehaviour
	{
        float a = 0;
        GameObject game;
        [Button("Test Null")]
		void TestNull()
        {
            game.SetActive(false);
        }
        [Button("Test Divide 0")]
        void TestDivide0()
        {
            float aa = 1 / a;
        }
    }
}