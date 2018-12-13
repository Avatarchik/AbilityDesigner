using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    public abstract class ConditionPhase : Phase
    {
        [SerializeField]
        private bool m_InvertResult;
        public bool invertResult { get { return m_InvertResult; } internal set { m_InvertResult = value; } }

        // Internal Continous Methods
        protected internal override void    OnInternalCast() { OnCast(); }
        protected internal override void    OnInternalStart() { OnStart(); }
        protected internal override Result  OnInternalUpdate()
        {
            Result result = OnUpdate();

            // Invert Result
            if (invertResult)
                switch (result)
                {
                    case Result.Success:    return Result.Fail;
                    case Result.Fail:       return Result.Success;
                }

            return result;
        }
        protected internal override void    OnInternalReset() { OnReset(); }

        // Exposed Continous Methods
        protected virtual void      OnCast() { }
        protected virtual void      OnStart() { }
        protected virtual Result    OnUpdate() { return Result.Success; }
        protected virtual void      OnReset() { }
    }
}