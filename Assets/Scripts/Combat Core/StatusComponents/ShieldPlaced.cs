using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatCore.StatusComponents
{
    public class ShieldPlaced : StatusComponent
    {
        private Vector3 position;
        private Quaternion rotation;

        private Vector3 target;
        private GameObject shield;
        private GameObject _shield;
        private Entity shieldEntity;
        private bool isAttached;

        private static float oldShieldStatus;

        public ShieldPlaced SetTarget(Vector3 targetPos) {
            this.target = targetPos;
            return this;
        }

        public override void OnApply(Entity subject)
        {
            GameManager.HeldShield.SetActive(false);
            subject.movespeed.LockTo(10);
            this.isAttached = true;

            float degrees = Vector3.Angle(Vector3.forward, target - subject.transform.position);
            if (target.x < subject.transform.position.x) {
                degrees = 360 - degrees;
            }

            if (shield == null)
            {
                shield = GameManager.PlacedShield;
                shieldEntity = shield.GetComponent<Entity>();

                shieldEntity.tookDamage += (Entity victim, Entity attacker, float rawDamage, float calcDamage, bool damageApplied, bool hitShields) => {
                    UpdateShieldAppearence(victim);
                };
                shieldEntity.shieldsBroken += () => {
                    _shield.SetActive(false);
                    subject.RemoveStatus(this.parent);
                    shield.GetComponent<Entity>().shieldRegen = 10f;
                };
                shieldEntity.shieldsRecharged += () =>
                {
                    Debug.Log("Shields recharged!");
                    shield.GetComponent<Entity>().shieldRegen = 25f;
                };
            }
            shield.transform.parent = GameManager.Player.transform;
            shield.transform.position = subject.transform.position;
            shield.transform.rotation = Quaternion.AngleAxis(degrees, Vector3.up);

            //this.position = subject.transform.position;
            this.rotation = Quaternion.AngleAxis(degrees, Vector3.up);


            int shieldStatus = Mathf.Min(4, (int)(shieldEntity.ShieldPerc * 5));
            oldShieldStatus = shieldStatus;
            _shield = shield.transform.GetChild(shieldStatus).gameObject;

            shieldEntity.shieldsMax = 50f;
            //shieldEntity.shieldRegen = 25f;
            shieldEntity.shieldDelayMax = 0f;
            _shield.SetActive(true);
        }

        private void UpdateShieldAppearence(Entity victim)
        {
            int localShieldStatus = Mathf.Min(4, (int)(victim.ShieldPerc * 5));
            if (Mathf.Abs(oldShieldStatus - localShieldStatus) < 0.01f)
            {
                return;
            }
            else
            {
                _shield.SetActive(false);
                _shield = shield.transform.GetChild(localShieldStatus).gameObject;
                _shield.SetActive(true);
                oldShieldStatus = localShieldStatus;
            }
        }

		public override void OnUpdate(Entity subject, float time)
        {

            bool wasAttached = isAttached;

            if (subject.HasStatus("ShieldRegen")) {
                UpdateShieldAppearence(shieldEntity);
            } else {
                isAttached = false;
            }
            shield.transform.rotation = rotation;

            if (wasAttached && !isAttached) {
                subject.movespeed.Unlock();
                //position = subject.transform.position;
                shield.transform.parent = null;
                shield.transform.position = subject.transform.position;
                shield.GetComponent<Entity>().shieldRegen = 0f;
            }
        }

        public override void OnRevert(Entity subject)
        {
            _shield.SetActive(false);
            subject.movespeed.Unlock();
            GameManager.HeldShield.SetActive(true);
            //subject.AddStatus(Status.Get("Exhausted"));
        }
    }
}