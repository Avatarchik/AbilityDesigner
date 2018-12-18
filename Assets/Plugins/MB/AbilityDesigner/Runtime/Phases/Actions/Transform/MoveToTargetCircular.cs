using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    [PhaseCategory("Transform")]
    public class MoveToTargetCircular : ActionPhase
    {
        public SubTarget m_Target;
        public SubFloat m_SpeedPerInstance;
        public SubFloat m_Side;

        [Header("Ground Detection")]
        public bool m_DetectGround;
        public LayerMask m_GroundLayer;
        public float m_GroundOffset = 1f;

        [Header("Structure")]
        public PhaseListLink m_RunWhile;

        private SubVector3 m_StartPosition = new SubVector3();
        private SubVector3 m_TargetPosition = new SubVector3();
        private SubVector3 m_CurrentAngle = new SubVector3();

        protected override void OnCast()
        {
            switch (m_Target.Value)
            {
                case Target.Target:
                    m_StartPosition.Value = originator.GetCenter();
                    m_TargetPosition.Value = target.GetCenter();
                    break;
                case Target.Originator:
                    m_StartPosition.Value = target.GetCenter();
                    m_TargetPosition.Value = originator.GetCenter();
                    break;
            }
            m_CurrentAngle.Value = Quaternion.identity.eulerAngles;
        }

        protected override Result OnUpdate()
        {
            Vector3 center = Vector3.Lerp(m_StartPosition.Value, m_TargetPosition.Value, 0.5f);
            float diameter = Vector3.Distance(m_StartPosition.Value, m_TargetPosition.Value);
            float circumference = Mathf.PI * diameter;
            float distance = m_SpeedPerInstance.Value / circumference;
            float angle = Mathf.InverseLerp(0f, circumference, distance) * m_Side.Value * 360f;

            Quaternion targetRotation = Quaternion.Euler(0f, angle * m_SpeedPerInstance.Value * Time.deltaTime, 0f) * Quaternion.Euler(m_CurrentAngle.Value);
            m_CurrentAngle.Value = targetRotation.eulerAngles;
            Vector3 targetPos = center + (targetRotation * (m_StartPosition.Value - center));

            Vector3 flatTarget = m_TargetPosition.Value;
            flatTarget.y = 0f;
            Vector3 flatPosition = transform.position;
            flatPosition.y = 0f;
            if (Vector3.Distance(flatTarget, flatPosition) <= distance)
            {
                transform.position = m_TargetPosition.Value;
                return Result.Success;
            }

            Vector3 flatCurrentTarget = targetPos;
            flatCurrentTarget.y = 0f;
            if (m_DetectGround)
            {
                RaycastHit hit;
                if (Physics.Raycast(flatCurrentTarget + new Vector3(0f, transform.position.y + m_GroundOffset, 0f), Vector3.down, out hit, 100f, m_GroundLayer))
                {
                    targetPos.y = hit.point.y + m_GroundOffset;
                }
            }

            transform.position = targetPos;

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