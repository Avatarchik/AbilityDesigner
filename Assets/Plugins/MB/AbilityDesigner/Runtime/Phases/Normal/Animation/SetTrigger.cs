using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseCategory("Animation")]
    public class SetTrigger : Phase
    {

        public string m_TriggerName = "Attack";

        public bool m_WaitForEnd = false;
        public string m_EndState = "EndState";

        private bool m_Casted = false;

        private Animator m_Animator;

        protected override void OnCast()
        {
            m_Animator = originator.gameObject.GetComponentInChildren<Animator>();
        }

        protected override Result OnUpdate()
        {
            if (m_Animator == null || !m_Animator.isActiveAndEnabled || !m_Animator.gameObject.activeInHierarchy)
            {
                return Result.Fail;
            }

            if (!m_Casted)
            {
                m_Animator.SetTrigger(m_TriggerName);
                m_Casted = true;
            }

            if (!m_WaitForEnd)
            {
                m_Casted = false;
                return Result.Success;
            }

            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_EndState))
            {
                m_Casted = false;
                return Result.Success;
            }

            return Result.Running;
        }

        protected override void OnCache()
        {
        }
        protected override void OnReset()
        {
        }
    }
}