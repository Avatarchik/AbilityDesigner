using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    [PhaseIcon("{SkinIcons}SetValue")]
    [PhaseCategory("Shared Variables")]
    [DefaultSubInstanceLinkOnly]
    public class SetSharedVector3 : Phase
    {
        public SharedVector3 m_InputValue;
        public Vector3 m_SetValue;

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