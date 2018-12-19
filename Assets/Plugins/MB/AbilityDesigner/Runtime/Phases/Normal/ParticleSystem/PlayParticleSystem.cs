using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    [PhaseCategory("Particle")]
    public class PlayParticleSystem : Phase
    {

        protected override Result OnUpdate()
        {
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
            return Result.Success;
        }

        protected override void OnCache()
        {
        }
        protected override void OnReset()
        {
        }
    }
}