using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
	/// <summary>
	/// Manages transitioning between active menus
	/// </summary>
	public sealed class MenuManager : MonoBehaviour
	{
		#region STATIC_VARS

		private static MenuManager instance;
		#endregion

		#region INSTANCE_VARS

		[SerializeField]
		private Menu currentMenu;

		private Stack<Menu> menuStack;
		#endregion

		#region STATIC_METHODS

		public MenuManager GetInstance()
		{
			if (instance == null)
			{
				Debug.LogWarning ("No MenuManager currently active!");
			}

			return instance;
		}
		#endregion

		#region INSTANCE_METHODS

		public void Start()
		{
			if (instance == null)
			{
				instance = this;
				menuStack = new Stack<Menu> ();
				currentMenu.OpenImmediate ();
			}
			else
			{
				Debug.LogWarning ("Multiple MenuManagers currently active!");
			}
		}

		/// <summary>
		/// Attempts to find a menu with the given name; if none exists, returns null
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Menu GetMenu(string name)
		{
			foreach (Menu m in Menu.GetAllMenus ())
			{
				if (m.gameObject.name == name)
					return m;
			}

			return null;
		}

		/// <summary>
		/// Closes the currently open menu, then opens the inidcated menu.
		/// </summary>
		/// <param name="menu"></param>
		/// <returns></returns>
		public void NavigateTo(Menu menu)
		{
			menuStack.Push (currentMenu);
			StartCoroutine (DoMenuTransition (currentMenu, menu));
		}

		/// <summary>
		/// Traverses down the previous menu stack one entry, closing the current menu.
		/// </summary>
		/// <returns>True if a back operation is possible, false otherwise</returns>
		public bool NavigateBack()
		{
			if (menuStack.Count > 0)
			{
				StartCoroutine (DoMenuTransition (currentMenu, menuStack.Pop ()));
				return true;
			}
			return false;
		}

		private IEnumerator DoMenuTransition(Menu prev, Menu next)
		{
			prev?.Close ();
			while (prev.IsOpen)
			{
				yield return null;
			}
			next.Open ();
			currentMenu = next;
		}
		#endregion

		#region INTERNAL_TYPES

		#endregion
	}
}
