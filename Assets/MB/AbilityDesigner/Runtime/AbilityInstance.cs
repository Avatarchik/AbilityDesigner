using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    public class AbilityInstance : MonoBehaviour
    {
        public bool isUpdating { get; private set; }
        public bool isDying { get; private set; }

        internal PhaseList[] phaseLists { get; set; }
        internal GameObject[] subInstances { get; private set; }

        private Stack<int> m_LastList;

        internal void Initiate(GameObject[] prefabs)
        {
            // TODO: convert to sub instance wrapper
            subInstances = new GameObject[prefabs.Length];
            for (int g = 0; g < prefabs.Length; g++)
            {
                GameObject prefabInstance = Instantiate(prefabs[g]);
                prefabInstance.name = name + " (Sub Instance " + g + ")";
                subInstances[g] = prefabInstance;
            }

            for (int p = 0; p < phaseLists.Length; p++)
            {
                phaseLists[p].OnStart();
            }
        }

        internal void Cast()
        {
            m_LastList = new Stack<int>();
            m_LastList.Push(0);

            for (int p = 0; p < phaseLists.Length; p++)
            {
                phaseLists[p].OnCast();
            }

            isUpdating = true;
        }

        private void Update()
        {
            if (!isUpdating || isDying)
            {
                return;
            }

            Result result = phaseLists[m_LastList.Peek()].OnUpdate();
            switch (result)
            {
                case Result.Success:
                    m_LastList.Pop();
                    // If default list is finished init ability dying
                    if (m_LastList.Count <= 0)
                    {
                        isDying = true;
                        return;
                    }
                    break;
                case Result.Fail:
                    isDying = true;
                    break;
            }
        }

        internal void Reset()
        {
            isDying = false;
            isUpdating = false;

            for (int p = 0; p < phaseLists.Length; p++)
            {
                phaseLists[p].OnReset();
            }
        }

        private void DefineContext(GameObject subInstance) // TODO: convert to subinstance wrapper
        {
            // TODO: Define context
        }

        internal void RunList(PhaseList list)
        {
            for (int p = 0; p < phaseLists.Length; p++)
            {
                if (phaseLists[p] == list)
                {
                    m_LastList.Push(p);
                    return;
                }
            }
        }

        private void OnDestroy()
        {
            for (int p = 0; p < phaseLists.Length; p++)
            {
                phaseLists[p].Destroy();
            }
        }
    }
}