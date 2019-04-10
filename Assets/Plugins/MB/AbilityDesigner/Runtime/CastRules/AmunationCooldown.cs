using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
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
            if (offset >= cooldowns[0])
            {
                return true;
            }

            return false;
        }

        public override void ApplyCast(ref float userFloat)
        {
            float offset = Time.timeSinceLevelLoad - userFloat;
            float newOffset = 0f;
            int currentCooldown = GetCooldownID(offset);
            currentCooldown = Mathf.Clamp(currentCooldown - 1, -1, cooldowns.Length - 2);
            for (int c = 0; c <= currentCooldown; c++)
            {
                newOffset += cooldowns[c];
            }
            userFloat = Time.timeSinceLevelLoad - newOffset;
        }

        private int GetCooldownID(float difference)
        {
            float expectedCooldown = 0f;
            for (int c = 0; c < cooldowns.Length; c++)
            {
                expectedCooldown += cooldowns[c];
                if (expectedCooldown > difference)
                {
                    return c - 1;
                }
            }
            return cooldowns.Length - 1;
        }

        public int GetAmunation(float userFloat)
        {
            float offset = Time.timeSinceLevelLoad - userFloat;
            float expectedCooldown = 0f;
            for (int c = 0; c < cooldowns.Length; c++)
            {
                expectedCooldown += cooldowns[c];
                if (expectedCooldown > offset)
                {
                    return c;
                }
            }
            return cooldowns.Length;
        }

        public float GetProgress(float userFloat)
        {
            float offset = Time.timeSinceLevelLoad - userFloat;
            float expectedCooldown = 0f;
            for (int c = 0; c < cooldowns.Length; c++)
            {
                expectedCooldown += cooldowns[c];
                if (expectedCooldown > offset)
                {
                    float seperation = 1 / (float)cooldowns.Length;
                    return ((float)c * seperation) + Mathf.InverseLerp(cooldowns[c], 0f, expectedCooldown - offset) * seperation;
                }
            }
            return 1f;
        }
    }
}