using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{ 
/// <summary>
/// Manages enemy spawning.
/// </summary>
    public class Spawner : MonoBehaviour
    {
        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]

        public static Spawner Instance;

        [SerializeField] Camera mainCam;
        [SerializeField, ReadOnly] CinemachineFramingTransposer virtualCameraFramingTransposer;
        [SerializeField] Image goImage;
        [SerializeField] IntEvent encounterEvent;
#if UNITY_EDITOR
        [SerializeField] private GameObject testSpawnEnemy;
#endif

        [Header("PARAMETERS"), HorizontalLine(2f, EColor.Orange)]
        [Tooltip("The distance range from the mainCam the enemy can spawn.")]
        [MinMaxSlider(-20f, 20f), SerializeField] Vector2 spawnDistance;
        [Tooltip("The absolute height range the enemy can spawn.")]
        [MinMaxSlider(-20f, 20f), SerializeField] Vector2 spawnHeight;
        [field: Tooltip("Defines all encounters on this level.")]
        [field: SerializeField] public LevelEncountersSO LevelEncounters { get; private set; }


        [Header("VARIABLES"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField, ReadOnly] float defaultDeadzoneWidtht = 0f;
        [SerializeField, ReadOnly] float maxDeadzoneWidtht = 2f;
        [SerializeField, ReadOnly] int currentEncounterIndex;
        [SerializeField, ReadOnly] List<GameObject> enemiesToSpawn;
        [ReadOnly] public List<GameObject> enemiesAlive;

        Coroutine encCoroutine;
        int PlayerCount => GameManager.Instance.UnityInputManager.playerCount;
        float MinSpawnDelay => LevelEncounters.Encounters[currentEncounterIndex].spawnDelay.x;
        float MaxSpawnDelay => LevelEncounters.Encounters[currentEncounterIndex].spawnDelay.y;
#if UNITY_EDITOR
        [Header("GIZMOS (EDITOR ONLY)"), HorizontalLine(2f, EColor.Green)]
        [SerializeField] Color handlesEncounterLineColor;
        [SerializeField] Color handlesSpawnLineColor;
        [SerializeField] float handlesLineScale;

        [SerializeField] GUIStyle style;

        Vector3 point1Pos;
        Vector3 point2Pos;
#endif

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else Instance = this;

            virtualCameraFramingTransposer = FindObjectOfType<CinemachineVirtualCamera>().
                GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Backspace)) DestroyAll();
#endif
            if (LevelEncounters.Encounters.Length == 0) return;

            if (LevelEncounters.Encounters.Length <= currentEncounterIndex) return;

            if (transform.position.x <= LevelEncounters.Encounters[currentEncounterIndex].position.x) return;

            if (encCoroutine != null) return;

            encCoroutine = StartCoroutine(EncounterRoutine());

        }

        private IEnumerator EncounterRoutine()
        {
            //Init
            goImage.enabled = false;
            virtualCameraFramingTransposer.m_DeadZoneWidth = maxDeadzoneWidtht;
            encounterEvent.Raise(this, currentEncounterIndex);

            //Wave loop
            foreach (var wave in LevelEncounters.Encounters[currentEncounterIndex].waves)
            {
                //Wave building
                foreach (var _enemy in wave.enemies)
                {
                    if (_enemy.multiplayerOnly && PlayerCount < _enemy.playersRequired) continue;

                    for (int i = 0; i < _enemy.amount; i++)
                    {
                        enemiesToSpawn.Add(_enemy.enemy);
                    }
                }
                enemiesToSpawn.ShuffleList();

                //Wave spawn loop
                SpawnEnemy();
                while (enemiesToSpawn.Count > 0)
                {
                    yield return new WaitForSeconds(Random.Range(MinSpawnDelay, MaxSpawnDelay));
                    SpawnEnemy();
                }

                //Wait till all enemies are dead
                while (enemiesAlive.Count > 0)
                {
                    yield return null;
                }
            }
            
            //End
            virtualCameraFramingTransposer.m_DeadZoneWidth = defaultDeadzoneWidtht;
            goImage.enabled = true;
            currentEncounterIndex++;
            encCoroutine = null;
        }


        public void SpawnEnemy()
        {
            if(enemiesToSpawn.Count < 1)
            {
                return;
            }
            Vector3 tempPosition = transform.position.ToXYY();
            float tempSpawnHeight = UnityEngine.Random.Range(spawnHeight.x, spawnHeight.y);
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                Instantiate(enemiesToSpawn[0], tempPosition + new Vector3(UnityEngine.Random.Range(spawnDistance.x, spawnDistance.y),
                   tempSpawnHeight, tempSpawnHeight), Quaternion.identity, transform);
                enemiesToSpawn.RemoveAt(0);
            }
            else
            {
                Instantiate(enemiesToSpawn[0], tempPosition + new Vector3(UnityEngine.Random.Range(-spawnDistance.x, -spawnDistance.y),
                   tempSpawnHeight, tempSpawnHeight), Quaternion.identity, transform);
                enemiesToSpawn.RemoveAt(0);
            }
        }

