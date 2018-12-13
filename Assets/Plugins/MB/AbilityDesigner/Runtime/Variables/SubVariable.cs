using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MB.Collections;

namespace Matki.AbilityDesigner
{
    [System.Serializable]
    public class SubVariable<T> : SerializableDictionary<SubInstanceLink, T>
    {
        public T Value
        {
            get
            {
                if (!ContainsKey(AbilityContext.subInstanceLink))
                {
                    return System.Activator.CreateInstance<T>();
                }
                return this[AbilityContext.subInstanceLink];
            }
            set
            {
                if (ContainsKey(AbilityContext.subInstanceLink))
                {
                    this[AbilityContext.subInstanceLink] = value;
                }
            }
        }
    }
}