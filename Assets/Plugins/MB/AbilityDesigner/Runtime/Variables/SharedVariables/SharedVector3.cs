using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    [System.Serializable]
    public class SharedVector3 : SharedVariable<Vector3>
    {
        public static implicit operator SharedVector3(Vector3 value) { return new SharedVector3 { m_Value = value }; }
    }
}