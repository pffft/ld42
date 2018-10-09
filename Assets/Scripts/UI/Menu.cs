﻿using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
	/// <summary>
	/// Represents a single screen in the menu system
	/// </summary>
	[RequireComponent (typeof (CanvasGroup))]
	public abstract class Menu : MonoBehaviour
	{
		#region STATIC_VARS

		private static List<Menu> allMenus;
		#endregion

		#region INSTANCE_VARS

		protected CanvasGroup canvGroup;

		private bool isOpen;
		public bool IsOpen
		{
			get { return isOpen; }
			protected set
			{
				bool changed = isOpen != value;
				isOpen = value;
				if(changed)
					OnFocusChanged (isOpen);
			}
		}

		public event FocusChanged changedFocus;
		#endregion

		#region STATIC_METHODS

		/// <summary>
		/// Get a list of all menus in the scene (active and inactive)
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Menu> GetAllMenus()
		{
			return allMenus;
		}
		#endregion

		#region INSTANCE_METHODS

		public void Start()
		{
			allMenus.Add (this);

			canvGroup = GetComponent<CanvasGroup> ();
		}

		public void OnDestroy()
		{
			allMenus.Remove (this);
		}

		/// <summary>
		/// Begin the transition into the closed state from the open state.
		/// Closing a closed menu does nothing.
		/// </summary>
		public abstract void Close();

		/// <summary>
		/// Force the menu into a closed state ASAP.
		/// </summary>
		public abstract void CloseImmediate();

		/// <summary>
		/// Begin the transition into the open state from the closed state.
		/// Opening an open menu does nothing.
		/// </summary>
		public abstract void Open();

		/// <summary>
		/// Force the menu into an open state ASAP.
		/// </summary>
		public abstract void OpenImmediate();

		public void OnFocusChanged(bool inFocus)
		{
			if (changedFocus != null)
				changedFocus (inFocus);
		}
		#endregion

		#region INTERNAL_TYPES

		public delegate void FocusChanged(bool inFocus);
		#endregion
	}
}
