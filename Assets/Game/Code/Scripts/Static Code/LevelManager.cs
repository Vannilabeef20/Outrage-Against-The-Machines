using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Singleton that manages Level specific information.
    /// </summary>
	public class LevelManager : MonoBehaviour
	{
        /// <summary>
        /// Static instance.
        /// </summary>
		public static LevelManager Instance { get; private set; }

        [Header("REFERENCES"), HorizontalLine(2f, EColor.Red)]
        [SerializeField, ReadOnly] private Camera mainCamera;


        [Header("PARAMETERS"), HorizontalLine(2f, EColor.Orange)]
        [Tooltip("Absoulte MinMax World Height(Y) coordinates for the *Play Zone*")]
        [SerializeField, MinMaxSlider(-7f, 7f)] private Vector2 playZoneHeight;

        [Tooltip("Half the Width(X) of the *Play Zone*")]
        [SerializeField, Min(0)] private float playZoneHalfWidth = 10.64f;
        [SerializeField] private Color playZoneColor;


        [Header("VARIABLES"), HorizontalLine(2f, EColor.Yellow)]
        [SerializeField, ReadOnly] private Vector3 point1Pos;
        [SerializeField, ReadOnly] private Vector3 point2Pos;

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
        public bool IsInsidePlayZone(Vector3 position)
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

        /// <summary>
        /// Clamps the given position value inside the "PlayZone" range.
        /// </summary>
        /// <param name="pos">The position to be clamped.</param>
        /// <returns>The given position, now clamped.</returns>
        public Vector3 ClampInsidePlayZone(Vector3 pos)
        {
            Vector3 clampedPos = Vector3.zero;
            Vector3 cameraPos = mainCamera.transform.position;
            clampedPos.x = Mathf.Clamp(pos.x, cameraPos.x - playZoneHalfWidth, cameraPos.x + playZoneHalfWidth);
            clampedPos.y = Mathf.Clamp(pos.y, playZoneHeight.x, playZoneHeight.y);
            clampedPos.z = Mathf.Clamp(pos.z, playZoneHeight.x, playZoneHeight.y);
            return clampedPos;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            #region Draw debug playzone box
            //Bottom Line
            point1Pos.x = mainCamera.transform.position.x + playZoneHalfWidth;
            point1Pos.y = playZoneHeight.x;
            point2Pos.x = mainCamera.transform.position.x - playZoneHalfWidth;
            point2Pos.y = playZoneHeight.x;
            Debug.DrawLine(point1Pos.ToXYY(), point2Pos.ToXYY(), playZoneColor);

            //Upper Line
            point1Pos.x = mainCamera.transform.position.x + playZoneHalfWidth;
            point1Pos.y = playZoneHeight.y;
            point2Pos.x = mainCamera.transform.position.x - playZoneHalfWidth;
            point2Pos.y = playZoneHeight.y;
            Debug.DrawLine(point1Pos.ToXYY(), point2Pos.ToXYY(), playZoneColor);

            //Left Line
            point1Pos.x = mainCamera.transform.position.x - playZoneHalfWidth;
            point1Pos.y = playZoneHeight.x;
            point2Pos.x = mainCamera.transform.position.x - playZoneHalfWidth;
            point2Pos.y = playZoneHeight.y;
            Debug.DrawLine(point1Pos.ToXYY(), point2Pos.ToXYY(), playZoneColor);

            //Right Line
            point1Pos.x = mainCamera.transform.position.x + playZoneHalfWidth;
            point1Pos.y = playZoneHeight.x;
            point2Pos.x = mainCamera.transform.position.x + playZoneHalfWidth;
            point2Pos.y = playZoneHeight.y;
            Debug.DrawLine(point1Pos.ToXYY(), point2Pos.ToXYY(), playZoneColor);
            #endregion
        }
#endif
    }
}