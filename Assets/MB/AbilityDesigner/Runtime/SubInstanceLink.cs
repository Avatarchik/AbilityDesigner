using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    public class SubInstanceLink
    {
        [SerializeField]
        public string title { get; internal set; }

        public enum Spawn { Originator, Target, Zero };
        [SerializeField]
        public Spawn spawn { get; internal set; }

        [SerializeField]
        public Vector3 spawnOffset { get; internal set; }

        [SerializeField]
        public GameObject obj { get; internal set; }

        // Cached data
        internal Transform transform { get; private set; }
        internal ParticleSystem particleSystem { get; private set; }
        internal Collider collider { get; private set; }
        internal MeshFilter meshFilter { get; private set; }
        internal MeshRenderer meshRenderer { get; private set; }

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
        }
    }
}