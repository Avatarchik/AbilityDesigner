using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseCategory("Particle")]
    public class SetParticleSystemMesh : Phase
    {

        public SharedMesh m_Mesh;
        public float m_ParticlesPerMeter = 100;
        public float[] m_SubParticlesPerMeter = new float[] { 100 };

        private ParticleSystem[] m_Systems;

        protected override void OnStart()
        {
            m_Systems = gameObject.GetComponentsInChildren<ParticleSystem>();
        }

        protected override Result OnUpdate()
        {
            float area = 0f;
            int[] tris = m_Mesh.Value.triangles;
            Vector3[] verts = m_Mesh.Value.vertices;
            for (int t = 0; t < tris.Length - 2; t+= 3)
            {
                float a = Vector3.Distance(verts[tris[t]], verts[tris[t + 1]]);
                float b = Vector3.Distance(verts[tris[t + 1]], verts[tris[t + 2]]);
                float c = Vector3.Distance(verts[tris[t + 2]], verts[tris[t]]);
                
                float currentArea = TriangleArea(a, b, c);
                if (!float.IsNaN(currentArea))
                {
                    area += currentArea;
                }
            }

            if (particleSystem != null)
            {
                ParticleSystem.ShapeModule shape = particleSystem.shape;
                shape.mesh = m_Mesh.Value;

                ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
                emissionModule.rateOverTime = area * m_ParticlesPerMeter;
            }
            int currentSystem = 0;
            for (int s = 0; s < m_Systems.Length; s++)
            {
                if (m_Systems[s] == particleSystem)
                {
                    continue;
                }

                ParticleSystem.ShapeModule shape = m_Systems[s].shape;
                shape.mesh = m_Mesh.Value;

                ParticleSystem.EmissionModule emissionModule = m_Systems[s].emission;
                emissionModule.rateOverTime = area * m_SubParticlesPerMeter[Mathf.Clamp(currentSystem, 0, m_SubParticlesPerMeter.Length - 1)];

                currentSystem++;
            }
            return Result.Success;
        }

        private float TriangleArea(float a, float b, float c)
        {
            float s = (a + b + c) / 2f;
            return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
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