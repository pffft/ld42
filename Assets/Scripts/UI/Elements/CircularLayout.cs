using UnityEngine;

namespace GameUI
{
	[ExecuteInEditMode]
	public class CircularLayout : MonoBehaviour
	{
		#region INSTANCE_VARS
		public float Radius { get; set; }

		[SerializeField]
		private float targetRadius;
		public float TargetRadius
		{
			get { return targetRadius; }
			set { targetRadius = value; }
		}

		[SerializeField]
		private float offsetRotation = 90f;
		public float OffsetRotation
		{
			get { return offsetRotation; }
			set { offsetRotation = Mathf.Repeat(value, 360f); }
		}
		[SerializeField]
		private float lerpSpeed = 5f;
		public float LerpSpeed
		{
			get { return lerpSpeed; }
			set { lerpSpeed = value; }
		}
		#endregion

		#region INSTANCE_METHODS
		public void Awake()
		{
			Radius = TargetRadius = 0f;
		}

		public void Update()
		{
			ApplyLayout ();
			Radius = Mathf.Lerp (Radius, TargetRadius, Time.unscaledDeltaTime * lerpSpeed);
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
				float theta = (i * (360f / transform.childCount)) + offsetRotation;
				theta *= Mathf.Deg2Rad;
				Transform petal = transform.GetChild (i);

				//set distance from center
				Vector2 unrotatedPos =  new Vector2 (Radius, 0f);

				//rotate that bitch
				petal.localPosition = new Vector2 (
					(unrotatedPos.x * Mathf.Cos (theta)) - (unrotatedPos.y * Mathf.Sin (theta)),
					(unrotatedPos.x * Mathf.Sin (theta)) + (unrotatedPos.y * Mathf.Cos (theta)));
			}
		}
		#endregion
	}
}
