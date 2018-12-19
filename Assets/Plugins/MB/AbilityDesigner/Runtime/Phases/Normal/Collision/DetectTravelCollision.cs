using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    [PhaseCategory("Collision")]
    public class DetectTravelCollision : Phase
    {

        public float m_HitRadius;
        public LayerMask m_Layer;
        public PhaseListLink m_RunOnHit;

        private Vector3 m_LastPosition;
        private List<Collider> m_AlreadyHit;

        protected override void OnCast()
        {
            m_LastPosition = transform.position;
            m_AlreadyHit = new List<Collider>();
        }

        protected override Result OnUpdate()
        {
            Collider[] collision = Physics.OverlapCapsule(m_LastPosition, transform.position, m_HitRadius, m_Layer);
            List<Collider> currentHits = new List<Collider>();
            List<Collider> validHits = new List<Collider>();
            for (int h = 0; h < collision.Length; h++)
            {
                if (!m_AlreadyHit.Contains(collision[h]))
                {
                    validHits.Add(collision[h]);
                }
            }
            for (int h = m_AlreadyHit.Count; h >= 0; h--)
            {
                if (!currentHits.Contains(m_AlreadyHit[h]))
                {
                    m_AlreadyHit.RemoveAt(h);
                }
            }

            // TODO: Set shared collider array to found colliders

            if (collision.Length > 0)
            {
                m_RunOnHit.RunList();
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