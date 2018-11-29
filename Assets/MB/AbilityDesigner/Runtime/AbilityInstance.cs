using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    public class AbilityInstance : MonoBehaviour
    {
        public Ability ability { get; internal set; }

        public bool isUpdating { get; private set; }
        public bool isDying { get; private set; }
        
        internal PhaseList[] phaseLists { get; set; }
        internal SubInstanceLink[] subInstanceLinks { get; set; }
        internal SharedVariable[] sharedVariables { get; set; }

        private Stack<int> m_LastList;

        private string m_SaveID;
        private IAbilityUser m_Originator;
        private IAbilityUser m_Target;

        internal void Initiate()
        {
            for (int p = 0; p < phaseLists.Length; p++)
            {
                phaseLists[p].OnStart();
            }
        }

        internal void Cast(IAbilityUser originator, IAbilityUser target)
        {
            m_SaveID = m_Originator.GetInstanceID() + "";
            m_Originator = originator;
            m_Target = target;

            m_LastList = new Stack<int>();
            m_LastList.Push(0);

            for (int s = 0; s < subInstanceLinks.Length; s++)
            {
                SubInstanceLink link = subInstanceLinks[s];

                // Set the correct spawn position
                switch (link.spawn)
                {
                    case SubInstanceLink.Spawn.Originator:
                        link.obj.transform.position = originator.GetCenter() + link.spawnOffset;
                        break;
                    case SubInstanceLink.Spawn.Target:
                        link.obj.transform.position = target.GetCenter() + link.spawnOffset;
                        break;
                    case SubInstanceLink.Spawn.Zero:
                        link.obj.transform.position = link.spawnOffset;
                        break;
                }

                // Start particle system
                if (link.particleSystem != null)
                {
                    link.particleSystem.Play();
                }

                // Show mesh renderer
                if (link.meshRenderer != null)
                {
                    link.meshRenderer.enabled = true;
                }

                if (target != null)
                {
                    link.direction = (target.GetCenter() - originator.GetCenter()).normalized;
                }
                else
                {
                    link.direction = Vector3.zero;
                }

                subInstanceLinks[s].obj.SetActive(true);
            }

            for (int p = 0; p < phaseLists.Length; p++)
            {
                DefineContext(phaseLists[p]);
                phaseLists[p].OnCast();
            }

            isUpdating = true;
        }

        private void Update()
        {
            if (isDying && IsDead())
            {
                Reset();
                return;
            }

            if (!isUpdating || isDying)
            {
                return;
            }

            PhaseList list = phaseLists[m_LastList.Peek()];
            DefineContext(list);
            Result result = list.OnUpdate();
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

            for (int s = 0; s < subInstanceLinks.Length; s++)
            {
                // Set the correct idle position
                subInstanceLinks[s].obj.transform.localPosition = Vector3.zero;
                subInstanceLinks[s].obj.SetActive(false);
            }
        }

        private bool IsDead()
        {
            bool somethingAlive = false;

            for (int s = 0; s < subInstanceLinks.Length; s++)
            {
                SubInstanceLink link = subInstanceLinks[s];

                // Test if any particle system is running and alive
                if (link.particleSystem != null)
                {
                    if (link.particleSystem.isPlaying)
                    {
                        link.particleSystem.Stop();
                    }
                    if (subInstanceLinks[s].particleSystem.IsAlive())
                    {
                        somethingAlive = true;
                    }
                }

                // Hide mesh renderer
                if (link.meshRenderer != null)
                {
                    link.meshRenderer.enabled = false;
                }
            }

            return !somethingAlive;
        }

        private void DefineContext(PhaseList list)
        {
            AbilityContext.phaseList = list;

            AbilityContext.originator = m_Originator;
            AbilityContext.target = m_Target;
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

            for (int s = 0; s < sharedVariables.Length; s++)
            {
                DestroyImmediate(sharedVariables[s], false);
            }
        }
    }
}