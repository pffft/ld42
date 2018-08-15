using UnityEngine;

namespace CombatCore.StatusComponents
{
	public class Exhausted : StatusComponent
	{
		public override void OnApply(Entity subject)
		{
			for (int i = 0; i < subject.abilityCap; i++)
			{
				Ability a = subject.GetAbility (i);
				if (a != null)
					a.available = false;
			}
		}

		public override void OnUpdate(Entity subject, float time)
		{
			Ability a = subject.GetAbility ("Reflect"), b = subject.GetAbility ("Shield Throw");
			if (a != null && b != null)
			{
				if (a.cooldownCurr <= 0f && b.cooldownCurr <= 0f)
					parent.End ();
			}
			else
				parent.End ();
		}

		public override void OnDamageTaken(Entity subject, Entity attacker, float rawDamage, float calcDamage, bool damageApplied, bool hitShields)
		{
			//TODO staggered on hit
		}

		public override void OnRevert(Entity subject)
		{
			for (int i = 0; i < subject.abilityCap; i++)
			{
				Ability a = subject.GetAbility (i);
				if (a != null)
					a.available = true;
			}
		}
	}
}
