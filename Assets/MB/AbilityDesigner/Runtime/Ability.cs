using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    public enum Result { Success, Fail, Running }

    public class Ability : ScriptableObject
    {
        public string title { get; internal set; }
        public string description { get; internal set; }
        public Texture icon { get; internal set; }

        public float[] cooldowns { get; internal set; }

        public int maxCountGlobal { get; internal set; }
        public int maxCountUser { get; internal set; }

        internal PhaseList[] phaseLists { get; set; }
        // TODO: sub instances
        // TODO: shared variables

        public GameObject CreateRuntimeInstance()
        {
            GameObject gameObject = new GameObject(title + " Runtime Instance");
            AbilityInstance instance = gameObject.AddComponent<AbilityInstance>();

            instance.phaseLists = new PhaseList[phaseLists.Length];
            for (int p = 0; p < phaseLists.Length; p++)
            {
                instance.phaseLists[p] = phaseLists[p].Clone();
            }

            return gameObject;
        }
    }
}