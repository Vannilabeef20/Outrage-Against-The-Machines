using System.Linq;
using UnityEngine;
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
                Encounters[i].Name =$"Enc: {i}";
                for (int j = 0; j < Encounters[i].enemies.Length; j++)
                {
                    if (Encounters[i].enemies[j].enemy == null)
                    {
                        Encounters[i].enemies[j].Name = "None";
                        continue;
                    }
                    Encounters[i].enemies[j].Name =  $"{Encounters[i].enemies[j].enemy.name} | {Encounters[i].enemies[j].amount} Units";
                }
            }
        }
    }

    /// <summary>
    /// Defines an enemy encounter and its parameters.<br/>
    /// Position, and enemies.
    /// </summary>
    [System.Serializable]
    public class Encounter
    {
        [HideInInspector] public string Name;

        [SerializeField, HorizontalLine(2f, EColor.Red)] public Vector3 position;
        [SerializeField] public SpawnableEnemy[] enemies;

    }

    /// <summary>
    /// Defines an enemy and its parameters.<br/>
    /// The enemy type/prefab and amount.
    /// </summary>
    [System.Serializable]
    public class SpawnableEnemy
    {
        [HideInInspector] public string Name;
        [SerializeField, ShowAssetPreview] public GameObject enemy;
        [SerializeField] public int amount;
    }
    
}