using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    [System.Serializable]
    public class PhaseList
    {
        [SerializeField]
        private int m_ID;
        internal int id { get { return m_ID; } set { m_ID = value; } }

        [SerializeField]
        private string m_Title;
        public string title { get { return m_Title; } internal set { m_Title = value; } }
        [SerializeField]
        private bool m_Instant;
        public bool instant { get { return m_Instant; } internal set { m_Instant = value; } }

        [SerializeField]
        private Phases.PhaseCore[] m_Phases = new Phases.PhaseCore[0];
        internal Phases.PhaseCore[] phases { get { return m_Phases; } set { m_Phases = value; } }

        private int m_CurrentPhase;

        public PhaseList(int id)
        {
            this.id = id;
        }

        #region Phase Update

        private void DefineContext(SubInstanceLink subInstanceLink)
        {
            AbilityContext.subInstanceLink = subInstanceLink;
        }

        internal void OnStart()
        {
            for (int p = 0; p < phases.Length; p++)
            {
                for (int s = 0; s < phases[p].runForSubInstances.Length; s++)
                {
                    DefineContext(phases[p].runForSubInstances[s]);
                    phases[p].OnInternalStart();
                    phases[p].OnInternalCache();
                }
            }
        }

        internal void OnCast()
        {
            for (int p = 0; p < phases.Length; p++)
            {
                for (int s = 0; s < phases[p].runForSubInstances.Length; s++)
                {
                    DefineContext(phases[p].runForSubInstances[s]);
                    phases[p].OnInternalCast();
                }
            }
            m_CurrentPhase = 0;
        }

        internal void OnReset()
        {
            for (int p = 0; p < phases.Length; p++)
            {
                for (int s = 0; s < phases[p].runForSubInstances.Length; s++)
                {
                    DefineContext(phases[p].runForSubInstances[s]);
                    phases[p].OnInternalReset();
                }
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
            Result result = Result.Success;
            if (m_CurrentPhase >= phases.Length)
            {
                return Result.Success;
            }
            for (int s = 0; s < phases[m_CurrentPhase].runForSubInstances.Length; s++)
            {
                DefineContext(phases[m_CurrentPhase].runForSubInstances[s]);
                Result localResult = phases[m_CurrentPhase].OnInternalUpdate();
                switch (localResult)
                {
                    case Result.Fail:
                        result = Result.Fail;
                        break;
                    case Result.Running:
                        if (result != Result.Fail)
                        {
                            result = Result.Running;
                        }
                        break;
                }
            }
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
            PhaseList clone = new PhaseList(id);
            clone.title = title;
            clone.instant = instant;
            clone.m_CurrentPhase = m_CurrentPhase;

            clone.phases = new Phases.PhaseCore[phases.Length];
            for (int p = 0; p < phases.Length; p++)
            {
                clone.phases[p] = ScriptableObject.Instantiate<Phases.PhaseCore>(phases[p]);
            }

            return clone;
        }

        #endregion

    }
}