using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Matki.AbilityDesigner
{
    public class AbilityDesignerWindow : EditorWindow
    {
        private const float INSPECTOR_WIDTH = 300f;

        private enum Tab { General, Structure, Variables, Inspector };
        private Tab m_CurrentlyActive;

        Ability m_Ability;

        #region Styles

        bool m_StylesInitialized;
        GUIStyle m_PhaseBackground;
        GUIStyle m_Toolbar, m_ToolbarButton, m_ToolbarButtonSelected, m_ToolbarDropdown, m_ToolbarPopup;
        GUIStyle m_Header;
        GUIStyle m_SeperationBox;

        private void InitStyles()
        {
            if (m_StylesInitialized)
            {
                return;
            }

            m_PhaseBackground = new GUIStyle("CurveEditorBackground");

            m_Toolbar = new GUIStyle(EditorStyles.toolbar);
            m_Toolbar.padding = new RectOffset(0, 0, 0, 0);
            m_ToolbarButton = new GUIStyle(EditorStyles.toolbarButton);
            m_ToolbarButtonSelected = new GUIStyle("TE toolbarbutton");
            m_ToolbarDropdown = new GUIStyle(EditorStyles.toolbarDropDown);
            m_ToolbarPopup = new GUIStyle(EditorStyles.toolbarPopup);

            m_Header = new GUIStyle("HeaderLabel");

            m_SeperationBox = new GUIStyle("GroupBox");

            m_StylesInitialized = true;
        }

        #endregion

        [MenuItem("Tools/Ability Designer/Editor")]
        private static void Init()
        {
            AbilityDesignerWindow window = GetWindow<AbilityDesignerWindow>();
            window.Show();
        }

        private void OnEnable()
        {
            minSize = new Vector2(400f, 400f);
            titleContent = new GUIContent("Ability Designer");
        }

        private void OnSelectionChange()
        {
            Ability[] selectedAbilities = Selection.GetFiltered<Ability>(SelectionMode.Assets);
            if (selectedAbilities == null || selectedAbilities.Length == 0)
            {
                if (m_Ability == null)
                {
                    return;
                }
                m_Ability = null;
                Repaint();
                return;
            }

            if (m_Ability != selectedAbilities[0])
            {
                m_Ability = selectedAbilities[0];
                Repaint();
            }
        }

        private void OnGUI()
        {
            InitStyles();

            if (m_Ability == null)
            {
                return;
            }

            // Toolbar Area
            Rect rect = new Rect(0f, 0f, position.width, m_Toolbar.fixedHeight);
            GUILayout.BeginArea(new Rect(0f, 0f, position.width, m_Toolbar.fixedHeight), m_Toolbar);
            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(INSPECTOR_WIDTH));
            GUILayout.Space(10f);
            TabArea();
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(position.width - INSPECTOR_WIDTH));
            GUILayout.Space(10f);
            // TODO: Draw Custom Stuff
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            // Left Area
            Rect leftRect = new Rect(0f, rect.height, INSPECTOR_WIDTH, position.height - rect.height);
            GUILayout.BeginArea(leftRect);
            leftRect = new Rect(5f, 5f, INSPECTOR_WIDTH - 10f, position.height - rect.height - 10f);
            GUILayout.BeginArea(leftRect);
            SelectEditorArea(new Rect(0f, 0f, leftRect.width, leftRect.height));
            GUILayout.EndArea();
            GUILayout.EndArea();

            // Right Area
            GUILayout.BeginArea(new Rect(INSPECTOR_WIDTH, rect.height, position.width - INSPECTOR_WIDTH, position.height - rect.height), m_PhaseBackground);
            GUILayout.EndArea();
        }

        void TabArea()
        {
            if (GUILayout.Button(new GUIContent("General"), m_CurrentlyActive == Tab.General ? m_ToolbarButtonSelected : m_ToolbarButton))
            {
                m_CurrentlyActive = Tab.General;
                Repaint();
            }
            if (GUILayout.Button(new GUIContent("Structure"), m_CurrentlyActive == Tab.Structure ? m_ToolbarButtonSelected : m_ToolbarButton))
            {
                m_CurrentlyActive = Tab.Structure;
                Repaint();
            }
            if (GUILayout.Button(new GUIContent("Variables"), m_CurrentlyActive == Tab.Variables ? m_ToolbarButtonSelected : m_ToolbarButton))
            {
                m_CurrentlyActive = Tab.Variables;
                Repaint();
            }
            if (GUILayout.Button(new GUIContent("Inspector"), m_CurrentlyActive == Tab.Inspector ? m_ToolbarButtonSelected : m_ToolbarButton))
            {
                m_CurrentlyActive = Tab.Inspector;
                Repaint();
            }
        }

        void SelectEditorArea(Rect rect)
        {
            switch (m_CurrentlyActive)
            {
                case Tab.General:
                    GeneralArea(rect);
                    break;
                case Tab.Structure:
                    break;
                case Tab.Variables:
                    break;
                case Tab.Inspector:
                    break;
            }
        }

        void GeneralArea(Rect rect)
        {
            Rect metaRect = new Rect(0f, 0f, rect.width, 130f);
            GUILayout.BeginArea(metaRect, m_SeperationBox);
            EditorGUI.LabelField(new Rect(5f, 5f, metaRect.width - 10f, 15f), new GUIContent("Meta"), m_Header);
            m_Ability.icon = (Texture)EditorGUI.ObjectField(new Rect(5f, 25f, 100f, 100f), m_Ability.icon, typeof(Texture), false);
            m_Ability.title = EditorGUI.TextField(new Rect(110f, 25f, metaRect.width - 115f, 15f), m_Ability.title);
            m_Ability.description = EditorGUI.TextArea(new Rect(110f, 45f, metaRect.width - 115f, 80f), m_Ability.description);
            GUILayout.EndArea();
            
            Rect rulesRect = new Rect(0f, metaRect.y + metaRect.height + 5f, rect.width, 105f + m_Ability.cooldowns.Length * 20f);
            GUILayout.BeginArea(rulesRect, m_SeperationBox);
            EditorGUI.LabelField(new Rect(5f, 5f, rulesRect.width - 10f, 15f), new GUIContent("Rules"), m_Header);
            m_Ability.maxCountGlobal = EditorGUI.IntField(new Rect(5f, 25f, rulesRect.width - 10f, 15f), new GUIContent("Max Count Global"), m_Ability.maxCountGlobal);
            m_Ability.maxCountUser = EditorGUI.IntField(new Rect(5f, 45f, rulesRect.width - 10f, 15f), new GUIContent("Max Count User"), m_Ability.maxCountUser);

            EditorGUI.LabelField(new Rect(5f, 65f, rulesRect.width - 10f, 15f), new GUIContent("Cooldowns"), m_Header);
            if (m_Ability.cooldowns.Length < 1)
            {
                m_Ability.cooldowns = new float[1];
            }
            m_Ability.cooldowns[0] = EditorGUI.FloatField(new Rect(5f, 85f, rulesRect.width - 10f, 15f), m_Ability.cooldowns[0]);
            for (int c = 1; c < m_Ability.cooldowns.Length; c++)
            {
                m_Ability.cooldowns[c] = EditorGUI.FloatField(new Rect(5f, 85f + c * 20f, rulesRect.width - 70f, 15f), m_Ability.cooldowns[c]);
                if (GUI.Button(new Rect(rulesRect.width - 60f, 85f + c * 20f, 55f, 15f), new GUIContent("Delete")))
                {
                    List<float> cooldowns = new List<float>(m_Ability.cooldowns);
                    cooldowns.RemoveAt(c);
                    m_Ability.cooldowns = cooldowns.ToArray();
                }
            } 
            if (GUI.Button(new Rect(5f, 85f + m_Ability.cooldowns.Length * 20f, rulesRect.width - 10f, 15f), new GUIContent("Add")))
            {
                List<float> cooldowns = new List<float>(m_Ability.cooldowns);
                cooldowns.Add(0f);
                m_Ability.cooldowns = cooldowns.ToArray();
            }
            GUILayout.EndArea();

            Rect poolingRect = new Rect(0f, rulesRect.y + rulesRect.height + 5f, rect.width, 45f);
            GUILayout.BeginArea(poolingRect, m_SeperationBox);
            EditorGUI.LabelField(new Rect(5f, 5f, metaRect.width - 10f, 15f), new GUIContent("Pooling"), m_Header);
            m_Ability.poolingChunkSize = EditorGUI.IntField(new Rect(5f, 25f, rulesRect.width - 10f, 15f), new GUIContent("Chunk Size"), m_Ability.poolingChunkSize);
            GUILayout.EndArea();

            EditorUtility.SetDirty(m_Ability);
        }

        /*
        [SerializeField]
        private float[] m_Cooldowns;
        public float[] cooldowns { get { return m_Cooldowns; } internal set { m_Cooldowns = value; } }

        [SerializeField]
        private int m_MaxCountGlobal;
        public int maxCountGlobal { get { return m_MaxCountGlobal; } internal set { m_MaxCountGlobal = value; } }
        [SerializeField]
        private int m_MaxCountUser;
        public int maxCountUser { get { return m_MaxCountUser; } internal set { m_MaxCountUser = value; } }

        [SerializeField]
        private int m_PoolingChunkSize;
        public int poolingChunkSize { get { return m_PoolingChunkSize; } internal set { m_PoolingChunkSize = value; } }*/
    }
}