using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    [System.Serializable]
    public class SharedVector4 : SharedVariable<Vector4>
    {
        public static implicit operator SharedVector4(Vector4 value) { return new SharedVector4 { m_Value = value }; }
    }
}