using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.Collections
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> m_Keys = new List<TKey>();

        [SerializeField]
        private List<TValue> m_Values = new List<TValue>();
        
        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                m_Keys.Add(pair.Key);
                m_Values.Add(pair.Value);
            }
        }
        
        public void OnAfterDeserialize()
        {
            this.Clear();

            if (m_Keys.Count != m_Values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < m_Keys.Count; i++)
                this.Add(m_Keys[i], m_Values[i]);
        }
    }
}