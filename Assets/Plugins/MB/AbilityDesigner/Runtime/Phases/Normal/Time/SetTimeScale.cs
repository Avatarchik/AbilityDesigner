using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    [PhaseIcon("{SkinIcons}Time")]
    [PhaseCategory("Time")]
    [DefaultSubInstanceLinkOnly]
    public class SetTimeScale : Phase
    {

        public SharedFloat m_TimeScale;

        protected internal override Result OnInternalUpdate()
        {
            Time.timeScale = m_TimeScale.Value;
            return Result.Success;
        }
    }
}