using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matki.AbilityDesigner.Edit
{
    [CreateAssetMenu(menuName = "Ability Designer/Editor Skin", order = 53)]
    public class EditorSkin : ScriptableObject
    {
        #region Meta

        [SerializeField]
        private string m_Title;
        public string title { get { return m_Title; } internal set { m_Title = value; } }

        [SerializeField]
        private int m_SortOrder;
        public int sortOrder { get { return m_SortOrder; } internal set { m_SortOrder = value; } }

        public string orderValue { get { return sortOrder + title; } }

        #endregion

        #region Color Settings

        [SerializeField]
        private Color m_SubInstanceLink_Color = new Color(1, 1, 1);
        internal Color subInstanceLink_Color { get { return m_SubInstanceLink_Color; } set { m_SubInstanceLink_Color = value; } }
        [SerializeField]
        private Color m_PhaseList_Color = new Color(1, 1, 1);
        internal Color phaseList_Color { get { return m_PhaseList_Color; } set { m_PhaseList_Color = value; } }
        [SerializeField]
        private Color m_SharedVariable_Color = new Color(1, 1, 1);
        internal Color sharedVariable_Color { get { return m_SharedVariable_Color; } set { m_SharedVariable_Color = value; } }

        #endregion

        #region Layout Settings

        [SerializeField]
        private float m_InspectorWidth = 320f;
        internal float inspectorWidth { get { return m_InspectorWidth; } set { m_InspectorWidth = value; } }

        [SerializeField]
        private float m_PhaseListWidth = 300f;
        internal float phaseListWidth { get { return m_PhaseListWidth; } set { m_PhaseListWidth = value; } }
        [SerializeField]
        private Vector2 m_PhaseListSpacing = new Vector2(20f, 10f);
        internal Vector2 phaseListSpacing { get { return m_PhaseListSpacing; } set { m_PhaseListSpacing = value; } }

        [SerializeField]
        private float m_PhaseTitleHeight = 80f;
        internal float phaseTitleHeight { get { return m_PhaseTitleHeight; } set { m_PhaseTitleHeight = value; } }
        [SerializeField]
        private float m_PhaseSpacing = 10f;
        internal float phaseSpacing { get { return m_PhaseSpacing; } set { m_PhaseSpacing = value; } }

        public enum PhaseIconLayout { Centered, Flat, NoIcon };
        [SerializeField]
        private PhaseIconLayout m_PhaseIconLayout = PhaseIconLayout.Centered;
        internal PhaseIconLayout phaseIconLayout { get { return m_PhaseIconLayout; } set { m_PhaseIconLayout = value; } }


        #endregion
    }
}