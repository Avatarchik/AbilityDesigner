using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    [CreateAssetMenu(menuName = "Ability Designer/Ability Set", order = 54)]
    public class AbilitySet : ScriptableObject
    {
        public Ability[] m_Abilites;
    }
}