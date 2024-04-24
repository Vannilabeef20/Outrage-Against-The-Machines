using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
	public static class StaticHelper
	{
		/// <summary>
		/// Checks if a layer is present in a given mask.
		/// </summary>
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


		public static float Map(this float value, float minRawValue, float maxRawValue, float minFinalValue, float maxFinalValue, bool clamp = false)
		{
			float val = minFinalValue + (maxFinalValue - minFinalValue) * ((value - minRawValue) / (maxRawValue - minRawValue));

			return clamp ? Mathf.Clamp(val, Mathf.Min(minFinalValue, maxFinalValue), Mathf.Max(minFinalValue, maxFinalValue)) : val;
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


		//Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
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


			public void Rotate(Quaternion orientation)
			{
				localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
				localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
				localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
				localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
			}
		}

		static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
		{
			Vector3 direction = point - pivot;
			return pivot + rotation * direction;
		}
	}
}
