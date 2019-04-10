using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    [System.Serializable]
    public class SharedVector2 : SharedVariable<Vector2>
    {
        public static implicit operator SharedVector2(Vector2 value) { return new SharedVector2 { m_Value = value }; }
    }
}