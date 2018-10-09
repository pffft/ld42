using System.Collections.Generic;
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

		private static List<Menu> activeMenus;
		#endregion

		#region INSTANCE_VARS

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

		public static IEnumerable<Menu> GetAllMenus()
		{
			return activeMenus;
		}
		#endregion

		#region INSTANCE_METHODS

		public void Start()
		{
			activeMenus.Add (this);
		}

		public void OnDestroy()
		{
			activeMenus.Remove (this);
		}

		public abstract void Open();

		public abstract void Close();

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
