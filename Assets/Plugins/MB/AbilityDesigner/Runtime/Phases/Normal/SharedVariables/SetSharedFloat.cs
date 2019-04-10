using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseIcon("{SkinIcons}SetValue")]
    [PhaseCategory("Shared Variables")]
    [DefaultSubInstanceLinkOnly]
    public class SetSharedFloat : Phase
    {
        public SharedFloat m_InputValue;
        public float m_SetValue;

        protected override Result OnUpdate()
        {
            if (m_InputValue != null)
            {
                m_InputValue.Value = m_SetValue;
            }
            return Result.Success;
        }

        // Does not need any cache or reset
        protected override void OnCache()
        {
        }

        protected override void OnReset()
        {
        }
    }
}