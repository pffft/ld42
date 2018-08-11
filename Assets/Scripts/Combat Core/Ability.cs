using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
namespace CombatCore
{
	[Serializable]
	public sealed partial class Ability
	{
		#region STATIC_VARS

		private static Dictionary<string, Ability> repository;

		// The latest ID number not assigned to an ability
		private static int latestID;
		#endregion

		#region INSTANCE_VARS

		// The unique ID for this ability type
		private int id;
		public int ID { get { return id; } }

		// The displayed name of this Ability
		[SerializeField]
		public readonly string name;

		// The displayed description of this Ability
		public readonly string desc;

		// The Sprite associated with this Ability
		public readonly Sprite icon;

		// The current cooldown value
		private float _cooldownCurr;
		public float cooldownCurr { get { return _cooldownCurr; } }

		// The maximum possible cooldown value
		public readonly float cooldownMax;

		// The number of use charges this ability has accrued (<= chargesMax)
		private int _charges;
		public int charges { get { return _charges; } }

		// The maximum number of use charges this ability can accrue
		public readonly int chargesMax;

		// Delegate pointing the the method that will run when this ability is used
		private UseEffect effect;

		// Can this Ability be activated?
		public bool available;

		// Is this Ability's cooldown being updated?
		public bool active;
		#endregion

		#region STATIC_METHODS

		// Get an ability from the ability repository, ifex
		public static Ability Get(string name)
		{
			Ability a;
			if (name != null && repository.TryGetValue (name, out a))
				return new Ability (a);
			return null;
		}

		// Add an ability to the ability repository
		private static void Put(Ability a)
		{
			repository.Add (a.name, a.AssignID ());
		}

		#endregion

		#region INSTANCE_METHODS

		private Ability(string name, string desc, Sprite icon, float cooldownMax, int chargesMax, UseEffect effect)
		{
			this.id = -1;
			this.name = name;
			this.desc = desc;
			this.icon = icon;

			this.cooldownMax = cooldownMax;
			_cooldownCurr = cooldownMax;

			this.chargesMax = chargesMax;
			_charges = 0;

			this.effect = effect;

			active = false;
			available = true;
		}
		public Ability(Ability a) : this (a.name, a.desc, a.icon, a.cooldownMax, a.chargesMax, a.effect) { this.id = a.id; }

		/* Instance Methods */

		// Can this Ability be used?
		public bool IsReady()
		{
			return (_cooldownCurr <= 0 || charges > 0) && active && available;
		}

		// Return the percentage of the cooldown that has been completed
		public float CooldownPercentage
		{
			get
			{
				return _cooldownCurr / cooldownMax;
			}
		}

		// Update the cooldown in accordance with the time the last update took
		public void UpdateCooldown(float time)
		{
			if (!active)
				return;

			_cooldownCurr -= time;
			if (_cooldownCurr <= 0f)
			{
				_cooldownCurr = 0f;
				if (_charges < chargesMax)
				{
					_charges++;
					if (_charges != chargesMax)
						_cooldownCurr = cooldownMax;
				}
			}
		}

		// Called to use the Ability
		public bool Use(Entity subject, Vector3 targetPosition, params object[] args)
		{
			if (!IsReady ())
				return false;

			if (effect (subject, targetPosition, args))
			{
				if (_charges > 0)
					_charges--;
				if (_charges < chargesMax || chargesMax == 0)
					_cooldownCurr = cooldownMax;
				return true;
			}
			return false;
		}

		// Equiv checks
		public override bool Equals(object obj)
		{
			Ability other = (Ability)obj;
			if (other.id == -1 || this.id == -1)
				return this.name == other.name;
			return this.id == other.id;
		}
		public override int GetHashCode()
		{
			return id;
		}

		// String representation
		public override string ToString()
		{
			return name + "\n" +
				desc + "\n" +
				"Icon: " + icon.ToString () + "\n" +
				"Cooldown: " + cooldownCurr.ToString ("##0.0") + " / " + cooldownMax.ToString ("##0.0") + "\n" +
				"Charges: " + _charges + " / " + chargesMax + "\n" +
				"Effect: " + effect.Method.ToString ();
		}

		// Setting of ID values
		private Ability AssignID()
		{
			id = latestID++;
			return this;
		}

		#endregion

		#region INTERNAL_TYPES

		// The effect that will occur when this ability is used
		private delegate bool UseEffect(Entity subject, Vector3 targetPosition, params object[] args);
		#endregion
	}
}