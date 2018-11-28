using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    [System.Serializable]
    public class PhaseList
    {
        public string title { get; internal set; }
        public bool instant { get; internal set; }

        internal Phases.Phase[] phases { get; set; }

        private int m_CurrentPhase;

        #region Phase Update

        internal void OnStart()
        {
            for (int p = 0; p < phases.Length; p++)
            {
                phases[p].OnInternalStart();
            }
        }

        internal void OnCast()
        {
            for (int p = 0; p < phases.Length; p++)
            {
                phases[p].OnInternalCast();
            }
            m_CurrentPhase = 0;
        }

        internal void OnReset()
        {
            for (int p = 0; p < phases.Length; p++)
            {
                phases[p].OnInternalReset();
            }
            m_CurrentPhase = 0;
        }

        internal Result OnUpdate()
        {
            if (instant)
            {
                Result listResult = Result.Running;
                while (listResult == Result.Running)
                {
                    listResult = UpdateList();
                }
                return listResult;
            }
            return UpdateList();
        }

        private Result UpdateList()
        {
            Result result = phases[m_CurrentPhase].OnInternalUpdate();
            switch (result)
            {
                case Result.Success:
                    m_CurrentPhase++;
                    if (m_CurrentPhase >= phases.Length)
                    {
                        m_CurrentPhase = 0;
                        return Result.Success;
                    }
                    break;
                case Result.Fail:
                    // Break the entire ability
                    if (phases[m_CurrentPhase].breakOnFail)
                    {
                        m_CurrentPhase = 0;
                        return Result.Fail;
                    }
                    // if the phase is conditional then keep running until success
                    if (phases[m_CurrentPhase].GetType().IsSubclassOf(typeof(Phases.ConditionPhase)))
                    {
                        return Result.Running;
                    }
                    // Just abandon the current list
                    return Result.Success;
            }
            return Result.Running;
        }

        #endregion

        #region Structure

        internal void Destroy()
        {
            for (int p = 0; p < phases.Length; p++)
            {
                Object.DestroyImmediate(phases[p], true);
            }
        }

        internal PhaseList Clone()
        {
            PhaseList clone = new PhaseList();
            clone.title = title;
            clone.instant = instant;
            clone.m_CurrentPhase = m_CurrentPhase;

            clone.phases = new Phases.Phase[phases.Length];
            for (int p = 0; p < phases.Length; p++)
            {
                clone.phases[p] = ScriptableObject.Instantiate<Phases.Phase>(phases[p]);
            }

            return clone;
        }

        #endregion

    }
}