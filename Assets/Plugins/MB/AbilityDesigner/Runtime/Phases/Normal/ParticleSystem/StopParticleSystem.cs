﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseCategory("Particle")]
    public class StopParticleSystem : Phase
    {

        protected override Result OnUpdate()
        {
            if (particleSystem != null)
            {
                particleSystem.Stop();
            }
            return Result.Success;
        }

        protected override void OnCache()
        {}
        protected override void OnReset()
        {
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }
    }
}