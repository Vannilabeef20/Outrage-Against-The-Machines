using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Defines all encounters in this level and its parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelEncounter", menuName = "Misc/New LevelEncounterSO")]
    public class LevelEncountersSO : ScriptableObject
	{
        [field: SerializeField] public Encounter[] Encounters { get; private set; }

        private void Awake()
        {
            Sort();
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateNames();
        }
#endif
        /// <summary>
        /// Sort all encounters by X position.
        /// </summary>
        [Button("SORT BY X")]
        private void Sort()
        {
            if(Encounters == null) return;
            Encounters = Encounters.OrderBy(e => e.position.x).ToArray();
            UpdateNames();
        }

        /// <summary>
        /// Update all encounters and its enemies names.
        /// </summary>
        private void UpdateNames()
        {
            if (Encounters == null) return;
            for (int i = 0; i < Encounters.Length; i++)
            {
                int enemyCount = 0;
                int waveCount = 0;

                foreach (var wave in Encounters[i].waves)
                {
                    if (wave == null) continue;

                    waveCount++;

                    foreach (var enemy in wave.enemies)
                    {
                        if (enemy != null) enemyCount += enemy.amount;
                    }
                }

                Encounters[i].Name = $"{i} | Waves: {waveCount} || Enemies: {enemyCount}";

                Encounters[i].UpdateNames();
            }
        }
    }

    /// <summary>
    /// Defines an enemy encounter and its parameters.<br/>
    /// Position, and Waves.
    /// </summary>
    [System.Serializable]
    public class Encounter
    {
        [HideInInspector] public string Name;

        [SerializeField] public Vector3 position;
        [SerializeField] public EnemyWave[] waves;

        public void UpdateNames()
        {
            if (waves == null) return;

            for (int i = 0; i < waves.Length; i++)
            {
                if (waves[i] == null) continue;

                int enemyCount = 0;
                foreach (var enemy in waves[i].enemies)
                {
                    if(enemy != null) enemyCount += enemy.amount;
                }
                waves[i].Name = $"{i} | Enemies: {enemyCount}";
                waves[i].UpdateNames();
            }
        }
    }

    [System.Serializable]
    public class EnemyWave
    {
        [HideInInspector] public string Name;
        [SerializeField] public SpawnableEnemy[] enemies;

        public void UpdateNames()
        {
            if(enemies == null) return;

            for (int j = 0; j < enemies.Length; j++)
            {
                if (enemies[j].enemy == null)
                {
                    enemies[j].Name = "None";
                    continue;
                }

                string playerReq;
                if (enemies[j].multiplayerOnly)
                {
                    playerReq = enemies[j].playersRequired.ToString();
                }
                else
                {
                    playerReq = "Any";
                }

                enemies[j].Name = $"{enemies[j].enemy.name} | {enemies[j].amount} Units| {playerReq}-P";
            }
        }
    }

    /// <summary>
    /// Defines an enemy and its parameters.<br/>
    /// The enemy type/prefab and amount.
    /// </summary>
    [System.Serializable]
    public class SpawnableEnemy
    {
        [HideInInspector] public string Name;
        [ShowAssetPreview] public GameObject enemy;
        public int amount;
        [Space]
        public bool multiplayerOnly;
        [AllowNesting, ShowIf("multiplayerOnly"), Range(2, 3)] public int playersRequired = 2;

    }

}