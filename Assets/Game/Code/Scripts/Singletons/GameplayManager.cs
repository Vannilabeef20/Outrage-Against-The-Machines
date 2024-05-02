using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance { get; private set; }

        #region Players
        [field: Header("Players"), HorizontalLine(2f, EColor.Red)]

        public PlayerInputManager UnityInputManager;

        [SerializeField] private GameObject[] playerCharacters;

        [SerializeField] private Vector3[] spawnCoordinates;

        public int[] playerIndexes;

        public bool[] playerAlive = new bool[3];

        [field: SerializeField, ReadOnly] public GameObject[] PlayerObjectArray { private set; get; }

        [SerializeField] private GameObject followGroupPrefab;
        #endregion

        #region Lifes
        [SerializeField] private IntEvent UpdateLifeCount;
        [field: SerializeField, ReadOnly] public int CurrentLifeAmount { get; private set; }
        public int maxLifeAmount;
        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            int level = SceneManager.GetActiveScene().buildIndex;
            if (level != 0)
            {
                PlayerObjectArray = GameObject.FindGameObjectsWithTag("Player");
                CurrentLifeAmount = maxLifeAmount;
                UpdateLifeCount.Raise(this, CurrentLifeAmount);
                UpdateLifeCount.Raise(this, CurrentLifeAmount);
                List<GameObject> players = new();
                for (int i = 0; i < playerIndexes.Length; i++)
                {
                    if (playerIndexes[i] >= 0)
                    {
                        playerAlive[i] = true;
                        players.Add(Instantiate(playerCharacters[i], spawnCoordinates[i], Quaternion.identity));
                    }
                }
                PlayerObjectArray = players.ToArray();
                Instantiate(followGroupPrefab);
            }
            else
            {
                Destroy(gameObject);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach (Vector3 coordinate in spawnCoordinates)
            {
                Gizmos.DrawSphere(coordinate, 0.2f);
            }
        }
#endif
        public void TakeAddLife(int amount)
        {
            CurrentLifeAmount += amount;
            UpdateLifeCount.Raise(this, CurrentLifeAmount);
        }

    }

}
