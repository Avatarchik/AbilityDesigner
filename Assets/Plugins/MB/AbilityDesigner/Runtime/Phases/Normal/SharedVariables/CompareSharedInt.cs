using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseIcon("{SkinIcons}Condition")]
    [PhaseCategory("Shared Variables")]
    [DefaultSubInstanceLinkOnly]
    public class CompareSharedInt : Phase
    {
        public SharedInt m_InputValue;

        public enum CompareType { Equal, NotEqual, Larger, LargerEqual, Lower, LowerEqual };
        public CompareType m_CompareType = CompareType.Equal;

        public int m_CompareValue;

        protected override Result OnUpdate()
        {
            if (m_InputValue != null)
            {
                return Result.Fail;
            }
            switch (m_CompareType)
            {
                case CompareType.Equal:
                    return m_InputValue.Value == m_CompareValue ? Result.Success : Result.Fail;
                case CompareType.NotEqual:
                    return m_InputValue.Value != m_CompareValue ? Result.Success : Result.Fail;
                case CompareType.Larger:
                    return m_InputValue.Value > m_CompareValue ? Result.Success : Result.Fail;
                case CompareType.LargerEqual:
                    return m_InputValue.Value >= m_CompareValue ? Result.Success : Result.Fail;
                case CompareType.Lower:
                    return m_InputValue.Value < m_CompareValue ? Result.Success : Result.Fail;
                case CompareType.LowerEqual:
                    return m_InputValue.Value <= m_CompareValue ? Result.Success : Result.Fail;
            }
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