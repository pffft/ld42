using System;
using UnityEngine;

namespace GameUI
{
    [RequireComponent (typeof (Animator))]
    public class AnimatedMenu : Menu
    {
        #region STATIC_VARS

        private const string IS_OPEN = "isOpen";
        private const string OPEN_STATE_NAME = "Open";
        private const string CLOSED_STATE_NAME = "Closed";
        #endregion

        #region INSTANCE_VARS

        private Animator animator;
        #endregion

        #region STATIC_METHODS

        #endregion

        #region INSTANCE_METHODS

        public new void Start()
        {
            base.Start ();
            animator = GetComponent<Animator> ();
        }

        public override void Close()
        {
            animator.SetBool (IS_OPEN, false);
            IsOpen = false;
        }

        public override void CloseImmediate()
        {
            animator.Play (OPEN_STATE_NAME);
            IsOpen = false;
        }

        public override void Open()
        {
            animator.SetBool (IS_OPEN, true);
            IsOpen = true;
        }

        public override void OpenImmediate()
        {
            animator.Play (CLOSED_STATE_NAME);
            IsOpen = true;
        }
        #endregion

        #region INTERNAL_TYPES

        #endregion
    }
}
