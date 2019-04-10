using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MB.AbilityDesigner;
using MB.AbilityDesigner.Phases;

namespace MB.Abilities
{
    [PhaseCategory("Collision")]
    public class DetectBoxCollision : Phase
    {
        public enum Target { Originator, Target, AnchorPoint };
        public Target m_Target = Target.Target;

        public string m_AnchorPointName;

        public Bounds m_Bounds;

        public LayerMask m_Layer;
        public PhaseListLink m_RunOnHit;

        public SharedColliderList m_Hits;

        private List<Collider> m_AlreadyHit;
        private AbilityAnchorPoint m_AnchorPoint;

        private Collider m_OriginCollider;
        private Collider m_TargetCollider;

        protected override void OnCast()
        {
            List<AbilityAnchorPoint> points = new List<AbilityAnchorPoint>(originator.gameObject.GetComponentsInChildren<AbilityAnchorPoint>());
            m_AnchorPoint = points.Find(obj => obj.m_AnchorPointName == m_AnchorPointName);

            m_OriginCollider = originator.gameObject.GetComponent<Collider>();
            m_TargetCollider = target.gameObject.GetComponent<Collider>();

            m_AlreadyHit = new List<Collider>();
        }

        protected override Result OnUpdate()
        {
            if (m_Bounds == null)
            {
                return Result.Fail;
            }

            Collider[] allCollider = new Collider[0];

            switch (m_Target)
            {
                case Target.Target:
                    allCollider = Physics.OverlapBox(target.GetCenter() + m_Bounds.center, m_Bounds.extents, target.gameObject.transform.rotation, m_Layer);
                    break;
                case Target.Originator:
                    allCollider = Physics.OverlapBox(originator.GetCenter() + m_Bounds.center, m_Bounds.extents, originator.gameObject.transform.rotation, m_Layer);
                    break;
                case Target.AnchorPoint:
                    if (m_AnchorPoint == null)
                    {
                        return Result.Fail;
                    }
                    allCollider = Physics.OverlapBox(m_AnchorPoint.transform.position + m_Bounds.center, m_Bounds.extents, m_AnchorPoint.transform.rotation, m_Layer);
                    break;
            }

            List<Collider> collision = new List<Collider>(allCollider);
            collision.Remove(m_OriginCollider);
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
