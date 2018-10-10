using System.Collections;
using UnityEngine;

namespace GameUI
{
	public class CircularMenu : Menu
	{
		#region INSTANCE_VARS

		[SerializeField]
		private CircularLayout layout;

		[SerializeField]
		private float holdingRadius;
		#endregion

		#region INSTANCE_METHODS
		public new void Awake()
		{
			base.Awake ();
			CloseImmediate ();
		}

		public override void Close()
		{
			if (IsOpen)
				StartCoroutine(Transition (0f, false));
		}

		public override void CloseImmediate()
		{
			layout.TargetRadius = layout.Radius = 0f;
			canvGroup.interactable = false;
			canvGroup.alpha = 0f;
			IsOpen = false;
		}

		public override void Open()
		{
			if(!IsOpen)
				StartCoroutine (Transition (holdingRadius, true));
		}

		public override void OpenImmediate()
		{
			layout.TargetRadius = layout.Radius = holdingRadius;
			canvGroup.interactable = true;
			canvGroup.alpha = 1f;
			IsOpen = true;
		}

		private IEnumerator Transition(float targetRadius, bool opening)
		{
			float totalDist = Mathf.Abs (layout.Radius - targetRadius);
			float currDist;

			if(!opening)
				canvGroup.interactable = opening;

			//wait for the layout to do its thing
			layout.TargetRadius = targetRadius;
			while ((currDist = Mathf.Abs (layout.TargetRadius - layout.Radius)) > 0.1f)
			{
				canvGroup.alpha = opening ? 1 - (currDist / totalDist) : currDist / totalDist;
				yield return null;
			}

			if (opening)
				canvGroup.interactable = opening;

			IsOpen = opening;
		}
		#endregion
	}
}
