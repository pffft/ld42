

namespace CombatCore.StatusComponents
{
	public class Reflecting : StatusComponent
	{
		public override void OnApply(Entity subject)
		{
			subject.shieldsMax = 1f;
			subject.shieldRegen = 1f;
			subject.shieldDelayMax = 0f;
		}

		public override void OnDamageTaken(Entity subject, Entity attacker, float rawDamage, float calcDamage, bool hitShields)
		{
			if (hitShields)
			{
				Projectile.Create (subject.transform.position, subject.transform.rotation, (float)ProjectileManager.Speed.VERY_FAST, 2f);
			}
		}

		public override void OnRevert(Entity subject)
		{
			subject.shieldsMax = 0f;
			subject.shieldRegen = 0f;
			subject.shieldDelayMax = 0f;
		}
	}
}
