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

        private static List<Menu> allMenus;
        #endregion

        #region INSTANCE_VARS

        public string Name { get { return gameObject.name; } }

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

        static Menu()
        {
            allMenus = new List<Menu> ();
        }

        /// <summary>
        /// Get a list of all menus in the scene (active and inactive)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Menu> GetAllMenus()
        {
            return allMenus;
        }

        /// <summary>
        /// Attempts to find a menu with the given name; if none exists, returns null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Menu GetMenu(string name)
        {
            foreach (Menu m in GetAllMenus ())
            {
                if (m.Name == name)
                    return m;
            }

            return null;
        }
        #endregion

        #region INSTANCE_METHODS

        public void Awake()
        {
            allMenus.Add (this);

            canvGroup = GetComponent<CanvasGroup> ();

            RectTransform rect = GetComponent<RectTransform> ();
            rect.offsetMax = rect.offsetMin = Vector2.zero;
        }

        public void Start()
        {
            transform.localPosition = Vector2.zero;
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
