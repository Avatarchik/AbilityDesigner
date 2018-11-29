using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Phases
{
    // TODO: sub variables (a variable which is different for all sub instances)

    public abstract class Phase : ScriptableObject
    {
        [SerializeField]
        public string customTitle { get; internal set; }

        [SerializeField]
        public bool instant { get; internal set; }
        [SerializeField]
        public bool breakOnFail { get; internal set; }

        [SerializeField]
        public SubInstanceLink[] runForSubInstances { get; internal set; }
        
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
            get { return AbilityContext.originator; }
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
        protected internal abstract void    OnInternalReset();
        
    }
}
