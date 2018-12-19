using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    [PhaseCategory("Particle")]
    public class SetParticleSystemMesh : Phase
    {

        public SharedMesh m_Mesh;

        protected override Result OnUpdate()
        {
            if (particleSystem != null)
            {
                ParticleSystem.ShapeModule shape = particleSystem.shape;
                shape.mesh = m_Mesh.Value;
            }
            return Result.Success;
        }

        private Mesh m_CachedMesh;

        protected override void OnCache()
        {
            if (particleSystem != null)
            {
                m_CachedMesh = particleSystem.shape.mesh;
            }
        }
        protected override void OnReset()
        {
            if (particleSystem != null)
            {
                ParticleSystem.ShapeModule shape = particleSystem.shape;
                shape.mesh = m_CachedMesh;
            }
        }
    }
}