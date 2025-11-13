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
        [Tooltip("The distance range from the followCam the enemy can spawn.")]
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

            virtualCameraFramingTransposer = Camera.main.transform.parent.GetComponentInChildren<CinemachineVirtualCamera>().
                GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Backspace))
            {
                DestroyAll();
            }
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
                while (enemiesToSpawn.Count > 0)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(Random.Range(MinSpawnDelay, MaxSpawnDelay));
                }

                //Wait till all enemies are dead
                if (currentEncounterIndex == LevelEncounters.Encounters.Length - 1)
                {
                    while (enemiesAlive.Count > 1) yield return null;
                }
                else
                {
                    while (enemiesAlive.Count > 0) yield return null;
                }
            }

            if (currentEncounterIndex == LevelEncounters.Encounters.Length - 1)
            {
                encCoroutine = StartCoroutine (EncounterRoutine());
                yield break;
            }

            //End
            virtualCameraFramingTransposer.m_DeadZoneWidth = defaultDeadzoneWidtht;
            goImage.enabled = true;
            currentEncounterIndex++;
            encCoroutine = null;
        }


        public void SpawnEnemy()
        {
            if(enemiesToSpawn.Count < 1) return;

            Vector3 tempPosition = transform.position;

            int dirX;
            dirX = Random.Range(0, 2) == 0 ? 1 : -1;
            float randomX = Random.Range(spawnDistance.x, spawnDistance.y) * dirX;

            float randomZ = Random.Range(spawnHeight.x, spawnHeight.y);

            Instantiate(enemiesToSpawn[0], tempPosition + new Vector3(randomX, 0, randomZ), Quaternion.identity, transform);
            enemiesToSpawn.RemoveAt(0);
        }

#if UNITY_EDITOR
        #region DEBUG & GIZMOS
        /// <summary>
        /// Spawns a cube inside the spawn zone to test its acurracy.
        /// </summary>
        [Button("Spawn testValue enemy cube")]
        public void SpawnTest()
        {
            Vector3 tempPosition = transform.position;

            int dirX;
            dirX = Random.Range(0, 2) == 0 ? 1 : -1;
            float randomX = Random.Range(spawnDistance.x, spawnDistance.y) * dirX;

            float randomZ = Random.Range(spawnHeight.x, spawnHeight.y);

            Instantiate(testSpawnEnemy, tempPosition + new Vector3(randomX, 0, randomZ), Quaternion.identity, transform);
        }

        /// <summary>
        /// Destroys all enemies in "enemiesAlive" and "enemiesToSpawn".
        /// </summary>
        [Button("Destroy all enemies")]
        public void DestroyAll()
        {
            int count = -1;
            foreach (var encounter in LevelEncounters.Encounters)
            {
                count++;
                encounterEvent.Raise(this, currentEncounterIndex);
                if (encounter.position.x <= transform.position.x) currentEncounterIndex = count;
            }

            GameObject[] enemyObjects = enemiesAlive.ToArray();
            foreach (var enemy in enemyObjects)
            {
                Destroy(enemy);
            }
            enemiesAlive.Clear();
            enemiesToSpawn.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            #region DRAW SPAWN BOXES
            Handles.color = handlesSpawnLineColor;

            // Compute the 8 corner points of the box
            Vector3 center = transform.position;

            Vector3[] corners = new Vector3[8];

            // Right side
            //corners[0] = new Vector3(center.x + spawnDistance.x, center.y + spawnHeight.x, center.z + spawnDepth.x); // right lower close
            //corners[1] = new Vector3(center.x + spawnDistance.x, center.y + spawnHeight.y, center.z + spawnDepth.x); // right upper close
            //corners[2] = new Vector3(center.x + spawnDistance.x, center.y + spawnHeight.x, center.z + spawnDepth.y); // right lower far
            //corners[3] = new Vector3(center.x + spawnDistance.x, center.y + spawnHeight.y, center.z + spawnDepth.y); // right upper far

            // Left side
            //corners[4] = new Vector3(center.x - spawnDistance.y, center.y + spawnHeight.x, center.z + spawnDepth.x); // left lower close
            //corners[5] = new Vector3(center.x - spawnDistance.y, center.y + spawnHeight.y, center.z + spawnDepth.x); // left upper close
            //corners[6] = new Vector3(center.x - spawnDistance.y, center.y + spawnHeight.x, center.z + spawnDepth.y); // left lower far
            //corners[7] = new Vector3(center.x - spawnDistance.y, center.y + spawnHeight.y, center.z + spawnDepth.y); // left upper far

            float s = handlesLineScale;

            // Connect edges (12 lines)
            void Line(int a, int b) => Handles.DrawDottedLine(corners[a], corners[b], s);

            // Vertical edges
            Line(0, 1); Line(2, 3); Line(4, 5); Line(6, 7);

            // Depth edges
            Line(0, 2); Line(1, 3); Line(4, 6); Line(5, 7);

            // Width edges
            Line(0, 4); Line(1, 5); Line(2, 6); Line(3, 7);
            #endregion

        }
        private void OnDrawGizmos()
        {
            #region DRAW ENCOUNTER ZONE
            Handles.color = handlesEncounterLineColor;
            if (LevelEncounters.Encounters.Length == 0)
            {
                return;
            }

            for (int i = 0; i < LevelEncounters.Encounters.Length; i++)
            {
                if (mainCam != null)
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
