using System.Collections;
using System.Collections.Generic;
using Projectiles;
using UnityEngine;

namespace CombatCore.StatusComponents
{
    public class ShieldThrown : StatusComponent
    {

        private Vector3 target;

        public void SetTarget(Vector3 target) {
            this.target = target;
        }

        public override void OnApply(Entity subject)
        {
            GameManager.HeldShield.SetActive(false);

            // Try to spawn a shield. If one exists (it shouldn't), fail.
            if (GameManager.ThrownShield == null)
            {
                GameManager.ThrownShield = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Shield"));
                GameManager.ThrownShield.name = "Thrown Shield";
                GameManager.ThrownShield.transform.position = subject.transform.position + Constants.Positions.CENTER;
                //GameManager.ThrownShield.transform.parent = homingProj.transform;
                GameManager.ThrownShield.GetComponent<KeepOnArena>().shouldReset = false;
            }
        }

        public override void OnRevert(Entity subject)
        {
            GameObject.Destroy(GameManager.ThrownShield);
            GameManager.ThrownShield = null;

            GameManager.HeldShield.SetActive(true);
            GameManager.HUD.shieldAvailable = true;
        }
    }
}