using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    public class MaxInstances : CastRule
    {
        [SerializeField]
        private int m_MaxCountGlobal;
        public int maxCountGlobal { get { return m_MaxCountGlobal; } internal set { m_MaxCountGlobal = value; } }

        [SerializeField]
        private int m_MaxCountUser;
        public int maxCountUser { get { return m_MaxCountUser; } internal set { m_MaxCountUser = value; } }

        public override bool IsCastLegitimate(int globalInstances, int userInstances, float userFloat)
        {
            if (globalInstances >= maxCountGlobal)
            {
                return false;
            }
            
            if (userInstances >= maxCountUser)
            {
                return false;
            }

            return true;
        }

        public override void ApplyCast(ref float userFloat)
        {
        }
    }
}