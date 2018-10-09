using UnityEngine;

namespace GameUI
{
	[ExecuteInEditMode]
	public class CircularLayout : MonoBehaviour
	{
		#region INSTANCE_VARS
		public bool Expanded { get; set; }
		public float Radius { get { return distanceToCenter; } }

		[SerializeField]
		private float initialRotation = 90f;
		private float distanceToCenter;
		[SerializeField]
		private float constrictedDistance = 50f;
		[SerializeField]
		private float expandedDistance = 100f;
		[SerializeField]
		private float expandSpeed = 5f;
		#endregion

		#region INSTANCE_METHODS
		public void Awake()
		{
			Expanded = false;
			distanceToCenter = constrictedDistance;
		}

		public void Update()
		{
			ApplyLayout ();
			distanceToCenter = Mathf.Lerp (distanceToCenter, Expanded ? expandedDistance : constrictedDistance, Time.deltaTime * expandSpeed);
		}

		private void ApplyLayout()
		{
			//in the case of one child, do not apply offset or rotation
			if (transform.childCount == 1)
			{
				transform.GetChild (0).localPosition = Vector2.zero;
				return;
			}

			//two or more children
			for (int i = 0; i < transform.childCount; i++)
			{
				//determine rotation
				float theta = (i * (360f / transform.childCount)) + initialRotation;
				theta *= Mathf.Deg2Rad;
				Transform petal = transform.GetChild (i);

				//set distance from center
				Vector2 unrotatedPos =  new Vector2 (distanceToCenter, 0f);

				//rotate that bitch
				petal.localPosition = new Vector2 (
					(unrotatedPos.x * Mathf.Cos (theta)) - (unrotatedPos.y * Mathf.Sin (theta)),
					(unrotatedPos.x * Mathf.Sin (theta)) + (unrotatedPos.y * Mathf.Cos (theta)));
			}
		}
		#endregion
	}
}
