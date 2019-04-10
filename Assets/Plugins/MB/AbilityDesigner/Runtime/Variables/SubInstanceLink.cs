using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    [System.Serializable]
    public class SubInstanceLink : ScriptableObject
    {
        #if UNITY_EDITOR
        #region Editor Fields

        [SerializeField]
        private bool m_Foldout = true;
        internal bool foldout { get { return m_Foldout; } set { m_Foldout = value; } }

        #endregion
        #endif

        [SerializeField]
        private int m_ID;
        internal int id { get { return m_ID; } set { m_ID = value; } }

        [SerializeField]
        private string m_Title;
        public string title { get { return m_Title; } internal set { m_Title = value; } }

        public enum Spawn { Originator, Target, Zero };
        [SerializeField]
        private Spawn m_Spawn;
        public Spawn spawn { get { return m_Spawn; } internal set { m_Spawn = value; } }

        [SerializeField]
        private Vector3 m_SpawnOffset;
        public Vector3 spawnOffset { get { return m_SpawnOffset; } internal set { m_SpawnOffset = value; } }

        [SerializeField]
        private GameObject m_Obj;
        public GameObject obj { get { return m_Obj; } internal set { m_Obj = value; } }

        // Cached data
        internal Transform transform { get; private set; }
        internal ParticleSystem particleSystem { get; private set; }
        internal Collider collider { get; private set; }
        internal MeshFilter meshFilter { get; private set; }
        internal MeshRenderer meshRenderer { get; private set; }
        internal MeshRenderer[] meshRendererAll { get; private set; }

        // Sub instance runtime data
        internal Vector3 direction { get; set; }

        internal Component[] RegisteredComponents()
        {
            if (obj == null)
            {
                return new Component[0];
            }
            return obj.GetComponents<Component>();
        }

        internal void CacheComponents()
        {
            transform = obj.transform;
            particleSystem = obj.GetComponent<ParticleSystem>();
            collider = obj.GetComponent<Collider>();
            meshFilter = obj.GetComponent<MeshFilter>();
            meshRenderer = obj.GetComponent<MeshRenderer>();
            meshRendererAll = obj.GetComponentsInChildren<MeshRenderer>(true);
        }

        internal static SubInstanceLink CreateInstance(int id)
        {
            SubInstanceLink link = CreateInstance<SubInstanceLink>();
            link.id = id;
            return link;
        }
    }
}