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

		private bool transitioning;

		public int HistoryDepth
		{
			get { return menuStack.Count; }
		}
		#endregion

		#region STATIC_METHODS

		public static MenuManager GetInstance()
		{
			if (instance == null)
			{
				Debug.LogWarning ("No MenuManager currently active!");
			}

			return instance;
		}
		#endregion

		#region INSTANCE_METHODS

		public void Awake()
		{
			if (instance == null)
			{
				instance = this;
				menuStack = new Stack<Menu> ();
				currentMenu?.OpenImmediate ();
				transitioning = false;
			}
			else
			{
				Debug.LogWarning ("Multiple MenuManagers currently active!");
			}
		}

		/// <summary>
		/// Closes the currently open menu, then opens the inidcated menu.
		/// </summary>
		/// <param name="menu"></param>
		/// <returns></returns>
		public void NavigateTo(Menu menu)
		{
			if (!transitioning)
			{
				if(currentMenu != null)
					menuStack.Push (currentMenu);
				StartCoroutine (DoMenuTransition (currentMenu, menu));
			}
		}

		/// <summary>
		/// Traverses down the previous menu stack one entry, closing the current menu.
		/// </summary>
		/// <returns></returns>
		public void NavigateBack()
		{
			if (!transitioning && menuStack.Count > 0)
			{
				StartCoroutine (DoMenuTransition (currentMenu, menuStack.Pop ()));
			}
		}

		private IEnumerator DoMenuTransition(Menu prev, Menu next)
		{
			Debug.Log ("Beginning transition from " + prev?.Name + " to " + next?.Name); //DEBUG
			transitioning = true;

			prev?.Close ();
			while (prev != null && prev.IsOpen)
			{
				yield return null;
			}

			next?.Open ();
			currentMenu = next;
			while (next != null && !next.IsOpen)
			{
				yield return null;
			}

			transitioning = false;
			Debug.Log ("Finshed transition from " + prev?.Name + " to " + next?.Name); //DEBUG
		}
		#endregion

		#region INTERNAL_TYPES

		#endregion
	}
}
