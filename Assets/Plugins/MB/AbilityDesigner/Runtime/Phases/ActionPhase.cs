using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    public abstract class ActionPhase : Phase
    {
        // Internal Continous Methods
        protected internal override void    OnInternalCast() { OnCast(); }
        protected internal override void    OnInternalStart() { OnStart(); }
        protected internal override Result  OnInternalUpdate() { return OnUpdate(); }
        protected internal override void    OnInternalReset() { OnReset(); }

        // Exposed Continous Methods
        protected virtual void      OnCast() { }
        protected virtual void      OnStart() { }
        protected virtual Result    OnUpdate() { return Result.Success; }
        protected virtual void      OnReset() { }
    }
}