using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner.Phases
{
    [PhaseCategory("Collision")]
    [DefaultSubInstanceLinkOnly]
    public class DetectMeshCollision : Phase
    {

        public SharedMesh m_Mesh;

        public LayerMask m_Layer;
        public PhaseListLink m_RunOnHit;

        public SharedColliderList m_Hits;
        
        private List<Collider> m_AlreadyHit;

        private Collider m_OriginCollider;
        private Collider m_TargetCollider;

        protected override void OnCast()
        {
            m_OriginCollider = originator.gameObject.GetComponent<Collider>();
            m_TargetCollider = target.gameObject.GetComponent<Collider>();

            m_AlreadyHit = new List<Collider>();
        }

        protected override Result OnUpdate()
        {
            if (m_Mesh == null || m_Mesh.Value == null)
            {
                return Result.Fail;
            }
            Vector3 extents = new Vector3(m_Mesh.Value.bounds.extents.x, 100f, m_Mesh.Value.bounds.extents.z);
            Collider[] allCollider = Physics.OverlapBox(m_Mesh.Value.bounds.center, extents, Quaternion.identity, m_Layer);

            List<Collider> collision = new List<Collider>();
            for (int t = 0; t < allCollider.Length; t++)
            {
                if (PointInMesh(m_Mesh.Value, allCollider[t].transform.position))
                {
                    collision.Add(allCollider[t].GetComponent<Collider>());
                }
            }
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

        private bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
        {
            Vector3 cp1 = Vector3.Cross(b - a, p1 - a).normalized;
            Vector3 cp2 = Vector3.Cross(b - a, p2 - a).normalized;
            return Vector3.Dot(cp1, cp2) >= 0;
        }

        private bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            return SameSide(p, a, b, c) && SameSide(p, b, a, c) && SameSide(p, c, a, b);
        }

        private bool PointInMesh(Mesh mesh, Vector3 point)
        {
            if (!mesh)
            {
                return false;
            }

            Vector2 searchPoint = new Vector2(point.x, point.z);

            Vector2[] points = new Vector2[mesh.vertices.Length];
            for (int v = 0; v < points.Length; v++)
            {
                points[v] = new Vector2(mesh.vertices[v].x, mesh.vertices[v].z);
            }

            for (int t = 0; t < mesh.triangles.Length - 4; t += 3)
            {
                if (PointInTriangle(searchPoint, points[mesh.triangles[t]], points[mesh.triangles[t + 1]], points[mesh.triangles[t + 2]]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
