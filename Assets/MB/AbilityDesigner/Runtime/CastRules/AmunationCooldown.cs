using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    public class AmunationCooldown : CastRule
    {

        [SerializeField]
        private float[] m_Cooldowns;
        public float[] cooldowns { get { return m_Cooldowns; } internal set { m_Cooldowns = value; } }

        public override bool IsCastLegitimate(int globalInstances, int userInstances, float userFloat)
        {
            float offset = Time.timeSinceLevelLoad - userFloat;
            if (userFloat <= 0f)
            {
                offset = float.MaxValue;
            }
            if (offset > cooldowns[0])
            {
                return false;
            }

            return true;
        }

        public override void ApplyCast(ref float userFloat)
        {
            float offset = Time.timeSinceLevelLoad - userFloat;
            float newOffset;
            int currentCooldown = GetCooldownID(offset, out newOffset);
            userFloat = Time.timeSinceLevelLoad - newOffset;
        }

        private int GetCooldownID(float difference, out float offset)
        {
            float expectedCooldown = 0f;
            for (int c = 0; c < cooldowns.Length; c++)
            {
                expectedCooldown += cooldowns[c];
                if (expectedCooldown > difference)
                {
                    offset = expectedCooldown - cooldowns[c];
                    return c - 1;
                }
            }
            offset = expectedCooldown;
            return cooldowns.Length - 1;
        }
    }
}