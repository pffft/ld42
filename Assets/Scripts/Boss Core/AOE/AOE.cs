using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatCore;
using BossCore;
using System.Runtime.CompilerServices;

namespace AOEs
{
    public delegate void AOECallbackDelegate(AOEComponent self);

    public class AOE
    {

        // This is a hack. It works, but the compiler will give a warning about hiding
        // the outer method. There's probably a better way, but this gives nice syntax.
//        public class RegionObj {
//#pragma warning disable RECS0146 // Member hides static member from outer class
//            public RegionObj On(float from, float to)
//            {
//#pragma warning restore RECS0146 // Member hides static member from outer class
        //        // Do the actual math here
        //        return new RegionObj();
        //    }
        //}

        //public static RegionObj On(float from, float to) {
        //    // Do the actual math here
        //    return new RegionObj();
        //}

        // How many sections are in the AOE attack mesh
        public const int NUM_SECTIONS = 360 / 5;

        // The number of degrees subtended by an AOE region.
        public const float THETA_STEP = 360f / NUM_SECTIONS;

        // The height at which we render the AOE, so it doesn't clip the ground.
        public const float HEIGHT = 0.5f;

        // Every AOE has the same material, for now. We cache it here.
        public static Material AOE_MATERIAL;

        // internal. Tracks what triangles are on or off in the mesh
        public bool[] Regions { get; protected set; } = new bool[NUM_SECTIONS];

        // Origin of the attack
        public ProxyVector3 Start { get; set; } = AI.AISequence.BOSS_POSITION;

        // Where the attack is facing (the 0 line is defined by start-target)
        public ProxyVector3 Target { get; set; } = AI.AISequence.DELAYED_PLAYER_POSITION;

        // internal. How much this attack is rotated from the north line.
        public float InternalRotation { get; set; } = 0f;

        // How much this attack is rotated from the center line.
        public float AngleOffset { get; set; } = 0f;

        // The scale of the inside ring, from 0-1 relative to the outside ring.
        // This value has no effect if "fixedWidth" is set; it will impact the
        // profile of the attack if "innerExpansionSpeed" is set and different
        // from "expansionSpeed". 
        public float InnerScale { get; set; } = 0.95f;

        // Current scale. This is exactly equal to the world unit radius of the attack.
        public float Scale { get; set; } = 1f;

        // How fast the inner ring expands
        private Speed _innerSpeed = Speed.MEDIUM;
        public Speed InnerSpeed
        {
            get
            {
                return _innerSpeed;
            }

            set
            {
                if (Mathf.Abs(FixedWidth) < 0.01f)
                {
                    _innerSpeed = value;
                    _outerSpeed = value;
                }
            }
        }

        // How fast the outer ring expands
        private Speed _outerSpeed = Speed.MEDIUM;
        public Speed OuterSpeed { 
            get {
                return _outerSpeed;
            }

            set {
                if (Mathf.Abs(FixedWidth) < 0.01f) {
                    _innerSpeed = value;
                    _outerSpeed = value;
                }
            }
        }

        // Does nothing if 0. Else, represents how many units there are between
        // the inner and outer ring at all times.
        // Setting fixed width and then speed will change both at once.
        // TODO fix logic to what it used to be.
        public float FixedWidth { get; set; } = 0f;

        // internal. Time since the move started
        public float CurrentTime { get; set; } = 0f;

        // The maximum lifetime of this attack
        public float MaxTime { get; set; } = 100f;

        // How much damage this attack does.
        public float Damage { get; set; } = 5f;

        // How fast this guy rotates.
        public float RotationSpeed { get; set; } = 0f;

        // Whether or not this AOE is destroyed when it goes out of bounds.
        public bool shouldDestroyOnOutOfBounds;

        #region callbacks

        public void ShouldDestroyOnOutOfBounds(bool to) {
            this.shouldDestroyOnOutOfBounds = to;
        }

        public AOECallbackDelegate OnDestroyOutOfBounds { get; set; } = AOECallbackDictionary.NOTHING;

        public AOECallbackDelegate OnDestroyTimeout { get; set; } = AOECallbackDictionary.NOTHING;

        #endregion

        public AOE()
        {
            for (int i = 0; i < Regions.Length; i++)
            {
                Regions[i] = false;
            }
            this.shouldDestroyOnOutOfBounds = true;
        }

        public AOE On(float from, float to)
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

            for (int i = 0; i < NUM_SECTIONS; i++)
            {
                float angle = (i + 0.5f) * THETA_STEP;
                if (angle >= from && angle <= to)
                {
                    Regions[i] = true;
                }
            }
            return this;
        }

        public AOE Off(float from, float to)
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

            for (int i = 0; i < NUM_SECTIONS; i++)
            {
                float angle = (i + 0.5f) * THETA_STEP;
                if (angle >= from && angle <= to)
                {
                    Regions[i] = false;
                }
            }
            return this;
        }

        // Stops this AOE from moving, until otherwise specified.
        public AOE Freeze()
        {
            this.InnerSpeed = Speed.FROZEN;
            this.OuterSpeed = Speed.FROZEN;
            this.RotationSpeed = 0f;

            return this;
        }

        public AOE Clone()
        {
            return MemberwiseClone() as AOE;
        }

        public AOEComponent Create()
        {
            // Set up the gameobject
            GameObject obj = new GameObject();
            obj.transform.position = Start.GetValue(); // TODO move this into initialization of AOEComponent
            obj.layer = LayerMask.NameToLayer("AOE");
            obj.name = "AOE";
            obj.SetActive(false); // hack so we can assign variables on init

            MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
            meshFilter.mesh = new Mesh();

            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
            if (AOE_MATERIAL == null)
            {
                AOE_MATERIAL = new Material(Resources.Load<Material>("Art/Materials/AOE"));
            }
            meshRenderer.material = AOE_MATERIAL;

            CapsuleCollider collider = obj.AddComponent<CapsuleCollider>();
            collider.center = Vector3.zero;
            collider.radius = 1f;
            collider.isTrigger = true;

            // Add the component with this as its data reference
            // We specifically make a copy, so that we can use this as a template.
            AOEComponent aoe = obj.AddComponent<AOEComponent>();
            aoe.data = Clone();

            obj.SetActive(true);
            return aoe;
        }

        public override string ToString()
        {
            return "AOE";
        }
    }
}
