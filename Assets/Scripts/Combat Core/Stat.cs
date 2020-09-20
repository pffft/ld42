using System.Runtime.Serialization;
using System;
using UnityEngine;

namespace CombatCore
{
    [Serializable]
    public struct Stat : ISerializable
    {
        #region INSTANCE_VARS

        // The imutable inital value of this Stat
        [SerializeField]
        private float baseValue;

        // Added onto the base
        private float addValue;

        // A multiplier applied to the base + add
        private float multiValue;

        // A value that can replace the calculated value
        private float lockValue;

        private bool locked;
        /// <summary>
        /// Is this Stat locked to lockValue?
        /// </summary>
        public bool Locked
        {
            get { return locked; }
            set
            {
                locked = value;
                OnStatChanged ();
            }
        }

        // A floor cap for value
        private bool hasMin;
        private float minValue;

        // A ceiling cap for value
        private bool hasMax;
        private float maxValue;

        /// <summary>
        /// The calculated value of this stat, between min and max
        /// </summary>
        public float Value
        {
            get
            {
                if (locked)
                    return lockValue;

                float tVal = calculatedValue;

                if (hasMin && tVal < minValue)
                    return minValue;
                if (hasMax && tVal > maxValue)
                    return maxValue;

                return tVal;
            }
        }

        // (bass + add) * mult
        private float calculatedValue
        {
            get
            {
                float aVal = baseValue + addValue;
                return aVal * multiValue;
            }
        }

        public float Min
        {
            get
            {
                if (hasMin)
                    return minValue;
                return int.MinValue;
            }
            set
            {
                minValue = Min;
                hasMin = true;

                OnStatChanged ();
            }
        }
        public float Max
        {
            get
            {
                if (hasMax)
                    return maxValue;
                return int.MaxValue;
            }
            set
            {
                maxValue = Max;
                hasMax = true;
                OnStatChanged ();
            }
        }
        #endregion

        #region STATIC_METHODS

        public static Stat operator +(Stat s, float value)
        {
            s.addValue += value;
            s.OnStatChanged ();
            return s;
        }

        public static Stat operator -(Stat s, float value)
        {
            s.addValue -= value;
            s.OnStatChanged ();
            return s;
        }

        public static Stat operator *(Stat s, float value)
        {
            s.multiValue *= value;
            s.OnStatChanged ();
            return s;
        }

        public static Stat operator /(Stat s, float value)
        {
            s.multiValue /= value;
            s.OnStatChanged ();
            return s;
        }
        #endregion

        #region INSTANCE_METHODS

        public Stat(float baseValue)
        {
            this.baseValue = baseValue;

            addValue = 0f;

            multiValue = 1f;

            lockValue = 0f;
            locked = false;

            hasMin = false;
            minValue = 0f;

            hasMax = false;
            maxValue = 0f;

            statModified = null;
        }
        public Stat(float baseValue, float minValue) : this (baseValue)
        {
            this.hasMin = true;
            this.minValue = minValue;
        }
        public Stat(float baseValue, float minValue, float maxValue) : this (baseValue)
        {
            this.hasMin = true;
            this.minValue = minValue;

            this.hasMax = true;
            this.maxValue = maxValue;
        }
        public Stat(SerializationInfo info, StreamingContext context) : this (0)
        {
            baseValue = info.GetSingle ("base");
            addValue = info.GetSingle ("add");
            multiValue = info.GetSingle ("multi");
            lockValue = info.GetSingle ("lock");
            locked = info.GetBoolean ("locked");
            hasMin = info.GetBoolean ("hasMin");
            minValue = info.GetSingle ("min");
            hasMax = info.GetBoolean ("hasMax");
            maxValue = info.GetSingle ("max");
        }

        /// <summary>
        /// Modify the baseValue of this Stat
        /// </summary>
        /// <param name="baseValue"></param>
        public void SetBase(float baseValue)
        {
            this.baseValue = baseValue;
            OnStatChanged ();
        }

        /// <summary>
        /// Set the lockValue and indicate it should be used instead of the calculated value
        /// </summary>
        /// <param name="lockValue"></param>
        public void LockTo(float lockValue)
        {
            this.lockValue = lockValue;
            locked = true;

            OnStatChanged ();
        }

        /// <summary>
        /// Indicate the calculated value should be used for now
        /// </summary>
        public void Unlock()
        {
            locked = false;
            OnStatChanged ();
        }

        /// <summary>
        /// Remove the maximum limit on this Stat
        /// </summary>
        public void RemoveMax()
        {
            hasMax = false;
            OnStatChanged ();
        }

        /// <summary>
        /// Remove the maximum limit on this Stat
        /// </summary>
        public void RemoveMin()
        {
            hasMin = false;
            OnStatChanged ();
        }

        /// <summary>
        /// Set the add value to the max
        /// </summary>
        public void Maximize()
        {
            if (hasMax)
                addValue = maxValue;
        }

        /// <summary>
        /// Set the add value to the min
        /// </summary>
        public void Minimize()
        {
            if (hasMin)
                addValue = minValue;
        }

        /// <summary>
        /// Procs whenever some value changes in the Stat that may affect the value output
        /// </summary>
        /// <param name="s"></param>
        public delegate void ChangedStat(Stat s);
        public event ChangedStat statModified;
        private void OnStatChanged()
        {
            if (statModified != null)
                statModified (this);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("base", baseValue);
            info.AddValue ("add", addValue);
            info.AddValue ("multi", multiValue);
            info.AddValue ("lock", lockValue);
            info.AddValue ("locked", locked);
            info.AddValue ("hasMin", hasMin);
            info.AddValue ("min", minValue);
            info.AddValue ("hasMax", hasMax);
            info.AddValue ("max", maxValue);
        }
        #endregion
    }
}