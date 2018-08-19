using System.Collections.Generic;
using CombatCore.StatusComponents;

namespace CombatCore
{
	public partial class Status
	{
		static Status()
		{
			repo = new Dictionary<string, Status> ();

			//template put
			Put (new Status (
				"DEBUG",
				"An empty status that does nothing, and lasts forever.",
				null,
				DecayType.serial,
				1,
				float.PositiveInfinity));

			Put (new Status (
				"Exhausted",
				"Cannot use abilities and staggered on hit",
				null,
				DecayType.communal,
				1,
				float.PositiveInfinity,
				new Exhausted ()));
		}
	}
}