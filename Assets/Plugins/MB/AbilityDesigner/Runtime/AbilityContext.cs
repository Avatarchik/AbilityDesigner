using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.AbilityDesigner
{
    public static class AbilityContext
    {
        public static PhaseList phaseList { get; internal set; }
        
        public static IAbilityUser originator { get; internal set; }
        public static IAbilityUser target { get; internal set; }

        internal static SubInstanceLink subInstanceLink { get; set; }

        internal static List<IAbilityUser> allreadyHit { get; set; }
        internal static List<IAbilityUser> currentHit { get; set; }

        internal static AbilityInstance instance { get; set; }

        #region Sub Instance Link Redirects

        public static GameObject gameObject
        {
            get { return subInstanceLink.obj; }
        }
        public static Transform transform
        {
            get { return subInstanceLink.transform; }
        }
        public static ParticleSystem particleSystem
        {
            get { return subInstanceLink.particleSystem; }
        }
        public static Collider collider
        {
            get { return subInstanceLink.collider; }
        }
        public static MeshFilter meshFilter
        {
            get { return subInstanceLink.meshFilter; }
        }
        public static MeshRenderer meshRenderer
        {
            get { return subInstanceLink.meshRenderer; }
        }

        public static Vector3 direction
        {
            get { return subInstanceLink.direction; }
        }

        #endregion
    }
}