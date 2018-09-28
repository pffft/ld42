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
        private bool isAttached;


        public ShieldPlaced SetTarget(Vector3 targetPos) {
            this.target = targetPos;
            return this;
        }

        public override void OnApply(Entity subject)
        {
            subject.movespeed.LockTo(10);
            this.isAttached = true;

            float degrees = Vector3.Angle(Vector3.forward, target - subject.transform.position);
            if (target.x < subject.transform.position.x) {
                degrees = 360 - degrees;
            }

            if (shield == null)
            {
                shield = GameObject.Find("Shield Down");
                _shield = shield.transform.GetChild(0).gameObject;
            }
            shield.transform.parent = GameObject.Find("Player").transform;
            shield.transform.position = subject.transform.position;
            shield.transform.rotation = Quaternion.AngleAxis(degrees, Vector3.up);

            //this.position = subject.transform.position;
            this.rotation = Quaternion.AngleAxis(degrees, Vector3.up);

            Entity shieldEntity = shield.GetComponent<Entity>();
            shieldEntity.shieldsBroken += () => {
                OnShieldsDown(subject);
            };
            shieldEntity.shieldsMax = 50f;
            //shieldEntity.shieldRegen = 25f;
            shieldEntity.shieldDelayMax = 0f;
            _shield.SetActive(true);
        }

        public override void OnShieldsDown(Entity subject)
        {
            subject.RemoveStatus(this.parent);
            shield.GetComponent<Entity>().shieldRegen = 10f;
        }

		public override void OnShieldsRecharged(Entity subject)
        {
            shield.GetComponent<Entity>().shieldRegen = 25f;
		}

		public override void OnUpdate(Entity subject, float time)
        {

            bool wasAttached = isAttached;

            if (subject.HasStatus("ShieldRegen")) {
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
            //subject.AddStatus(Status.Get("Exhausted"));
        }
    }
}