using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseIcon("{SkinIcons}Phase")]
    public abstract class Phase : PhaseCore
    {
        // Internal Continous Methods
        protected internal override void    OnInternalCast() { OnCast(); }
        protected internal override void    OnInternalStart() { OnStart(); }
        protected internal override Result  OnInternalUpdate() { return OnUpdate(); }

        // Exposed Continous Methods
        protected virtual void      OnCast() { }
        protected virtual void      OnStart() { }
        protected virtual Result    OnUpdate() { return Result.Success; }
    }
}