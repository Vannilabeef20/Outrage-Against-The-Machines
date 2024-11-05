using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game 
{
	/// <summary>
	/// Static class for Utility stuff that does not require instancing.
	/// </summary>
	public static class Helper
	{
		#region MAPPING METHODS
		/// <summary>
		/// Maps this float value from a range to another one.
		/// </summary>
		/// <param name="value">The given float.</param>
		/// <param name="oldLow">Lowest value in the current range.</param>
		/// <param name="oldHigh">Highest value in the current range.</param>
		/// <param name="newLow">Lowest value in the new range.</param>
		/// <param name="newHigh">Highest value in the new range.</param>
		/// <param name="clamp">Whether to clamp "value" between the "newLow" and "newHigh".</param>
		/// <returns>The mapped value.<br/>
		/// Corresponds to the equivalent value of "value" in the new range
		/// </returns>
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
		/// Swizzles this Vector3 from XYZ to XYY.<br/>
		/// (AKA Copies the Y value of this Vector3 to the Z value).
		/// </summary>
		/// <param name="vector">The given Vector3.</param>
		/// <returns>Given Vector3 swizzled to XYY.</returns>
		public static Vector3 ToXYY(this Vector3 vector)
        {
			return new Vector3(vector.x, vector.y, vector.y);
        }
		/// <summary>
		/// Swizzles this Vector3 from XYZ to XZZ.<br/>
		/// AKA Copies the Z value of this Vector3 to the Y value.
		/// </summary>
		/// <param name="vector">The given Vector3.</param>
		/// <returns>Given Vector3 swizzled to XZZ.</returns>
		public static Vector3 ToXZZ(this Vector3 vector)
		{
			return new Vector3(vector.x, vector.z, vector.z);
		}
        #endregion

        /// <summary>
        /// Checks if this Vector3 has its values between a "min" and a "max" Vector3.
        /// </summary>
        /// <param name="vector">The given Vector3.</param>
        /// <param name="min">Minimum Vector3 inside the range.</param>
        /// <param name="max">Maximum Vector3 inside the range.</param>
        /// <returns> Returns "False" if any of the "vector" x,y or z values is
        /// below its counterpart in "min", or higher than "max".</returns>
        public static bool InsideRange(this Vector3 vector, Vector3 min, Vector3 max)
        {
			if(vector.x < min.x)
            {
				return false;
            }
			if(vector.y < min.y)
            {
				return false;
            }
			if(vector.z < min.z)
            {
				return false;
			}
			if (vector.x > max.x)
			{
				return false;
			}
			if (vector.y > max.y)
			{
				return false;
			}
			if (vector.z > max.z)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Checks if this Vector2 has its values between a "min" and a "max" Vector2.
		/// </summary>
		/// <param name="vector">The given Vector2.</param>
		/// <param name="min">Minimum Vector2 inside the range.</param>
		/// <param name="max">Maximum Vector2 inside the range.</param>
		/// <returns> Returns "False" if any of the "vector" x or y values are
		/// below its counterpart in "min", or higher than "max".</returns>
		public static bool InsideRange(this Vector2 vector, Vector2 min, Vector2 max)
		{
			if (vector.x < min.x)
			{
				return false;
			}
			if (vector.y < min.y)
			{
				return false;
			}
			if (vector.x > max.x)
			{
				return false;
			}
			if (vector.y > max.y)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Absolutes all values in this Vector3.
		/// </summary>
		/// <param name="vector3">The given Vector3.</param>
		/// <returns>The given Vector3 Absoluted<br/>
		/// (AKA only positive values).</returns>
		public static Vector3 Absolute(this Vector3 vector3)
        {
			Math.Abs(vector3.x);
			Math.Abs(vector3.y);
			Math.Abs(vector3.z);
			return vector3;
		}

		/// <summary>
		/// Absoultes all values in this Vector2.
		/// </summary>
		/// <param name="vector2">The given Vector2.</param>
		/// <returns>The given Vector3 Absoluted<br/>
		/// (AKA only positive values).</returns>
		public static Vector2 Absolute(this Vector2 vector2)
		{
			Math.Abs(vector2.x);
			Math.Abs(vector2.y);
			return vector2;
		}

		public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
		{
			Vector3 clampedVector;
			clampedVector.x = Mathf.Clamp(vector.x, min.x, max.x);
			clampedVector.y = Mathf.Clamp(vector.y, min.y, max.y);
			clampedVector.z = Mathf.Clamp(vector.z, min.z, max.z);
			return clampedVector;
		}
		public static Vector2 Clamp(this Vector2 vector, Vector2 min, Vector2 max)
		{
			Vector2 clampedVector;
			clampedVector.x = Mathf.Clamp(vector.x, min.x, max.x);
			clampedVector.y = Mathf.Clamp(vector.y, min.y, max.y);
			return clampedVector;
		}

		/// <summary>
		/// Checks if this enum has a given flag.
		/// </summary>
		/// <typeparam name="TEnum"></typeparam>
		/// <param name="e"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static bool HasAnyFlag<TEnum>(this TEnum e, TEnum flag) where TEnum : IConvertible
        {
			if (e.GetType() != flag.GetType())
            {
				CustomLogger.LogError(Sender:"Helper",Message:"enum types should be the same type.");
				return false;
            }
			return (e.ToInt32(null) & flag.ToInt32(null)) != 0;
		}

		/// <summary>
		/// Checks if a given layer is present in this LayerMask.
		/// </summary>
		/// <param name="layerMask">The given LayerMask.</param>
		/// <param name="layer">The given Layer.</param>
		/// <returns>True if layer is present.</returns>
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


        /// <summary>
        /// Randomly shuffles this list.
        /// </summary>
        /// <typeparam name="T">Data "Type".</typeparam>
        /// <param name="targetList">List to be shuffled.</param>
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

		/// <summary>
		/// Creates a new color based on another one with the same RGB values and a new Alpha.
		/// </summary>
		/// <param name="color">The color that will be changed.</param>
		/// <param name="newAlpha">The new alpha value for the color.</param>
		/// <returns>The same given color with a new Alpha.</returns>
		public static Color ChangeAlpha(this Color color, float newAlpha)
        {
			return new Color(color.r, color.g, color.b, newAlpha);
        }

		public static bool IsVisible(this Camera cam, Vector3 worldPos)
		{
			Vector2 viewportPos = cam.WorldToViewportPoint(worldPos);
			if (viewportPos.InsideRange(Vector2.zero, Vector2.one))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static (int hundreds, int tens, int units) DecomposeDecimal(int totalUnits)
        {
			// Decompose drop amount in hundreds tens and units
			int temp_hundreds = (int)(totalUnits / 100f);
			int tempRemainder = totalUnits % 100;

			int temp_tens = (int)(tempRemainder / 10f);
			tempRemainder %= 10;

			int temp_units = tempRemainder;

			return (temp_hundreds, temp_tens, temp_units);
		}

		#region DEBUG

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

			Debug.DrawLine(bottomBox.BackBottomLeft, topBox.BackBottomLeft, color);
			Debug.DrawLine(bottomBox.BackBottomRight, topBox.BackBottomRight, color);
			Debug.DrawLine(bottomBox.BackTopLeft, topBox.BackTopLeft, color);
			Debug.DrawLine(bottomBox.BackTopRight, topBox.BackTopRight, color);
			Debug.DrawLine(bottomBox.FrontTopLeft, topBox.FrontTopLeft, color);
			Debug.DrawLine(bottomBox.FrontTopRight, topBox.FrontTopRight, color);
			Debug.DrawLine(bottomBox.FrontBottomLeft, topBox.FrontBottomLeft, color);
			Debug.DrawLine(bottomBox.FrontBottomRight, topBox.FrontBottomRight, color);

			DrawBox(bottomBox, color);
			DrawBox(topBox, color);
		}

		/// <summary>
		/// Draws a Box via Debug.Drawline().
		/// </summary>
		/// <param name="box">The struct representation of a box.</param>
		/// <param name="color">The color of the box made of Debug.DrawLine().</param>
		public static void DrawBox(Box box, Color color)
		{
			Debug.DrawLine(box.FrontTopLeft, box.FrontTopRight, color);
			Debug.DrawLine(box.FrontTopRight, box.FrontBottomRight, color);
			Debug.DrawLine(box.FrontBottomRight, box.FrontBottomLeft, color);
			Debug.DrawLine(box.FrontBottomLeft, box.FrontTopLeft, color);

			Debug.DrawLine(box.BackTopLeft, box.BackTopRight, color);
			Debug.DrawLine(box.BackTopRight, box.BackBottomRight, color);
			Debug.DrawLine(box.BackBottomRight, box.BackBottomLeft, color);
			Debug.DrawLine(box.BackBottomLeft, box.BackTopLeft, color);

			Debug.DrawLine(box.FrontTopLeft, box.BackTopLeft, color);
			Debug.DrawLine(box.FrontTopRight, box.BackTopRight, color);
			Debug.DrawLine(box.FrontBottomRight, box.BackBottomRight, color);
			Debug.DrawLine(box.FrontBottomLeft, box.BackBottomLeft, color);
		}

		public static void DrawBox(Vector3 center, Vector3 HalfExtents, Quaternion orientation, Color color)
		{
			Box box = new Box(center, HalfExtents, orientation);

			Debug.DrawLine(box.FrontTopLeft, box.FrontTopRight, color);
			Debug.DrawLine(box.FrontTopRight, box.FrontBottomRight, color);
			Debug.DrawLine(box.FrontBottomRight, box.FrontBottomLeft, color);
			Debug.DrawLine(box.FrontBottomLeft, box.FrontTopLeft, color);

			Debug.DrawLine(box.BackTopLeft, box.BackTopRight, color);
			Debug.DrawLine(box.BackTopRight, box.BackBottomRight, color);
			Debug.DrawLine(box.BackBottomRight, box.BackBottomLeft, color);
			Debug.DrawLine(box.BackBottomLeft, box.BackTopLeft, color);

			Debug.DrawLine(box.FrontTopLeft, box.BackTopLeft, color);
			Debug.DrawLine(box.FrontTopRight, box.BackTopRight, color);
			Debug.DrawLine(box.FrontBottomRight, box.BackBottomRight, color);
			Debug.DrawLine(box.FrontBottomLeft, box.BackBottomLeft, color);
		}



		/// <summary>
		/// Represents a wireframe box for easier debugging.
		/// </summary>
		public struct Box
		{
			public Vector3 LocalFrontTopLeft { get; private set; }
			public Vector3 LocalFrontTopRight { get; private set; }
			public Vector3 LocalFrontBottomLeft { get; private set; }
			public Vector3 LocalFrontBottomRight { get; private set; }
			public Vector3 LocalBackTopLeft { get { return -LocalFrontBottomRight; } }
			public Vector3 LocalBackTopRight { get { return -LocalFrontBottomLeft; } }
			public Vector3 LocalBackBottomLeft { get { return -LocalFrontTopRight; } }
			public Vector3 LocalBackBottomRight { get { return -LocalFrontTopLeft; } }

			public Vector3 FrontTopLeft { get { return LocalFrontTopLeft + Origin; } }
			public Vector3 FrontTopRight { get { return LocalFrontTopRight + Origin; } }
			public Vector3 FrontBottomLeft { get { return LocalFrontBottomLeft + Origin; } }
			public Vector3 FrontBottomRight { get { return LocalFrontBottomRight + Origin; } }
			public Vector3 BackTopLeft { get { return LocalBackTopLeft + Origin; } }
			public Vector3 BackTopRight { get { return LocalBackTopRight + Origin; } }
			public Vector3 BackBottomLeft { get { return LocalBackBottomLeft + Origin; } }
			public Vector3 BackBottomRight { get { return LocalBackBottomRight + Origin; } }

			public Vector3 Origin { get; private set; }

			public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
			{
				Rotate(orientation);
			}
			public Box(Vector3 origin, Vector3 halfExtents)
			{
				this.LocalFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
				this.LocalFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
				this.LocalFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
				this.LocalFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

				this.Origin = origin;
			}


			private void Rotate(Quaternion orientation)
			{
				LocalFrontTopLeft = RotatePointAroundPivot(LocalFrontTopLeft, Vector3.zero, orientation);
				LocalFrontTopRight = RotatePointAroundPivot(LocalFrontTopRight, Vector3.zero, orientation);
				LocalFrontBottomLeft = RotatePointAroundPivot(LocalFrontBottomLeft, Vector3.zero, orientation);
				LocalFrontBottomRight = RotatePointAroundPivot(LocalFrontBottomRight, Vector3.zero, orientation);
			}
		}

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
		{
			Vector3 direction = point - pivot;
			return pivot + rotation * direction;
		}
		#endregion

		#region Arrows
		/// <summary>
		/// Draws an arrow via Debug.Drawline() from "pointFrom" to "pointTo".
		/// </summary>
		/// <param name="pointFrom">Where the arrow originates from.</param>
		/// <param name="pointTo">Where the arrow ends at.</param>
		/// <param name="arrowBodyColor">Color for the body of the arrow.</param>
		/// <param name="arrowHeadColor">Color for the head of the arrow.</param>
		/// <param name="arrowHeadSize">The size of the head of the arrow.</param>
		/// <param name="arrowHeadAngle">The angle that the arrowHead lines are rotated away from the body.</param>
		public static void DrawPointArrow(Vector3 pointFrom, Vector3 pointTo, Color arrowBodyColor, Color arrowHeadColor, float arrowHeadSize = 1f, float arrowHeadAngle = 20.0f)
		{
			if (pointFrom == pointTo)
			{
				return;
			}
			Debug.DrawLine(pointFrom, pointTo, arrowBodyColor);
			Vector3 dir = pointTo - pointFrom;
			Vector3 up = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 down = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 front = Quaternion.LookRotation(dir, Vector3.forward) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 behind = Quaternion.LookRotation(dir, Vector3.forward) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(dir, Vector3.right) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 right = Quaternion.LookRotation(dir, Vector3.right) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

			Debug.DrawLine(pointTo, pointTo + up.normalized * arrowHeadSize, arrowHeadColor);
			Debug.DrawLine(pointTo, pointTo + down.normalized * arrowHeadSize, arrowHeadColor);
			Debug.DrawLine(pointTo, pointTo + front.normalized * arrowHeadSize, arrowHeadColor);
			Debug.DrawLine(pointTo, pointTo + behind.normalized * arrowHeadSize, arrowHeadColor);
			Debug.DrawLine(pointTo, pointTo + left.normalized * arrowHeadSize, arrowHeadColor);
			Debug.DrawLine(pointTo, pointTo + right.normalized * arrowHeadSize, arrowHeadColor);
		}

		/// <summary>
		/// Draws an arrow via Debug.Drawline() from "pointFrom" towards "directionTo".
		/// </summary>
		/// <param name="pointFrom">Where the arrow originates from.</param>
		/// <param name="directionTo">The direction that the arrow will point to.</param>
		/// <param name="arrowBodyColor">Color for the body of the arrow.</param>
		/// <param name="arrowHeadColor">Color for the head of the arrow.</param>
		/// <param name="arrowHeadSize">The size of the head of the arrow.</param>
		/// <param name="arrowHeadAngle">The angle that the arrowHead lines are rotated away from the body.</param>
		public static void DrawDirArrow(Vector3 pointFrom, Vector3 directionTo, Color arrowBodyColor, Color arrowHeadColor, float arrowHeadSize = 1f, float arrowHeadAngle = 20.0f)
		{
			if(Vector3.zero == directionTo)
            {
				return;
            }
			Vector3 pointTo = pointFrom + directionTo;
			Debug.DrawLine(pointFrom, pointTo, arrowBodyColor);
			Vector3 up = Quaternion.LookRotation(directionTo) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 down = Quaternion.LookRotation(directionTo) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 front = Quaternion.LookRotation(directionTo, Vector3.forward) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 behind = Quaternion.LookRotation(directionTo, Vector3.forward) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(directionTo, Vector3.right) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 right = Quaternion.LookRotation(directionTo, Vector3.right) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

			Debug.DrawLine(pointTo, pointTo + up.normalized * arrowHeadSize, arrowHeadColor);
			Debug.DrawLine(pointTo, pointTo + down.normalized * arrowHeadSize, arrowHeadColor);
			Debug.DrawLine(pointTo, pointTo + front.normalized * arrowHeadSize, arrowHeadColor);
			Debug.DrawLine(pointTo, pointTo + behind.normalized * arrowHeadSize, arrowHeadColor);
			Debug.DrawLine(pointTo, pointTo + left.normalized * arrowHeadSize, arrowHeadColor);
			Debug.DrawLine(pointTo, pointTo + right.normalized * arrowHeadSize, arrowHeadColor);
		}

		#endregion

		#endregion
	}
}