#if UNITY_EDITOR
        #region DEBUG & GIZMOS
        /// <summary>
        /// Spawns a cube inside the spawn zone to test its acurracy.
        /// </summary>
        [Button("Spawn testValue enemy cube")]
        public void SpawnTest()
        {
            Vector3 tempPosition = transform.position.ToXYY();
            float tempSpawnHeight = UnityEngine.Random.Range(spawnHeight.x, spawnHeight.y);
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                Instantiate(testSpawnEnemy, tempPosition + new Vector3(UnityEngine.Random.Range(spawnDistance.x, spawnDistance.y),
                   tempSpawnHeight, tempSpawnHeight), Quaternion.identity, transform);
            }
            else
            {
                Instantiate(testSpawnEnemy, tempPosition + new Vector3(UnityEngine.Random.Range(-spawnDistance.x, -spawnDistance.y),
                   tempSpawnHeight, tempSpawnHeight), Quaternion.identity, transform);
            }
        }

        /// <summary>
        /// Destroys all enemies in "enemiesAlive" and "enemiesToSpawn".
        /// </summary>
        [Button("Destroy all enemies")]
        public void DestroyAll()
        {
            GameObject[] enemyObjects = enemiesAlive.ToArray();
            foreach (var enemy in enemyObjects)
            {
                Destroy(enemy);
            }
            enemiesAlive.Clear();
            enemiesToSpawn.Clear();
        }

        private void OnDrawGizmos()
        {
            #region DRAW SPAWN BOXES
            Handles.color = handlesSpawnLineColor;

            //Furthest = Max distance .y, Closest = Min distance .x, Upper = MaxHeight .y, Lower = MinHeight .x, Left Blue, Right Magenta                                           
            //Furtherst upper right 
            point1Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.y,0);

            //Furtherst lower right                                                                              
            point2Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.x,0);

            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), handlesLineScale);
            //In betweem furthest and closest upper right upper                                                  
            point1Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.x,0);

            point2Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.x,0);

            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), handlesLineScale);
            //In betweem furthest and closest lower right lower                                                  
            point1Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.y,0);

            point2Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.y,0);

            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), handlesLineScale);
            //Closest upper right                                                                                
            point1Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.y,0);

            //Closest lower right                                                                                
            point2Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.x,0);

            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), handlesLineScale);
            //Furtherst upper left                                                                               
            point1Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.y,0);

            //Furtherst lower left                                                                               
            point2Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.x,0);

            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), handlesLineScale);
            //Closest upper left                                                                                 
            point1Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.y,0);

            //Closest lower left,                                                                                
            point2Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.x,0);

            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), handlesLineScale);
            //In betweem furthest and closest Left upper
            point1Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.y,0);

            point2Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.y,0);

            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), handlesLineScale);
            //In betweem furthest and closest Left lower
            point1Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.x,0);

            point2Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.x,0);

            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), handlesLineScale);
            #endregion

            #region DRAW ENCOUNTER ZONE
            Handles.color = handlesEncounterLineColor;
            if (LevelEncounters.Encounters.Length == 0)
            {
                return;
            }

            for (int i = 0; i < LevelEncounters.Encounters.Length; i++)
            {
                if(mainCam != null)
                {
                    Vector3 tempPos1 = LevelEncounters.Encounters[i].position; //UP RIGHT
                    tempPos1.y += mainCam.orthographicSize; 
                    tempPos1.x += mainCam.aspect * mainCam.orthographicSize;
                    Vector3 tempPos2 = LevelEncounters.Encounters[i].position; //UP LEFT 
                    tempPos2.y += mainCam.orthographicSize;
                    tempPos2.x -= mainCam.aspect * mainCam.orthographicSize;
                    Handles.DrawDottedLine(tempPos1, tempPos2, handlesLineScale); //UP LINE

                    tempPos1 = LevelEncounters.Encounters[i].position; //DOWN RIGHT
                    tempPos1.y -= mainCam.orthographicSize;
                    tempPos1.x += mainCam.aspect * mainCam.orthographicSize;
                    tempPos2 = LevelEncounters.Encounters[i].position;  //DOWN LEFT
                    tempPos2.y -= mainCam.orthographicSize;
                    tempPos2.x -= mainCam.aspect * mainCam.orthographicSize;
                    Handles.DrawDottedLine(tempPos1, tempPos2, handlesLineScale); //DOWN LINE

                    tempPos1 = LevelEncounters.Encounters[i].position;
                    tempPos1.y += mainCam.orthographicSize;
                    tempPos1.x += mainCam.aspect * mainCam.orthographicSize;
                    tempPos2 = LevelEncounters.Encounters[i].position;
                    tempPos2.y -= mainCam.orthographicSize;
                    tempPos2.x += mainCam.aspect * mainCam.orthographicSize;
                    Handles.DrawDottedLine(tempPos1, tempPos2, handlesLineScale); //RIGHT LINE

                    tempPos1 = LevelEncounters.Encounters[i].position;
                    tempPos1.y += mainCam.orthographicSize;
                    tempPos1.x -= mainCam.aspect * mainCam.orthographicSize;
                    tempPos2 = LevelEncounters.Encounters[i].position;
                    tempPos2.y -= mainCam.orthographicSize;
                    tempPos2.x -= mainCam.aspect * mainCam.orthographicSize;
                    Handles.DrawDottedLine(tempPos1, tempPos2, handlesLineScale); //LEFT LINE
                }
                Handles.Label(LevelEncounters.Encounters[i].position, $"Enc: {i}", style);
            }

            #endregion
        }
        #endregion
#endif
    }
}
