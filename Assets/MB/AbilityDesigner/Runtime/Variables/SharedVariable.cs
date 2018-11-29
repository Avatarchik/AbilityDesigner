using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    public abstract class SharedVariable : ScriptableObject
    {
        [SerializeField]
        public string title { get; set; }
    }

    public abstract class SharedVariable<T> : SharedVariable
    {
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
    }
}