using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matki.ActorSystem;

namespace Matki.AbilityDesigner.Phases
{
    // TODO: sub bariables (a variable which is different for all sub instances)
    // TODO: shared variables

    public abstract class Phase : ScriptableObject
    {
        public string customTitle { get; internal set; }

        public bool instant { get; internal set; }
        public bool breakOnFail { get; internal set; }

        internal PhaseList phaseList { get; set; }
        // TODO: run for sub instances

        private GameObject gameObject;
        private Transform transform;
        private ParticleSystem particleSystem;
        private Collider collider;

        private Actor originator;
        private Actor target;

        private Vector3 direction;

        // Internal Continous Methods
        protected internal abstract void    OnInternalCast();
        protected internal abstract void    OnInternalStart();
        protected internal abstract Result  OnInternalUpdate();
        protected internal abstract void    OnInternalReset();

        // Shortcut for handle hits
        protected void HandleHits()
        {
            // TODO: Handle Hits redirect for phases
        }
    }
}
