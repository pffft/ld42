using System.Collections.Generic;
using UnityEngine;

namespace CombatCore
{
	public partial class Ability
	{
		static Ability()
		{
			latestID = 0;

			repository = new Dictionary<string, Ability> ();

			//player offensive
			Put (new Ability (
				"DEBUG",
				"Does nothing",
				null,
				0.1f,
				0,
				"DebugAbil")
			);
		}

		private bool DebugAbil(Entity subject, Vector2 targetPosition, params object[] args)
		{
			Debug.Log ("BANG");
			return true;
		}
	}
}