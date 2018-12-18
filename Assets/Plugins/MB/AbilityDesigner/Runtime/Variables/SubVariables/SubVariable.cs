using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MB.Collections;

namespace Matki.AbilityDesigner
{
    [System.Serializable]
    public class SubVariable<T> : SerializableDictionary<int, T>
    {
        public T Value
        {
            get
            {
                if (!ContainsKey(AbilityContext.subInstanceLink.id))
                {
                    return System.Activator.CreateInstance<T>();
                }
                return this[AbilityContext.subInstanceLink.id];
            }
            set
            {
                if (ContainsKey(AbilityContext.subInstanceLink.id))
                {
                    this[AbilityContext.subInstanceLink.id] = value;
                }
                else
                {
                    Add(AbilityContext.subInstanceLink.id, value);
                }
            }
        }

        internal void Prepare(SubInstanceLink[] links)
        {
            List<int> incomingKeys = new List<int>();
            for (int l = 0; l < links.Length; l++)
            {
                if (!ContainsKey(links[l].id))
                {
                    Add(links[l].id, System.Activator.CreateInstance<T>());
                }
                incomingKeys.Add(links[l].id);
            }

            List<int> localKeys = new List<int>(Keys);
            for (int lk = 0; lk < localKeys.Count; lk++)
            {
                if (!incomingKeys.Contains(localKeys[lk]))
                {
                    Remove(localKeys[lk]);
                }
            }
        }

        internal object GetValue(SubInstanceLink link)
        {
            return this[link.id];
        }

        internal void SetValue(SubInstanceLink link, object value)
        {
            this[link.id] = (T)value;
        }

        internal SubVariable<T> Clone()
        {
            SubVariable<T> clone = new SubVariable<T>();
            
            foreach (KeyValuePair<int, T> pair in this)
            {
                clone.Add(pair.Key, pair.Value);
            }
            return clone;
        }

        internal void Apply(SubVariable<T> blueprint)
        {
            foreach (KeyValuePair<int, T> pair in blueprint)
            {
                Add(pair.Key, pair.Value);
            }
        }
    }
}