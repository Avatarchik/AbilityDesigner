using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    public class SimpleCooldown : CastRule
    {
        [SerializeField]
        private float m_Cooldown;
        public float cooldown { get { return m_Cooldown; } internal set { m_Cooldown = value; } }

        public override bool IsCastLegitimate(int globalInstances, int userInstances, float userFloat)
        {
            float difference = Time.timeSinceLevelLoad - userFloat;
            if (userFloat <= 0f)
            {
                difference = float.MaxValue;
            }
            if (difference < cooldown)
            {
                return false;
            }

            return true;
        }

        public override void ApplyCast(ref float userFloat)
        {
            userFloat = Time.timeSinceLevelLoad;
        }

        public float GetProgress(float userFloat)
        {
            float difference = Time.timeSinceLevelLoad - userFloat;
            return difference / cooldown;
        }
    }
}