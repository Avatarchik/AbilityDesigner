using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseIcon("{SkinIcons}FollowTarget")]
    [PhaseCategory("Transform")]
    public class FollowTarget : Phase
    {
        public SubTarget m_Target;
        public SubFloat m_SpeedPerInstance;
        public SharedFloat m_GeneralSpeed;

        public float m_StopDistance;

        [Header("Ground Detection")]
        public bool m_DetectGround;
        public LayerMask m_GroundLayer;
        public float m_GroundOffset = 1f;

        [Header("Structure")]
        public PhaseListLink m_RunWhile;

        public SharedVector3 m_Direction;
        
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

            Vector3 direction = (targetPos - transform.position).normalized * m_SpeedPerInstance.Value * m_GeneralSpeed.Value * Time.deltaTime;
            if (m_Direction != null)
            {
                m_Direction.Value = direction;
            }
            if (Vector3.Distance(targetPos, transform.position) <= direction.magnitude + m_StopDistance)
            {
                transform.position = targetPos;
                return Result.Success;
            }

            Vector3 newTarget = transform.position + direction;
            Vector3 flatCurrentTarget = newTarget;
            flatCurrentTarget.y = 0f;
            if (m_DetectGround)
            {
                RaycastHit hit;
                if (Physics.Raycast(flatCurrentTarget + new Vector3(0f, transform.position.y + m_GroundOffset, 0f), Vector3.down, out hit, 100f, m_GroundLayer))
                {
                    newTarget.y = hit.point.y + m_GroundOffset;
                }
            }

            transform.position = newTarget;
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