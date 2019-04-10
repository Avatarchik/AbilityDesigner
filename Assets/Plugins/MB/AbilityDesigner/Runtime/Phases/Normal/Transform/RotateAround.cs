using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseCategory("Transform")]
    public class RotateAround : Phase
    {
        public SubTarget m_Target;
        public SubFloat m_SpeedPerInstance;
        public SharedFloat m_GeneralSpeed;

        public float m_Distance = 2f;

        public PhaseListLink m_RunWhile;

        private SubVector3 m_Rotation = new SubVector3();

        public SharedVector3 m_IncomingDirection;
        private Vector3 m_Direction;

        protected override void OnCast()
        {
            m_Direction = Vector3.zero;
            m_Rotation.Value = Quaternion.identity.eulerAngles;
        }

        protected override Result OnUpdate()
        {
            Vector3 targetPos = transform.position;
            switch (m_Target.Value)
            {
                case Target.Target:
                    targetPos = target.GetCenter();
                    break;
                case Target.Originator:
                    targetPos = originator.GetCenter();
                    break;
            }

            if (m_IncomingDirection.Value != m_Direction)
            {
                m_Direction = m_IncomingDirection.Value;
                m_Rotation.Value = Quaternion.LookRotation(-m_Direction).eulerAngles;
            }

            float speed = m_SpeedPerInstance.Value * m_GeneralSpeed.Value;
            m_Rotation.Value = (Quaternion.Euler(0f, speed * 45f * Time.deltaTime, 0f) * Quaternion.Euler(m_Rotation.Value)).eulerAngles;

            targetPos += Quaternion.Euler(m_Rotation.Value) * new Vector3(0f, 0f, m_Distance);
            Vector3 direction = (targetPos - transform.position).normalized * speed * Time.deltaTime;

            transform.position += direction;

            m_RunWhile.RunList();

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