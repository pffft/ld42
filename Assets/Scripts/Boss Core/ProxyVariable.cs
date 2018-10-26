using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BossCore
{
    /// <summary>
    /// For referencing values that cannot be directly saved
    /// in an object reference
    /// </summary>
    public class ProxyVariable<T>
    {
        protected ProxiedValueGet get;
        protected ProxiedValueSet set;

        public ProxyVariable(ProxiedValueGet get, ProxiedValueSet set) : this(get)
        {
            this.set = set;
        }
        public ProxyVariable(ProxiedValueGet get)
        {
            this.get = get;
        }

        public virtual T GetValue()
        {
            return (T)get();
        }

        public virtual void SetValue(T val)
        {
            set?.Invoke(val);
        }

        public delegate object ProxiedValueGet();
        public delegate void ProxiedValueSet(T value);
    }
}
