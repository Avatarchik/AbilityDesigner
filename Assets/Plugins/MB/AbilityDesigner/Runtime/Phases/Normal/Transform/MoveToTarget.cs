using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    [PhaseIcon("{SkinIcons}MoveToTarget")]
    [PhaseCategory("Transform")]
    public class MoveToTarget : Phase
    {
        public SubTarget m_Target;
        public SubFloat m_SpeedPerInstance;

        public PhaseListLink m_RunWhile;

        private SubVector3 m_TargetPosition = new SubVector3();

        protected override void OnCast()
        {
            switch (m_Target.Value)
            {
                case Target.Target:
                    m_TargetPosition.Value = target.GetCenter();
                    break;
                case Target.Originator:
                    m_TargetPosition.Value = originator.GetCenter();
                    break;
            }
        }

        protected override Result OnUpdate()
        {
            Vector3 direction = (m_TargetPosition.Value - transform.position).normalized * m_SpeedPerInstance.Value * Time.deltaTime;
            if (Vector3.Distance(m_TargetPosition.Value, transform.position) <= direction.magnitude)
            {
                transform.position = m_TargetPosition.Value;
                return Result.Success;
            }
            transform.position += direction;

            m_RunWhile.RunList();
            return Result.Running;
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