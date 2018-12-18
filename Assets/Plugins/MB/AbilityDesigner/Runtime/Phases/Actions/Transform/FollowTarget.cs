using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    [PhaseCategory("Transform")]
    public class FollowTarget : ActionPhase
    {
        public SubTarget m_Target;
        public SubFloat m_SpeedPerInstance;
        
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

            Vector3 direction = (targetPos - transform.position).normalized * m_SpeedPerInstance.Value * Time.deltaTime;
            if (Vector3.Distance(targetPos, transform.position) <= direction.magnitude)
            {
                transform.position = targetPos;
                return Result.Success;
            }
            transform.position += direction;
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