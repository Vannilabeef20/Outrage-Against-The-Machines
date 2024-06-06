using System;
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
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;
        [SerializeField] private Image goImage;

        [SerializeField, ReadOnly] private float defaultDeadzoneWidtht = 0f;
        [SerializeField, ReadOnly] private float maxDeadzoneWidtht = 2f;
        [SerializeField, ReadOnly] private CinemachineFramingTransposer virtualCameraFramingTransposer;

        [Header("SPAWN PARAMETERS"), HorizontalLine(2f, EColor.Red)]
        [SerializeField, Expandable] private List<EncounterSO> encounters;
        [SerializeField, ReadOnly] private List<GameObject> enemiesToSpawn;
        [ReadOnly] public List<GameObject> enemiesAlive;
        [MinMaxSlider(-20f, 20f), SerializeField] private Vector2 spawnDistance;
        [MinMaxSlider(-20f, 20f), SerializeField] private Vector2 spawnHeight;
        [MinMaxSlider(0.5f, 10f), SerializeField] private Vector2 spawnDelay;

        [SerializeField] private Color spawnRegionColor;

#if UNITY_EDITOR
        [SerializeField] private GameObject testSpawnEnemy;

        [Header("GIZMOS"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField] private float encounterPositionGizmosRadius;
        [SerializeField] private Color encounterPositionGizmosColor;
        [SerializeField] private float encounterPositionDebugLineLenght;
        [SerializeField] private Color encounterPositionLimitDebugColor;

        [SerializeField] private GUIStyle style;

        private Vector3 point1Pos;
        private Vector3 point2Pos;
#endif

        private void Awake()
        {
            virtualCameraFramingTransposer = FindObjectOfType<CinemachineVirtualCamera>().
                GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Backspace))
            {
                GameObject[] enemyObjects = enemiesAlive.ToArray();
                foreach(var enemy in enemyObjects)
                {
                    Destroy(enemy);
                }
                enemiesAlive.Clear();
                enemiesToSpawn.Clear();
            }
            if (encounters.Count == 0)
            {
                return;
            }
            if (transform.position.x > encounters[0].encounterPosition.x)
            {
                foreach (SpawnableEnemy _enemy in encounters[0].enemies)
                {
                    for (int i = 0; i < _enemy.amount; i++)
                    {
                        enemiesToSpawn.Add(_enemy.enemy);
                    }
                }
                enemiesToSpawn.ShuffleList();
                StartCoroutine(EncounterRoutine());
                encounters.RemoveAt(0);
            }
        }

        private IEnumerator EncounterRoutine()
        {
            goImage.enabled = false;
            virtualCameraFramingTransposer.m_DeadZoneWidth = maxDeadzoneWidtht;
            SpawnEnemy();
            while (enemiesToSpawn.Count > 0)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(spawnDelay.x, spawnDelay.y));
                SpawnEnemy();
            }
            while (enemiesAlive.Count > 0)
            {
                yield return null;
            }
           virtualCameraFramingTransposer.m_DeadZoneWidth = defaultDeadzoneWidtht;
            goImage.enabled = true;
        }


        public void SpawnEnemy()
        {
            if(enemiesToSpawn.Count < 1)
            {
                return;
            }
            Vector3 tempPosition = transform.position.ToY2D();
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
        [Button("Spawn test enemy")]
        public void SpawnTest()
        {
            Vector3 tempPosition = transform.position.ToY2D();
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
        private void OnDrawGizmos()
        {
           
            #region Draw debug spawn boxes
            //Furthest = Max distance .y, Closest = Min distance .x, Upper = MaxHeight .y, Lower = MinHeight .x, Left Blue, Right Magenta                                           
            //Furtherst upper right 
            point1Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.y,0);

            //Furtherst lower right                                                                              
            point2Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.x,0);

            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), spawnRegionColor);
            //In betweem furthest and closest upper right upper                                                  
            point1Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.x,0);

            point2Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.x,0);

            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), spawnRegionColor);
            //In betweem furthest and closest lower right lower                                                  
            point1Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.y,0);

            point2Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.y,0);

            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), spawnRegionColor);
            //Closest upper right                                                                                
            point1Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.y,0);

            //Closest lower right                                                                                
            point2Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.x,0);

            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), spawnRegionColor);
            //Furtherst upper left                                                                               
            point1Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.y,0);

            //Furtherst lower left                                                                               
            point2Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.x,0);

            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), spawnRegionColor);
            //Closest upper left                                                                                 
            point1Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.y,0);

            //Closest lower left,                                                                                
            point2Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.x,0);

            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), spawnRegionColor);
            //In betweem furthest and closest Left upper
            point1Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.y,0);

            point2Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.y,0);

            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), spawnRegionColor);
            //In betweem furthest and closest Left lower
            point1Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.x,0);

            point2Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.x,0);

            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), spawnRegionColor);
            #endregion
            #region Draw debug enconter regions limit
            if (encounters.Count == 0)
            {
                return;
            }
            foreach (EncounterSO encounter in encounters)
            {
                Gizmos.color = encounterPositionGizmosColor;
                if(mainCam != null)
                {
                    Vector3 tempPos1 = encounter.encounterPosition; //UP RIGHT
                    tempPos1.y += mainCam.orthographicSize; 
                    tempPos1.x += mainCam.aspect * mainCam.orthographicSize;
                    Vector3 tempPos2 = encounter.encounterPosition; //UP LEFT 
                    tempPos2.y += mainCam.orthographicSize;
                    tempPos2.x -= mainCam.aspect * mainCam.orthographicSize;
                    Debug.DrawLine(tempPos1, tempPos2, Color.blue); //UP LINE

                    tempPos1 = encounter.encounterPosition; //DOWN RIGHT
                    tempPos1.y -= mainCam.orthographicSize;
                    tempPos1.x += mainCam.aspect * mainCam.orthographicSize;
                    tempPos2 = encounter.encounterPosition;  //DOWN LEFT
                    tempPos2.y -= mainCam.orthographicSize;
                    tempPos2.x -= mainCam.aspect * mainCam.orthographicSize;
                    Debug.DrawLine(tempPos1, tempPos2, Color.blue); //DOWN LINE

                    tempPos1 = encounter.encounterPosition;
                    tempPos1.y += mainCam.orthographicSize;
                    tempPos1.x += mainCam.aspect * mainCam.orthographicSize;
                    tempPos2 = encounter.encounterPosition;
                    tempPos2.y -= mainCam.orthographicSize;
                    tempPos2.x += mainCam.aspect * mainCam.orthographicSize;
                    Debug.DrawLine(tempPos1, tempPos2, Color.blue); //RIGHT LINE

                    tempPos1 = encounter.encounterPosition;
                    tempPos1.y += mainCam.orthographicSize;
                    tempPos1.x -= mainCam.aspect * mainCam.orthographicSize;
                    tempPos2 = encounter.encounterPosition;
                    tempPos2.y -= mainCam.orthographicSize;
                    tempPos2.x -= mainCam.aspect * mainCam.orthographicSize;
                    Debug.DrawLine(tempPos1, tempPos2, Color.blue); //LEFT LINE
                }
                Handles.Label(encounter.encounterPosition, encounter.name, style);
            }

            #endregion
        }
#endif
    }
}
