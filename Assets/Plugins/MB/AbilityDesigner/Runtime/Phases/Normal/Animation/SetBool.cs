using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseCategory("Animation")]
    public class SetBool : Phase
    {

        public string m_ParameterName = "Bool";
        public bool m_Value = true;

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

            m_Animator.SetBool(m_ParameterName, m_Value);

            return Result.Success;
        }

        protected override void OnCache()
        {
        }
        protected override void OnReset()
        {
        }
    }
}