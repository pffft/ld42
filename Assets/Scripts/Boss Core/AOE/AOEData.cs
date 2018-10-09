using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatCore;
using BossCore;

namespace AOEs
{
    public delegate void AOECallbackDelegate(AOE self);

    public class AOEData
    {
        // Mostly so we know what side we're on.
        internal Entity entity;

        // internal. Tracks what triangles are on or off in the mesh
        internal bool[] regions;

        // Origin of the attack
        internal Vector3? preStart; // A value assigned at start. Will be resolved later.
        internal Vector3 start;

        // Where the attack is facing (the 0 line is defined by start-target)
        internal Vector3? preTarget; // A value assigned at start. Will be resolved later.
        internal Vector3 target;

        // internal. How much this attack is rotated from the north line.
        internal float internalRotation;

        // How much this attack is rotated from the center line.
        internal float angleOffset;

        // The scale of the inside ring, from 0-1 relative to the outside ring.
        // This value has no effect if "fixedWidth" is set; it will impact the
        // profile of the attack if "innerExpansionSpeed" is set and different
        // from "expansionSpeed". 
        internal float innerScale;

        // Current scale. This is exactly equal to the world unit radius of the attack.
        internal float scale;

        // How fast the inner ring expands
        internal Speed innerExpansionSpeed;

        // How fast the outer ring expands
        internal Speed expansionSpeed;

        // Does nothing if 0. Else, represents how many units there are between
        // the inner and outer ring at all times.
        internal float fixedWidth;

        // internal. Time since the move started
        internal float currentTime;

        // The maximum lifetime of this attack
        internal float maxTime;

        // How much damage this attack does.
        internal float damage;

        // How fast this guy rotates.
        internal float rotationSpeed;

        // Whether or not this AOE is destroyed when it goes out of bounds.
        internal bool shouldDestroyOnOutOfBounds;

        #region callbacks
        internal AOECallbackDelegate OnDestroyOutOfBoundsImpl;
        public AOEData OnDestroyOutOfBounds(AOECallbackDelegate deleg)
        {
            this.OnDestroyOutOfBoundsImpl = deleg;
            return this;
        }

        internal AOECallbackDelegate OnDestroyTimeoutImpl;
        public AOEData OnDestroyTimeout(AOECallbackDelegate deleg) {
            this.OnDestroyTimeoutImpl = deleg;
            return this;
        }

        #endregion

        public AOEData(Entity self)
        {
            this.entity = self;
            this.regions = new bool[AOE.NUM_SECTIONS];
            for (int i = 0; i < regions.Length; i++)
            {
                regions[i] = false;
            }
            this.preStart = null;
            this.start = Vector3.zero;
            this.preTarget = null;
            this.target = Vector3.zero;
            this.internalRotation = 0f;
            this.angleOffset = 0f;
            this.innerScale = 0.95f;
            this.scale = 1f;
            this.innerExpansionSpeed = BossCore.Speed.MEDIUM;
            this.expansionSpeed = BossCore.Speed.MEDIUM;
            this.fixedWidth = 0f;
            this.currentTime = 0f;
            this.maxTime = 100f;
            this.damage = 5;
            this.rotationSpeed = 0f;

            this.shouldDestroyOnOutOfBounds = true;

            this.OnDestroyOutOfBoundsImpl = AOECallbackDictionary.NOTHING;
            this.OnDestroyTimeoutImpl = AOECallbackDictionary.NOTHING;
        }

        public AOEData On(float from, float to)
        {
            if (to < from)
            {
                return On(to, from);
            }

            if (from < 0 && to > 0)
            {
                return On(from + 360, 360).On(0, to);
            }

            from = from < 0 ? from + 360 : from;
            to = to < 0 ? to + 360 : to;

            for (int i = 0; i < AOE.NUM_SECTIONS; i++)
            {
                float angle = (i + 0.5f) * AOE.THETA_STEP;
                if (angle >= from && angle <= to)
                {
                    regions[i] = true;
                }
            }
            return this;
        }

        public AOEData Off(float from, float to)
        {
            if (to < from)
            {
                return Off(to, from);
            }

            if (from < 0 && to > 0)
            {
                return Off(from + 360, 360).Off(0, to);
            }

            from = from < 0 ? from + 360 : from;
            to = to < 0 ? to + 360 : to;

            for (int i = 0; i < AOE.NUM_SECTIONS; i++)
            {
                float angle = (i + 0.5f) * AOE.THETA_STEP;
                if (angle >= from && angle <= to)
                {
                    regions[i] = false;
                }
            }
            return this;
        }

