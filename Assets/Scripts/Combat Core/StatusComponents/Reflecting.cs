using Projectiles;
using UnityEngine;

namespace CombatCore.StatusComponents
{
	public class Reflecting : StatusComponent
	{
		private int hitsAbsorbed;

		public override void OnApply(Entity subject)
		{
			subject.GetAbility ("Reflect").active = false;
			subject.SetInvincible (true);
			subject.SetRooted (true);

			subject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		}

		public override void OnDamageTaken(Entity subject, Entity attacker, float rawDamage, float calcDamage, bool damageApplied, bool hitShields)
		{
			hitsAbsorbed++;
		}

		public override void OnRevert(Entity subject)
		{
            for (int i = -hitsAbsorbed / 2; i <= hitsAbsorbed / 2; i++)
            {
                //Projectile.spawnBasic(subject, subject.transform.position, subject.transform.forward, 2f, i * 30f, Speed.VERY_FAST);
                //Projectile.Create(subject, target: subject.transform.forward, maxTime: 2f, angleOffset: i * 30f, speed: Speed.VERY_FAST);
                new Projectile(subject)
                          .Target(subject.transform.forward)
                          .MaxTime(2f)
                          .AngleOffset(i * 30f)
                          .Create();
            }

			subject.SetRooted (false);
			subject.SetInvincible (false);
			subject.GetAbility ("Reflect").active = true;
			subject.AddStatus (Status.Get("Exhausted"));
		}
	}
}
