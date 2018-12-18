using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner
{
    [System.Serializable]
    public class PhaseListLink
    {
        [SerializeField]
        private int m_ID = -1;
        public int id { get { return m_ID; } internal set { m_ID = value; } }

        public void RunList()
        {
            PhaseList list = AbilityContext.instance.IDToPhaseList(id);
            if (list != null)
            {
                AbilityContext.instance.RunList(list);
            }
        }
    }
}