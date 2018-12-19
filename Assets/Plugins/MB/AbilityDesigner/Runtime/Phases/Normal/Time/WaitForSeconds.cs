using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    [PhaseIcon("{SkinIcons}Time")]
    [PhaseCategory("Time")]
    [DefaultSubInstanceLinkOnly]
    public class WaitForSeconds : Phase
    {

        public SharedFloat m_TimeToWait;

        private float m_WaitedTime;

        protected override void OnCast()
        {
            m_WaitedTime = 0f;
        }

        protected internal override Result OnInternalUpdate()
        {
            m_WaitedTime += Time.deltaTime;
            if (m_WaitedTime >= m_TimeToWait.Value)
            {
                m_WaitedTime = 0f;
                return Result.Success;
            }
            return Result.Running;
        }
    }
}