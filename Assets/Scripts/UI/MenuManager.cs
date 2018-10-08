﻿using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
	public class MenuManager : MonoBehaviour
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
			}
			else
			{
				Debug.LogWarning ("Multiple MenuManagers currently active!");
			}

		}

		/// <summary>
		/// Opens the indicated menu and closes the currently open menu
		/// </summary>
		/// <param name="menu"></param>
		/// <returns></returns>
		public bool NavigateTo(Menu menu)
		{
			currentMenu?.Close ();
			menuStack.Push (currentMenu);
			currentMenu = menu;
			currentMenu.Open ();

			return currentMenu.IsOpen();
		}

		/// <summary>
		/// Traverses down the previous menu stack one entry, closing the current menu
		/// </summary>
		/// <returns></returns>
		public bool NavigateBack()
		{
			if (menuStack.Count > 0)
			{
				currentMenu?.Close ();
				currentMenu = menuStack.Pop ();
				currentMenu.Open();

				return currentMenu.IsOpen ();
			}

			return false;
		}
		#endregion

		#region INTERNAL_TYPES

		#endregion
	}
}
