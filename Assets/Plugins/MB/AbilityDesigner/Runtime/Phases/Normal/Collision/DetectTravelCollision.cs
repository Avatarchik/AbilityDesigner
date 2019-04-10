using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseCategory("Collision")]
    public class DetectTravelCollision : Phase
    {

        public float m_HitRadius;
        public LayerMask m_Layer;
        public PhaseListLink m_RunOnHit;

        public SharedColliderList m_Hits;

        public bool m_ExcludeOriginator;
        public bool m_ExcludeTarget;

        private SubVector3 m_LastPosition = new SubVector3();
        private List<Collider> m_AlreadyHit;

        private Collider m_OriginCollider;
        private Collider m_TargetCollider;

        protected override void OnCast()
        {
            m_LastPosition.Value = transform.position;

            if (originator != null)
            {
                m_OriginCollider = originator.gameObject.GetComponent<Collider>();
            }
            if (target != null)
            {
                m_TargetCollider = target.gameObject.GetComponent<Collider>();
            }

            m_AlreadyHit = new List<Collider>();
        }

        protected override Result OnUpdate()
        {
            Collider[] collisions = Physics.OverlapCapsule(m_LastPosition.Value, transform.position, m_HitRadius, m_Layer);
            m_LastPosition.Value = transform.position;
            List<Collider> collision = new List<Collider>(collisions);
            if (m_ExcludeOriginator)
                collision.Remove(m_OriginCollider);
            if (m_ExcludeTarget)
                collision.Remove(m_TargetCollider);
            
            List<Collider> validHits = new List<Collider>();
            for (int h = 0; h < collision.Count; h++)
            {
                if (!m_AlreadyHit.Contains(collision[h]))
                {
                    validHits.Add(collision[h]);
                }
            }
            for (int h = m_AlreadyHit.Count - 1; h >= 0; h--)
            {
                if (!collision.Contains(m_AlreadyHit[h]))
                {
                    m_AlreadyHit.RemoveAt(h);
                }
            }
            m_AlreadyHit.AddRange(validHits);

            m_Hits.Value = validHits;

            if (collision.Count > 0)
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