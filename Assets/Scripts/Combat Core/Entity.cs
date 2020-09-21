using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace CombatCore
{
	public sealed class Entity : MonoBehaviour
	{
		#region STATIC_VARS

		private const float COMBAT_TIMER_MAX = 5f; // 5 seconds
		#endregion

		#region INSTANCE_VARS

		// The faction this Entity belongs to. Entities of the same faction cannot hurt eachother
		[SerializeField]
		private Faction faction = Faction.neutral;

		// A resource pool that is deducted from when taking damage. Death occurs when it reaches 0.
		[SerializeField]
		private float health;
		public float healthMax = 75f;

		// A resource pool that is deducted before health. Can regen after a delay.
		[SerializeField]
		private float shields;
		public float shieldsMax = 25f;
		public float shieldRegen = 1f;
		private float shieldDelay;
		public float shieldDelayMax = 2f;

		// The speed at which this Entity can move through the world
		public Stat movespeed = new Stat (0);

		// If > 0, this Entity is in combat
		private float combatTimer;

		// If > 0, this Entity will not take damage
		private Stat invincible;

		// If > 0, this Entity's Controller cannot take any action
		private Stat stunned;

		// If > 0, this Entity cannot move
		private Stat rooted;

		// Used with Cryo damage type
		private float freezeProgress;

		// The Statuses currently affecting this Entity
		[SerializeField]
		private Dictionary<string, Status> statuses;

		// The Abilities that this Entity
		private Ability[] abilities;
		private int abilSize;

		#endregion

		#region STATIC_METHODS
		public static void DamageEntity(Entity victim, Entity attacker, float damage, bool ignoreShields = false, params Status[] s)
		{
			//if there's no victim, what's the point?
			if (victim == null)
				return;

			//everyone is in combat
			victim.combatTimer = COMBAT_TIMER_MAX;
			if (attacker != null)
				attacker.combatTimer = COMBAT_TIMER_MAX;

			//don't deal negative damage; that's healing
			if (damage < 0)
				damage = 0;

			//the actual damage value that will be subtracted from health/shields
			float calcDamage = damage;
			bool hitShields = false;

			//victim is invincible, do no damage
			if (!victim.IsInvincible ())
			{
				hitShields = victim.shields > 0f && !ignoreShields;
				if (hitShields)
				{
					//deal damage to the shields
					victim.shields -= calcDamage;
					if (victim.shields <= 0f)
					{
						victim.shields = 0f;
						victim.OnShieldsDown ();
						victim.shieldDelay = victim.shieldDelayMax;
					}
				}
				else
				{
					//deal damage to health
					victim.health -= calcDamage;
					if (victim.health <= 0f)
					{
						victim.health = 0f;
						victim.OnDeath ();
					}
				}
			}

			//combat event hooks
			victim.OnDamageTaken (attacker, damage, calcDamage, !victim.IsInvincible (), hitShields);
			if (attacker != null)
				attacker.OnDamageDealt (victim, damage, calcDamage, !victim.IsInvincible (), hitShields);
		}

		public static void HealEntity(Entity e, float healAmount)
		{
			if (healAmount < 0f)
				healAmount = 0f;
			e.health += healAmount;
			if (e.health > e.healthMax)
				e.health = e.healthMax;

			foreach (Status s in e.statuses.Values)
				s.OnHealed (e, healAmount);

			if (e.healed != null)
				e.healed (healAmount);

			//TODO heal effect
		}
		#endregion

		#region INSTANCE_METHODS

		public void Awake()
		{
			//setup non-editor setable values
			health = healthMax;
			shields = shieldsMax;
			shieldDelay = shieldDelayMax;

			combatTimer = 0f;

			invincible = new Stat (0, 0);
			stunned = new Stat (0, 0);
			rooted = new Stat (0, 0);
			freezeProgress = 0f;

			statuses = new Dictionary<string, Status> ();
			abilities = new Ability[3];
			abilSize = 0;
		}

		public float HealthPerc { get { return health / healthMax; } }
		public float ShieldPerc { get { return shields / shieldsMax; } }


		public Faction GetFaction()
		{
			return faction;
		}

		// --- Monobehavior Stuff ---
		public void Update()
		{
			//update all statuses
			Queue<Status> removeQueue = new Queue<Status> ();
			foreach (Status s in statuses.Values)
			{
				if (s.UpdateDuration (this, Time.deltaTime))
					removeQueue.Enqueue (s);
			}

			//remove statuses that have finished their durations
			while (removeQueue.Count > 0)
				RemoveStatus (removeQueue.Dequeue ());

			//update all abilities
			for (int i = 0; i < abilities.Length; i++)
			{
				if (abilities[i] != null)
					abilities[i].UpdateCooldown (Time.deltaTime);
			}

			//update combat timer
			combatTimer -= Time.deltaTime;
			if (combatTimer <= 0f)
				combatTimer = 0f;

			//update freeze progress
			freezeProgress -= Time.deltaTime;
			if (freezeProgress <= 0f)
				freezeProgress = 0f;

			//shield recharge + recharge delay
			shieldDelay -= Time.deltaTime;
			if (shieldDelay <= 0f)
			{
				shieldDelay = 0f;
                if (shields < shieldsMax)
                {
                    shields += (shieldRegen * Time.deltaTime);
                    if (shields >= shieldsMax)
                    {
                        shields = shieldsMax;
                        OnShieldsRecharged();
                    }
                }
			}
		}

		#region STATUS_HANDLING

		// Add a status to the Entity and begin listening for its end
		public void AddStatus(Status s)
		{
			Status existing;
			if (statuses.TryGetValue (s.name, out existing))
			{
				existing.Stack (this, 1);
				return;
			}

			//this status is new to this Entity
			statuses.Add (s.name, s);
			s.OnApply (this);

			//notify listeners
			if (statusAdded != null)
				statusAdded (s);
		}

		// Either a status naturally ran out, or it is being manually removed
        public void RemoveStatus(string name) {
            if (HasStatus(name)) {
                Status s;
                statuses.TryGetValue(name, out s);
                RemoveStatus(s);
            }
        }

		public void RemoveStatus(Status s)
		{
			s.OnRevert (this);
			statuses.Remove (s.name);

			//notify listeners
			if (statusRemoved != null)
				statusRemoved (s);
		}

		/// <summary>
		/// Ends all statuses
		/// </summary>
		public void ClearStatuses()
		{
			foreach (Status s in statuses.Values)
			{
				s.End ();
				s.OnRevert (this);

				if (statusRemoved != null)
					statusRemoved (s);
			}
			statuses.Clear ();
		}

		// Check for a specific status in this Entity's status list
		public bool HasStatus(string name)
		{
			return statuses.ContainsKey (name);
		}

		public bool HasStatus(Status s)
		{
			return HasStatus (s.name);
		}

		// Get an IEnumerable so that the list can be iterated over, but not changed
		// Casting totally isn't a thing, right?
		public IEnumerable GetStatusList()
		{
			return statuses;
		}
		#endregion

		#region ABILITY_HANDLING

		// Add an ability to this Entity
		public bool AddAbility(Ability a, int index = -1)
		{
			//don't allow ability changes if in combat
			//don't allow null insertions
			//don't allow active ability additions
			if (InCombat () || a == null || a.active)
				return false;

			//add the ability and set it to active
			if (index == -1)
			{
				abilities[abilSize++] = a;
				a.active = true;

				if (abilSize >= abilities.Length)
					ResizeAbilities ();
			}
			else if (index >= 0 && index < abilities.Length)
			{
				if (abilities[index] != null && abilityRemoved != null)
					abilityRemoved (abilities[index], index);
				abilities[index] = a;
				a.active = true;
			}
			else
				return false;

			//notify listeners
			if (abilityAdded != null)
				abilityAdded (a, index);

			return true;
		}
		private void ResizeAbilities()
		{
			Ability[] temp = abilities;
			abilities = new Ability[temp.Length * 2];
			for (int i = 0; i < temp.Length; i++)
				abilities[i] = temp[i];
		}

		// Remove an ability from this Entity
		public bool RemoveAbility(Ability a)
		{
			//don't allow ability changes if in combat
			if (InCombat ())
				return false;

			//remove the ability and set it to inactive
			for (int i = 0; i < abilities.Length; i++)
			{
				//look for equal reference
				if (a == abilities[i])
					return RemoveAbility (i);
			}
			return false;
		}
#pragma warning disable 0168
		public bool RemoveAbility(int index)
		{
			if (InCombat ())
				return false;

			Ability removed = null;

			try
			{
				if (abilities[index] == null)
					return false;

				removed = abilities[index];
				abilities[index] = null;
				removed.active = false;

				if (abilityRemoved != null)
					abilityRemoved (removed, index);
			}
			catch (IndexOutOfRangeException ioore)
			{
				Debug.LogError ("Attempted to remove a non-existant Ability."); //DEBUG
				return false;
			}

			return true;
		}

		// Swap the ability at index for a new ability. Returns the the old ability (null if fails)
		public Ability SwapAbility(Ability a, int index)
		{
			if (InCombat () || a == null)
				return null;

			//swap 'em
			Ability old = null;
			try
			{
				old = abilities[index];
				abilities[index] = a;
				if (a != null)
					a.active = true;
				if (old != null)
					old.active = false;
			}
			catch (IndexOutOfRangeException ioore)
			{
				Debug.LogError ("Tried to swap out non-existant Ability"); //DEBUG
				return null;
			}

			if (abilitySwapped != null)
				abilitySwapped (a, old, index);

			return old;
		}
#pragma warning restore 0168

		// Ability getter
		public Ability GetAbility(int index)
		{
			if (index < 0 || index >= abilities.Length)
				return null;
			return abilities[index];
		}

		public Ability GetAbility(string name)
		{
			if (name == null || name == "")
				return null;

			for (int i = 0; i < abilities.Length; i++)
			{
				if (abilities[i] != null && abilities[i].name == name)
					return abilities[i];
			}
			return null;
		}

		// For externally looping through the ability list
		public int abilityCount { get { return abilSize; } }
		public int abilityCap { get { return abilities.Length; } }
		#endregion

		#region BUILTIN_STATUSES

		public bool InCombat()
		{
			return combatTimer > 0f;
		}

		public bool SetInvincible(bool val)
		{
			if (val)
				invincible += 1;
			else
				invincible -= 1;
			return IsInvincible ();
		}
		public bool IsInvincible()
		{
			return invincible.Value > 0;
		}

		public bool SetStunned(bool val)
		{
			if (val)
			{
				stunned += 1;

				foreach (Status s in statuses.Values)
					s.OnStunned (this);

				if (wasStunned != null)
					wasStunned ();
			}
			else
				stunned -= 1;
			return IsStunned ();
		}
		public bool IsStunned()
		{
			return stunned.Value > 0;
		}

		public bool SetRooted(bool val)
		{
			if (val)
			{
				rooted += 1;

				foreach (Status s in statuses.Values)
					s.OnRooted (this);

				if (wasRooted != null)
					wasRooted ();
			}
			else
				rooted -= 1;
			return IsRooted ();
		}
		public bool IsRooted()
		{
			return rooted.Value > 0;
		}
		#endregion

		#region HOOKS

		// This Entity took damage
		private void OnDamageTaken(Entity attacker, float rawDamage, float calcDamage, bool damageApplied, bool hitShields)
		{
			foreach (Status s in statuses.Values)
				s.OnDamageTaken (this, attacker, rawDamage, calcDamage, damageApplied, hitShields);

			if (tookDamage != null)
				tookDamage (this, attacker, rawDamage, calcDamage, damageApplied, hitShields);
		}

		// This Entity dealt damage
		private void OnDamageDealt(Entity victim, float rawDamage, float calcDamage, bool damageApplied, bool hitShields)
		{
			foreach (Status s in statuses.Values)
				s.OnDamageDealt (this, victim, rawDamage, calcDamage, damageApplied, hitShields);

			if (dealtDamage != null)
				dealtDamage (victim, this, rawDamage, calcDamage, damageApplied, hitShields);
		}

		// This Entity has died
		public void OnDeath()
		{
			foreach (Status s in statuses.Values)
				s.OnDeath (this);

			if (died != null)
				died ();
		}

		// Shields have fallen to zero
		private void OnShieldsDown()
		{
			foreach (Status s in statuses.Values)
				s.OnShieldsDown (this);

			if (shieldsBroken != null)
				shieldsBroken ();
		}

		// Shields have recharged to full
		private void OnShieldsRecharged()
		{
			foreach (Status s in statuses.Values)
				s.OnShieldsRecharged (this);

			if (shieldsRecharged != null)
				shieldsRecharged ();
		}
		#endregion
		#endregion

		#region INTERNAL_TYPES

		public enum Faction { neutral, player, enemy }

		/* Delegates and Events */
		public delegate void StatusChanged(Status s);
		public event StatusChanged statusAdded;
		public event StatusChanged statusRemoved;

		public delegate void AbilityChanged(Ability a, int index = -1);
		public event AbilityChanged abilityAdded;
		public event AbilityChanged abilityRemoved;
		public delegate void AbilitySwap(Ability a, Ability old, int index);
		public event AbilitySwap abilitySwapped;

		public delegate void EntityAttacked(Entity victim, Entity attacker, float rawDamage, float calcDamage, bool damageApplied, bool hitShields);
		public event EntityAttacked tookDamage;
		public event EntityAttacked dealtDamage;

		public delegate void EntityGeneric();
		public event EntityGeneric died;
		public event EntityGeneric shieldsBroken;
		public event EntityGeneric shieldsRecharged;
		public event EntityGeneric wasStunned;
		public event EntityGeneric wasRooted;

		public delegate void EntityHealed(float healAmount);
		public event EntityHealed healed;
		#endregion
	}
}