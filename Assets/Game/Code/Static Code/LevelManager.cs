using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    /// <summary>
    /// Singleton that manages Level specific information.
    /// </summary>
	public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField, ReadOnly] private Camera mainCamera;

        [Header("PLAYZONE"), HorizontalLine(2f, EColor.Orange)]
        [Tooltip("Absoulte MinMax World Height(Y) coordinates for the *Play Zone*")]
        [SerializeField, MinMaxSlider(-7f, 7f)] private Vector2 playZoneHeight;

        [Tooltip("Half the Width(X) of the *Play Zone*")]
        [SerializeField, Min(0)] private float playZoneHalfWidth = 10.64f;
        [SerializeField] private Color playZoneColor;

#if UNITY_EDITOR
        [SerializeField] float playZoneLineScale;

        Vector3 point1Pos;
        Vector3 point2Pos;
#endif

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                mainCamera = Camera.main;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Checks if X,Y and Z coordinates in the given position are within the "Play Zone" range.
        /// </summary>
        /// <param name="position">The position to be checked.</param>
        /// <returns>False if any of the values are higher or lower than the "Play Zone" range.</returns>
        public bool IsInsidePlayzone(Vector3 position)
        {
            position.x -= mainCamera.transform.position.x;
            
            if (position.x < -playZoneHalfWidth)
            {
                return false; // too far left
            }
            if (position.x > playZoneHalfWidth)
            {
                return false; //Too far right
            }
            if (position.y < playZoneHeight.x)
            {
                return false; //Too low
            }
            if (position.y > playZoneHeight.y)
            {
                return false; //Too High
            }
            if (position.z < playZoneHeight.x)
            {
                return false; //Too close
            }
            if (position.z > playZoneHeight.y)
            {
                return false; //Too far
            }
            return true;
        }

        public Vector3 ClampInsidePlayzone(Vector3 position)
        {
            position.x -= mainCamera.transform.position.x;

            if (position.x < -playZoneHalfWidth)
            {
                position.x = -playZoneHalfWidth;
            }
            if (position.x > playZoneHalfWidth)
            {
                position.x = playZoneHalfWidth;
            }
            if (position.y < playZoneHeight.x)
            {
                position.y = playZoneHeight.x;
            }
            if (position.y > playZoneHeight.y)
            {
                position.y = playZoneHeight.y;
            }
            if (position.z < playZoneHeight.x)
            {
                position.z = playZoneHeight.x;
            }
            if (position.z > playZoneHeight.y)
            {
                position.z = playZoneHeight.y;
            }
            
            position.x += mainCamera.transform.position.x;
            return position;
        }

#if UNITY_EDITOR      
        private void OnDrawGizmos()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            #region Draw debug playzone box
            Handles.color = playZoneColor;
            //Bottom Line
            point1Pos.x = mainCamera.transform.position.x + playZoneHalfWidth;
            point1Pos.y = playZoneHeight.x;
            point2Pos.x = mainCamera.transform.position.x - playZoneHalfWidth;
            point2Pos.y = playZoneHeight.x;
            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), playZoneLineScale);

            //Upper Line
            point1Pos.x = mainCamera.transform.position.x + playZoneHalfWidth;
            point1Pos.y = playZoneHeight.y;
            point2Pos.x = mainCamera.transform.position.x - playZoneHalfWidth;
            point2Pos.y = playZoneHeight.y;
            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), playZoneLineScale);

            //Left Line
            point1Pos.x = mainCamera.transform.position.x - playZoneHalfWidth;
            point1Pos.y = playZoneHeight.x;
            point2Pos.x = mainCamera.transform.position.x - playZoneHalfWidth;
            point2Pos.y = playZoneHeight.y;
            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), playZoneLineScale);

            //Right Line
            point1Pos.x = mainCamera.transform.position.x + playZoneHalfWidth;
            point1Pos.y = playZoneHeight.x;
            point2Pos.x = mainCamera.transform.position.x + playZoneHalfWidth;
            point2Pos.y = playZoneHeight.y;
            Handles.DrawDottedLine(point1Pos.ToXYY(), point2Pos.ToXYY(), playZoneLineScale);
            #endregion
        }
#endif
    }
}