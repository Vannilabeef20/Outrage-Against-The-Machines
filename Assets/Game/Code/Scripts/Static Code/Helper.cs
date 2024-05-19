using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game 
{
	public static class Helper
	{
		/// <summary>
		/// Checks if a Vector3 is within a certain range.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns>Returns true if within range.</returns>
		public static bool InsideRange(this Vector3 value, Vector3 min, Vector3 max)
        {
			if(value.x < min.x)
            {
				return false;
            }
			if(value.y < min.y)
            {
				return false;
            }
			if(value.z < min.z)
            {
				return false;
			}
			if (value.x > max.x)
			{
				return false;
			}
			if (value.y > max.y)
			{
				return false;
			}
			if (value.z > max.z)
			{
				return false;
			}
			return true;
		}
		/// <summary>
		/// Checks if a Vector2 is within a certain range.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static bool InsideRange(this Vector2 value, Vector2 min, Vector2 max)
		{
			if (value.x < min.x)
			{
				return false;
			}
			if (value.y < min.y)
			{
				return false;
			}
			if (value.x > max.x)
			{
				return false;
			}
			if (value.y > max.y)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Checks if a layer is present in a given mask.
		/// </summary>
		/// <returns>True if layer is present.</returns>
		/// 

		public static bool ContainsLayer(this LayerMask layerMask, int layer)
		{
			if ((layerMask.value & (1 << layer)) > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		public static float Map(this float value, float oldLow, float oldHigh, float newLow = 0f, float newHigh = 1f, bool clamp = true)
		{
			float temp = (value - oldLow) / (oldHigh - oldLow);
			temp = newLow + (newHigh - newLow) * temp;
			if (clamp)
            {
				temp = Mathf.Clamp(temp, newLow, newHigh);
            }
			else
            {
				temp = newLow;
            }
			return temp;

		}

        /// <summary>
        /// Randomly shuffles given list.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="targetList">List to be shuffled</param>
        public static void ShuffleList<T>(this List<T> targetList)
		{
			for (int i = 0; i < targetList.Count - 1; i++)
			{
				T temp = targetList[i];
				int rand = UnityEngine.Random.Range(i, targetList.Count);
				targetList[i] = targetList[rand];
				targetList[rand] = temp;
			}
		}

		#region Debug

		#region LogFormatting

		public static string EnrichString(string message)
		{
			return $"<color=yellow>{message}</color>";
		}
		public static string FormatLog(string message, Color color)
		{
			string endcolorString = " </color>";
			string colorString = $"<color={ColorUtility.ToHtmlStringRGB(color)}> ";
			string finalLog = colorString;
			finalLog += message;
			finalLog += endcolorString;
			return finalLog;
		}

		#endregion

		#region Box/boxcast
		/// <summary>
		/// Draws a Boxcast with its natural parameters.
		/// </summary>
		/// <param name="origin">World position the Boxcast originates from.</param>
		/// <param name="halfExtents">Half the lenght of the box to be cast.</param>
		/// <param name="orientation">Rotation to be applied to the box before the cast.</param>
		/// <param name="direction">The direction the cast will travel to.</param>
		/// <param name="distance">How far the cast will go.</param>
		/// <param name="color">The color of the cast.</param>
		public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
		{
			direction.Normalize();
			Box bottomBox = new Box(origin, halfExtents, orientation);
			Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

			Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
			Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
			Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
			Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
			Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
			Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
			Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
			Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

			DrawBox(bottomBox, color);
			DrawBox(topBox, color);
		}

		/// <summary>
		/// Draws a Box.
		/// </summary>
		/// <param name="box"></param>
		/// <param name="color"></param>
		public static void DrawBox(Box box, Color color)
		{
			Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
			Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
			Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
			Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

			Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
			Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
			Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
			Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

			Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
			Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
			Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
			Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
		}

		public struct Box
		{
			public Vector3 localFrontTopLeft { get; private set; }
			public Vector3 localFrontTopRight { get; private set; }
			public Vector3 localFrontBottomLeft { get; private set; }
			public Vector3 localFrontBottomRight { get; private set; }
			public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
			public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
			public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
			public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

			public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
			public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
			public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
			public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
			public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
			public Vector3 backTopRight { get { return localBackTopRight + origin; } }
			public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
			public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

			public Vector3 origin { get; private set; }

			public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
			{
				Rotate(orientation);
			}
			public Box(Vector3 origin, Vector3 halfExtents)
			{
				this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
				this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
				this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
				this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

				this.origin = origin;
			}


			private void Rotate(Quaternion orientation)
			{
				localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
				localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
				localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
				localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
			}
		}

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
		{
			Vector3 direction = point - pivot;
			return pivot + rotation * direction;
		}
        #endregion

		#endregion
	}
}
