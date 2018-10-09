using System.Collections;
using System.Collections.Generic;
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
		/// Opens the indicated menu, then closes the currently open menu
		/// </summary>
		/// <param name="menu"></param>
		/// <returns></returns>
		public void NavigateTo(Menu menu)
		{
			menuStack.Push (currentMenu);
			StartCoroutine (DoMenuTransition (currentMenu, menu));
		}

		/// <summary>
		/// Traverses down the previous menu stack one entry, closing the current menu
		/// </summary>
		/// <returns></returns>
		public void NavigateBack()
		{
			if(menuStack.Count > 0)
				StartCoroutine (DoMenuTransition (currentMenu, menuStack.Pop ()));
		}

		private IEnumerator DoMenuTransition(Menu prev, Menu next)
		{
			prev?.Close ();
			while (prev.IsOpen ())
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
