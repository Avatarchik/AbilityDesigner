using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;

namespace Matki.AbilityDesigner.Phases
{
    [PhaseIcon("{SkinIcons}Phase")]
    public abstract class Phase : ScriptableObject
    {
        [SerializeField]
        private string m_CustomTitle;
        public string customTitle { get { return m_CustomTitle; } internal set { m_CustomTitle = value; } }

        [SerializeField]
        private Color m_CustomColor = Color.white;
        public Color customColor { get { return m_CustomColor; } internal set { m_CustomColor = value; } }

        [SerializeField]
        private bool m_Instant;
        public bool instant { get { return m_Instant; } internal set { m_Instant = value; } }
        [SerializeField]
        private bool m_BreakOnFail;
        public bool breakOnFail { get { return m_BreakOnFail; } internal set { m_BreakOnFail = value; } }

        [SerializeField]
        private SubInstanceLink[] m_RunForSubInstances = new SubInstanceLink[0];
        public SubInstanceLink[] runForSubInstances { get { return m_RunForSubInstances; } internal set { m_RunForSubInstances = value; } }

        private delegate void ResetDelegate(FieldInfo[] infos, object[] objects);
        private ResetDelegate OnCachedReset;
        private FieldInfo[] m_Infos;
        private object[] m_CachedObjects;

        #region Context Redirects

        protected static PhaseList phaseList
        {
            get { return AbilityContext.phaseList; }
        }

        protected static IAbilityUser originator
        {
            get { return AbilityContext.originator; }
        }
        protected static IAbilityUser target
        {
            get { return AbilityContext.target; }
        }

        protected static GameObject gameObject
        {
            get { return AbilityContext.gameObject; }
        }
        protected static Transform transform
        {
            get { return AbilityContext.transform; }
        }
        protected static ParticleSystem particleSystem
        {
            get { return AbilityContext.particleSystem; }
        }
        protected static Collider collider
        {
            get { return AbilityContext.collider; }
        }
        protected static MeshFilter meshFilter
        {
            get { return AbilityContext.meshFilter; }
        }
        protected static MeshRenderer meshRenderer
        {
            get { return AbilityContext.meshRenderer; }
        }

        protected static Vector3 direction
        {
            get { return AbilityContext.direction; }
        }

        #endregion

        // Internal Continous Methods
        protected internal abstract void    OnInternalCast();
        protected internal abstract void    OnInternalStart();
        protected internal abstract Result  OnInternalUpdate();

        protected internal virtual void     OnInternalReset() { OnReset(); }
        protected internal virtual void     OnInternalCache() { OnCache(); }

        protected virtual void OnReset()
        {
            if (OnCachedReset != null)
            {
                OnCachedReset.Invoke(m_Infos, m_CachedObjects);
            }
        }

        protected virtual void OnCache()
        {
            FieldInfo[] infos = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<object> objects = new List<object>();
            List<FieldInfo> fields = new List<FieldInfo>();
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Name.Equals(nameof(m_CachedObjects)))
                {
                    continue;
                }
                if (infos[i].Name.Equals(nameof(m_Infos)))
                {
                    continue;
                }
                if (infos[i].Name.Equals(nameof(OnCachedReset)))
                {
                    continue;
                }
                objects.Add(infos[i].GetValue(this));
                fields.Add(infos[i]);
            }
            m_Infos = fields.ToArray();
            m_CachedObjects = objects.ToArray();
        }

        private void ResetFields(FieldInfo[] infos, object[] objects)
        {
            for (int f = 0; f < infos.Length; f++)
            {
                infos[f].SetValue(this, objects[f]);
            }
        }
        
    }
}
