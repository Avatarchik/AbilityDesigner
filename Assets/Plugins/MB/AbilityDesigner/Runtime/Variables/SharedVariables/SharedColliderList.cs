using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    [System.Serializable]
    public class SharedColliderList : SharedVariable<List<Collider>>
    {
        public static implicit operator SharedColliderList(List<Collider> value) { return new SharedColliderList { m_Value = value }; }
    }
}