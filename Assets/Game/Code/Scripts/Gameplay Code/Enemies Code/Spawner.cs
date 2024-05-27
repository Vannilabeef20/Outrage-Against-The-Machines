using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using NaughtyAttributes;

namespace Game
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;
        private Vector2 point1Pos;
        private Vector2 point2Pos;
        [SerializeField] private FollowPos followPos;
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

        [Header("GIZMOS"), HorizontalLine(2f, EColor.Orange)]
        [SerializeField] private float encounterPositionGizmosRadius;
        [SerializeField] private Color encounterPositionGizmosColor;
        [SerializeField] private float encounterPositionDebugLineLenght;
        [SerializeField] private Color encounterPositionLimitDebugColor;


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
            followPos.active = false;
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
            followPos.CenterOnTarget();
            followPos.active = true;
           virtualCameraFramingTransposer.m_DeadZoneWidth = defaultDeadzoneWidtht;
            goImage.enabled = true;
        }


        public void SpawnEnemy()
        {
            Vector3 tempPosition = new Vector3(transform.position.x, transform.position.y, transform.position.y);
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
        private void OnDrawGizmos()
        {
           
            #region Draw debug spawn boxes
            //Furthest = Max distance .y, Closest = Min distance .x, Upper = MaxHeight .y, Lower = MinHeight .x, Left Blue, Right Magenta                                           
            //Furtherst upper right 
            point1Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.y, transform.position.z - spawnHeight.y);
            //Furtherst lower right                                                                                                    
            point2Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.x, transform.position.z - spawnHeight.x);
            Debug.DrawLine(point1Pos, point2Pos, spawnRegionColor);
            //In betweem furthest and closest upper right upper                                                                        
            point1Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.x, transform.position.z - spawnHeight.x);
            point2Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.x, transform.position.z - spawnHeight.x);
            Debug.DrawLine(point1Pos, point2Pos, spawnRegionColor);
            //In betweem furthest and closest lower right lower                                                                        
            point1Pos = new Vector3(transform.position.x + spawnDistance.y, transform.position.y + spawnHeight.y, transform.position.z - spawnHeight.y);
            point2Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.y, transform.position.z - spawnHeight.y);
            Debug.DrawLine(point1Pos, point2Pos, spawnRegionColor);
            //Closest upper right                                                                                                      
            point1Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.y, transform.position.z - spawnHeight.y);
            //Closest lower right                                                                                                      
            point2Pos = new Vector3(transform.position.x + spawnDistance.x, transform.position.y + spawnHeight.x, transform.position.z - spawnHeight.x);
            Debug.DrawLine(point1Pos, point2Pos, spawnRegionColor);
            //Furtherst upper left                                                                                                     
            point1Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.y, transform.position.z - spawnHeight.y);
            //Furtherst lower left                                                                                                     
            point2Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.x, transform.position.z - spawnHeight.x);
            Debug.DrawLine(point1Pos, point2Pos, spawnRegionColor);
            //Closest upper left                                                                                                       
            point1Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.y, transform.position.z - spawnHeight.y);
            //Closest lower left,                                                                                                      
            point2Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.x, transform.position.z - spawnHeight.x);
            Debug.DrawLine(point1Pos, point2Pos, spawnRegionColor);
            //In betweem furthest and closest Left upper
            point1Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.y, transform.position.z - spawnHeight.y);
            point2Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.y, transform.position.z - spawnHeight.y);
            Debug.DrawLine(point1Pos, point2Pos, spawnRegionColor);
            //In betweem furthest and closest Left lower
            point1Pos = new Vector3(transform.position.x - spawnDistance.y, transform.position.y + spawnHeight.x, transform.position.z - spawnHeight.x);
            point2Pos = new Vector3(transform.position.x - spawnDistance.x, transform.position.y + spawnHeight.x, transform.position.z - spawnHeight.x);
            Debug.DrawLine(point1Pos, point2Pos, spawnRegionColor);
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
                Gizmos.DrawSphere(encounter.encounterPosition, encounterPositionGizmosRadius);
            }

            #endregion
        }
#endif
    }
}
