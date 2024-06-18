using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class LevelManager : MonoBehaviour
	{
		public static LevelManager Instance { get; private set; }

        [SerializeField] private Camera mainCamera;
        [SerializeField, MinMaxSlider(-7f, 7f)] private Vector2 playZoneHeight;
        [SerializeField, Min(0)] private float playZoneWidth;
        [SerializeField] private Color playZoneColor;
        [SerializeField, ReadOnly] private Vector3 point1Pos, point2Pos;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }            
        }

        public bool IsInsidePlayZone(Vector3 vector)
        {
            vector.x -= mainCamera.transform.position.x;

            if (vector.x < -playZoneWidth)
            {
                return false;
            }
            if (vector.x > playZoneWidth)
            {
                return false;
            }
            if (vector.y < playZoneHeight.x)
            {
                return false;
            }
            if (vector.y > playZoneHeight.y)
            {
                return false;
            }
            if (vector.z < playZoneHeight.x)
            {
                return false;
            }
            if (vector.z > playZoneHeight.y)
            {
                return false;
            }
            return true;
        }

        public Vector3 ClampInsidePlayZone(Vector3 pos)
        {
            Vector3 clampedPos = Vector3.zero;
            Vector3 cameraPos = mainCamera.transform.position;
            clampedPos.x = Mathf.Clamp(pos.x, cameraPos.x - playZoneWidth, cameraPos.x + playZoneWidth);
            clampedPos.y = Mathf.Clamp(pos.y, playZoneHeight.x, playZoneHeight.y);
            clampedPos.z = Mathf.Clamp(pos.z, playZoneHeight.x, playZoneHeight.y);
            return clampedPos;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            #region Draw debug playzone box
            //Bottom Line
            point1Pos.x = mainCamera.transform.position.x + playZoneWidth;
            point1Pos.y = playZoneHeight.x;
            point2Pos.x = mainCamera.transform.position.x - playZoneWidth;
            point2Pos.y = playZoneHeight.x;
            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), playZoneColor);

            //Upper Line
            point1Pos.x = mainCamera.transform.position.x + playZoneWidth;
            point1Pos.y = playZoneHeight.y;
            point2Pos.x = mainCamera.transform.position.x - playZoneWidth;
            point2Pos.y = playZoneHeight.y;
            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), playZoneColor);

            //Left Line
            point1Pos.x = mainCamera.transform.position.x - playZoneWidth;
            point1Pos.y = playZoneHeight.x;
            point2Pos.x = mainCamera.transform.position.x - playZoneWidth;
            point2Pos.y = playZoneHeight.y;
            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), playZoneColor);

            //Right Line
            point1Pos.x = mainCamera.transform.position.x + playZoneWidth;
            point1Pos.y = playZoneHeight.x;
            point2Pos.x = mainCamera.transform.position.x + playZoneWidth;
            point2Pos.y = playZoneHeight.y;
            Debug.DrawLine(point1Pos.ToY2D(), point2Pos.ToY2D(), playZoneColor);
            #endregion
        }
#endif
    }
}