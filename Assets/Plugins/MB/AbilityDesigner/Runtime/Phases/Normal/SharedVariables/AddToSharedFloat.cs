using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MB.AbilityDesigner;
using MB.AbilityDesigner.Phases;

namespace MB.Abilities
{
    [PhaseIcon("{SkinIcons}SetValue")]
    [PhaseCategory("Shared Variables")]
    [DefaultSubInstanceLinkOnly]
    public class AddToSharedFloat : Phase
    {
        public SharedFloat m_Value;
        public float m_AddValue;

        protected override Result OnUpdate()
        {
            m_Value.Value += m_AddValue;
            return Result.Success;
        }
    }
}