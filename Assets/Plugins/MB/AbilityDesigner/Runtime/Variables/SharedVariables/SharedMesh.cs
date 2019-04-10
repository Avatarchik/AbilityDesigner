using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    [System.Serializable]
    public class SharedMesh : SharedVariable<Mesh>
    {
        public static implicit operator SharedMesh(Mesh value) { return new SharedMesh { m_Value = value }; }
    }
}