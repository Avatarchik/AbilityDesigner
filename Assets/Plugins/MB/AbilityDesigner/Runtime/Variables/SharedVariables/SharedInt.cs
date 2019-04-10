using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    [System.Serializable]
    public class SharedInt : SharedVariable<int>
    {
        public static implicit operator SharedInt(int value) { return new SharedInt { m_Value = value }; }
    }
}