        public AOEData Start(Vector3? start)
        {
            this.preStart = start;
            return this;
        }

        public AOEData Target(Vector3? target)
        {
            this.preTarget = target;
            return this;
        }

        public AOEData AngleOffset(float degrees)
        {
            this.angleOffset = degrees;
            return this;
        }

        public AOEData MaxTime(float time)
        {
            this.maxTime = time;
            return this;
        }

        public AOEData Speed(Speed speed)
        {
            this.expansionSpeed = speed;
            return this;
        }

        public AOEData Damage(float damage)
        {
            this.damage = damage;
            return this;
        }

        public AOEData FixedWidth(float width)
        {
            //this.innerScale = 0f;
            this.innerExpansionSpeed = BossCore.Speed.FROZEN;
            this.fixedWidth = width;
            return this;
        }

        public AOEData InnerScale(float scale)
        {
            this.innerScale = scale;
            //this.innerExpansionSpeed = 0f;
            this.fixedWidth = 0f;
            return this;
        }

        public AOEData InnerSpeed(Speed speed)
        {
            //this.innerScale = 0f; // initial inner scale gives slightly different effects
            this.innerExpansionSpeed = speed;
            this.fixedWidth = 0f;
            return this;
        }

        public AOEData RotationSpeed(float speed)
        {
            this.rotationSpeed = speed;
            return this;
        }

        // Stops this AOE from moving, until otherwise specified.
        public AOEData Freeze() {
            this.expansionSpeed = BossCore.Speed.FROZEN;
            this.innerExpansionSpeed = BossCore.Speed.FROZEN;
            this.rotationSpeed = 0f;

            return this;
        }

        public AOEData CloneWithCallbacks() {
            AOEData clone = new AOEData(this.entity);

            clone.regions = new bool[AOE.NUM_SECTIONS];
            for (int i = 0; i < regions.Length; i++)
            {
                clone.regions[i] = this.regions[i];
            }
            clone.preStart = preStart;
            clone.start = start;
            clone.preTarget = preTarget;
            clone.target = target;
            clone.internalRotation = internalRotation;
            clone.angleOffset = angleOffset;
            clone.innerScale = innerScale;
            clone.scale = scale;
            clone.innerExpansionSpeed = innerExpansionSpeed;
            clone.expansionSpeed = expansionSpeed;
            clone.fixedWidth = fixedWidth;
            clone.currentTime = 0f;
            clone.maxTime = maxTime;
            clone.damage = damage;
            clone.rotationSpeed = rotationSpeed;

            clone.shouldDestroyOnOutOfBounds = shouldDestroyOnOutOfBounds;

            clone.OnDestroyOutOfBoundsImpl = OnDestroyOutOfBoundsImpl;
            clone.OnDestroyTimeoutImpl = OnDestroyTimeoutImpl;
            return clone;
        }

        public AOEData Clone() {
            AOEData clone = CloneWithCallbacks();
            clone.OnDestroyOutOfBoundsImpl = AOECallbackDictionary.NOTHING;
            clone.OnDestroyTimeoutImpl = AOECallbackDictionary.NOTHING;
            return clone;
        }

        public AOE Create()
        {
            // Set up the gameobject
            GameObject obj = new GameObject();
            obj.transform.position = entity.transform.position;
            obj.layer = LayerMask.NameToLayer("AOE");
            obj.name = "AOE";
            obj.SetActive(false); // hack so we can assign variables on init

            MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();

            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
            if (AOE.AOE_MATERIAL == null)
            {
                AOE.AOE_MATERIAL = new Material(Resources.Load<Material>("Art/Materials/AOE"));
            }
            meshRenderer.material = AOE.AOE_MATERIAL;

            CapsuleCollider collider = obj.AddComponent<CapsuleCollider>();
            collider.center = Vector3.zero;
            collider.radius = 1f;
            collider.isTrigger = true;

            // Add the component with this as its data reference
            // We specifically make a copy, so that we can use this as a template.
            AOE aoe = obj.AddComponent<AOE>();
            aoe.data = CloneWithCallbacks();

            obj.SetActive(true);
            return aoe;
        }
    }
}
