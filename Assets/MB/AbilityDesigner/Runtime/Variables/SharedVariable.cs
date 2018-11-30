using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    public abstract class SharedVariable : ScriptableObject
    {
        [SerializeField]
        public string title { get; set; }

        internal abstract void CacheDefault();
        internal abstract void RestoreDefault();
    }

    public class SharedVariable<T> : SharedVariable
    {
        private T m_DefaultValue;
        [SerializeField]
        protected T m_Value;
        public T Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        internal override void CacheDefault()
        {
            m_DefaultValue = m_Value;
        }

        internal override void RestoreDefault()
        {
            m_Value = m_DefaultValue;
        }
    }
}