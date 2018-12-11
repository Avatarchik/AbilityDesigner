using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Matki.AbilityDesigner
{
    public class AbilityDesignerWindow : EditorWindow
    {
        private const float INSPECTOR_WIDTH = 320f;
        private static Color PARENT_COLOR = new Color(0.7f, 0.7f, 0.7f);

        #region Structure Fields

        private enum Tab { General, Structure, Variables, Inspector };
        private Tab m_CurrentlyActive;

        private event System.Action OnRepaint;

        private Vector2 m_GeneralScroll;
        private Vector2 m_StructureScroll;
        private Vector2 m_VariablesScroll;
        private Vector2 m_InspectorScroll;

        #endregion

        #region Private Fields

        private Ability m_Ability;
        private Phases.Phase m_SelectedPhase;

        #endregion

        #region Styles

        [System.NonSerialized]
        bool m_StylesInitialized;
        GUIStyle m_PhaseBackground;
        GUIStyle m_Toolbar, m_ToolbarButton, m_ToolbarButtonSelected, m_ToolbarDropdown, m_ToolbarPopup;
        GUIStyle m_Header, m_HeaderTitle, m_HeaderButton;
        GUIStyle m_SeperationBox;
        GUIStyle m_LineBox, m_LineEven, m_LineOdd;

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

            m_Header = new GUIStyle("ProgressBarBack");
            m_Header.fixedHeight = 0f;
            m_Header.alignment = TextAnchor.MiddleCenter;
            m_Header.margin = new RectOffset(0, 0, 0, 0);
            m_Header.padding = new RectOffset(5, 5, 0, 0);

            m_HeaderTitle = new GUIStyle("HeaderLabel");
            m_HeaderTitle.alignment = TextAnchor.MiddleLeft;

            m_HeaderButton = new GUIStyle("HeaderLabel");
            m_HeaderButton.alignment = TextAnchor.MiddleRight;

            m_SeperationBox = new GUIStyle("GroupBox");
            m_SeperationBox.padding = new RectOffset(5,5,5,5);
            m_SeperationBox.margin = new RectOffset(0,0,0,5);

            m_LineBox = new GUIStyle(EditorStyles.helpBox);
            m_LineBox.padding = new RectOffset(1, 1, 1, 1);
            m_LineEven = new GUIStyle("CN EntryBackEven");
            m_LineEven.padding = new RectOffset(0, 0, 0, 0);
            m_LineEven.margin = new RectOffset(0, 0, 0, 0);
            m_LineEven.border = new RectOffset(0, 0, 0, 0);
            m_LineOdd = new GUIStyle("CN EntryBackOdd");
            m_LineOdd.padding = new RectOffset(0, 0, 0, 0);
            m_LineOdd.margin = new RectOffset(0, 0, 0, 0);
            m_LineOdd.border = new RectOffset(0, 0, 0, 0);

            m_StylesInitialized = true;
        }

        #endregion

        #region Helper Methods

        private void BeginInspectorGroup(GUIContent content, bool parent = false, GUIContent buttonInfo = null, System.Action<Rect> onButtonCall = null)
        {
            GUILayout.BeginVertical();
            GUI.backgroundColor = parent ? PARENT_COLOR : Color.white;
            GUILayout.BeginHorizontal(GUILayout.Height(25f));
            GUILayout.BeginHorizontal(m_Header, GUILayout.Height(25f));
            EditorGUILayout.LabelField(content, m_HeaderTitle, GUILayout.ExpandHeight(true));
            GUILayout.EndHorizontal();
            if (buttonInfo != null)
            {
                GUILayout.BeginHorizontal(GUILayout.Height(25f));
                Rect button = GUILayoutUtility.GetRect(buttonInfo, m_Header, GUILayout.Height(25f), GUILayout.ExpandWidth(true));
                if (GUI.Button(button, buttonInfo, m_Header))
                {
                    if (onButtonCall != null)
                    {
                        onButtonCall.Invoke(button);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical(m_SeperationBox);
            EditorGUILayout.LabelField(GUIContent.none, GUILayout.Height(0f));
        }

        private void EndInspectorGroup()
        {
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        #endregion

        #region Window

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
            leftRect = new Rect(0f, 0f, INSPECTOR_WIDTH, position.height - rect.height);
            GUILayout.BeginArea(leftRect);
            SelectEditorArea(new Rect(0f, 0f, leftRect.width, leftRect.height));
            GUILayout.EndArea();
            GUILayout.EndArea();

            // Right Area
            GUILayout.BeginArea(new Rect(INSPECTOR_WIDTH, rect.height, position.width - INSPECTOR_WIDTH, position.height - rect.height), m_PhaseBackground);
            GUILayout.EndArea();

            if (OnRepaint != null)
            {
                OnRepaint.Invoke();
            }
        }

        #endregion

        #region Left Area

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
            EditorGUI.BeginChangeCheck();

            switch (m_CurrentlyActive)
            {
                case Tab.General:
                    GeneralArea(rect);
                    break;
                case Tab.Structure:
                    StuctureArea(rect);
                    break;
                case Tab.Variables:
                    VariablesArea(rect);
                    break;
                case Tab.Inspector:
                    InspectorArea(rect);
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_Ability);
            }
        }

        #endregion

        #region General Area

        void GeneralArea(Rect rect)
        {
            m_GeneralScroll = EditorGUILayout.BeginScrollView(m_GeneralScroll, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
            GUILayout.Space(5f);
            EditorGUILayout.BeginVertical(GUILayout.Width(rect.width - 14f));

            // Meta Area
            BeginInspectorGroup(new GUIContent("Meta"), true);
            GUILayout.BeginHorizontal();
            m_Ability.icon = (Texture)EditorGUILayout.ObjectField(m_Ability.icon, typeof(Texture), false, GUILayout.Width(100f), GUILayout.Height(100f));
            GUILayout.BeginVertical(GUILayout.Height(100f));
            m_Ability.title = EditorGUILayout.TextField(m_Ability.title);
            m_Ability.description = EditorGUILayout.TextArea(m_Ability.description, GUILayout.ExpandHeight(true));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            EndInspectorGroup();

            // Pooling Area
            BeginInspectorGroup(new GUIContent("Pooling"), true);
            m_Ability.poolingChunkSize = EditorGUILayout.IntField(new GUIContent("Chunk Size"), m_Ability.poolingChunkSize);
            EndInspectorGroup();

            // Pooling Area
            BeginInspectorGroup(new GUIContent("Cast Rules"), true, new GUIContent("Add New"), CastRuleDropdown);
            for (int r = 0; r < m_Ability.castRules.Length; r++)
            {
                DrawCastRule(r);
            }
            EndInspectorGroup();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawCastRule(int rule)
        {
            // Cast Rule Area
            BeginInspectorGroup(new GUIContent(ObjectNames.NicifyVariableName(m_Ability.castRules[rule].GetType().Name)), false, new GUIContent("Delete"), delegate (Rect buttonRect)
            {
                int index = rule;
                OnRepaint = delegate ()
                {
                    RemoveCastRuleAt(index);
                    OnRepaint = null;
                };
                Repaint();
            });

            EditorGUI.BeginChangeCheck();
            Editor editor = Editor.CreateEditor(m_Ability.castRules[rule]);
            editor.DrawDefaultInspector();
            DestroyImmediate(editor);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_Ability.castRules[rule]);
            }

            EndInspectorGroup();
        }
        


        // <>-------------------<Calls>-------------------<>
        
        private void CastRuleDropdown(Rect pos)
        {
            List<CastRule> rules = new List<CastRule>(m_Ability.castRules);
            string[] guids = AssetDatabase.FindAssets("t:MonoScript");
            List<System.Type> types = new List<System.Type>();
            for (int g = 0; g < guids.Length; g++)
            {
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guids[g]));
                System.Type scriptType = script.GetClass();
                if (scriptType != null && scriptType.IsSubclassOf(typeof(CastRule)))
                {
                    if (!rules.Exists(rule => rule.GetType() == scriptType))
                    {
                        types.Add(scriptType);
                    }
                }
            }

            GenericMenu menu = new GenericMenu();
            for (int t = 0; t < types.Count; t++)
            {
                menu.AddItem(new GUIContent(types[t].Name), false, AddCastRuleOfType, types[t]);
            }
            menu.DropDown(pos);
        }

        private void AddCastRuleOfType(object obj)
        {
            System.Type type = (System.Type)obj;
            List<CastRule> rules = new List<CastRule>(m_Ability.castRules);
            CastRule rule = (CastRule)CreateInstance(type);
            rules.Add(rule);
            m_Ability.castRules = rules.ToArray();
            AssetDatabase.AddObjectToAsset(rule, m_Ability);
        }

        private void RemoveCastRuleAt(int index)
        {
            List<CastRule> rules = new List<CastRule>(m_Ability.castRules);
            CastRule rule = rules[index];
            rules.RemoveAt(index);
            DestroyImmediate(rule, true);
            m_Ability.castRules = rules.ToArray();
        }

        #endregion

        #region Structure Area

        void StuctureArea(Rect rect)
        {
            m_StructureScroll = EditorGUILayout.BeginScrollView(m_StructureScroll, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
            GUILayout.Space(5f);
            EditorGUILayout.BeginVertical(GUILayout.Width(rect.width - 14f));

            if (m_Ability.phaseLists.Length < 1)
            {
                PhaseList list = AddPhaseList();
                list.title = "Default";
            }

            BeginInspectorGroup(new GUIContent("Phase Lists"), true, new GUIContent("Add New"), delegate(Rect buttonRect) { AddPhaseList(); });
            
            DrawPhaseList(0, true);

            for (int l = 1; l < m_Ability.phaseLists.Length; l++)
            {
                DrawPhaseList(l);
            }

            EndInspectorGroup();

            if (m_Ability.subInstanceLinks.Length < 1)
            {
                SubInstanceLink link = AddSubInstanceLink();
                link.title = "Default";
            }
            BeginInspectorGroup(new GUIContent("Sub Instances"), true, new GUIContent("Add New"), delegate (Rect buttonRect) { AddSubInstanceLink(); });

            DrawSubInstanceLink(0, true);

            for (int l = 1; l < m_Ability.subInstanceLinks.Length; l++)
            {
                DrawSubInstanceLink(l);
            }

            EndInspectorGroup();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawPhaseList(int list, bool noButton = false)
        {
            // Cast Rule Area
            if (noButton)
            {
                BeginInspectorGroup(new GUIContent(list + " " + ObjectNames.NicifyVariableName(m_Ability.phaseLists[list].title)));
            }
            else
            {
                BeginInspectorGroup(new GUIContent(list + " " + ObjectNames.NicifyVariableName(m_Ability.phaseLists[list].title)), false, new GUIContent("Delete"), delegate (Rect buttonRect)
                {
                    int index = list;
                    OnRepaint = delegate ()
                    {
                        RemovePhaseListAt(index);
                        OnRepaint = null;
                    };
                    Repaint();
                });
            }

            m_Ability.phaseLists[list].title = EditorGUILayout.TextField(new GUIContent("Title"), m_Ability.phaseLists[list].title);
            m_Ability.phaseLists[list].instant = EditorGUILayout.Toggle(new GUIContent("Instant"), m_Ability.phaseLists[list].instant);

            EndInspectorGroup();
        }

        private void DrawSubInstanceLink(int link, bool noButton = false)
        {
            // Cast Rule Area
            if (noButton)
            {
                BeginInspectorGroup(new GUIContent(link + " " + ObjectNames.NicifyVariableName(m_Ability.subInstanceLinks[link].title)));
            }
            else
            {
                BeginInspectorGroup(new GUIContent(link + " " + ObjectNames.NicifyVariableName(m_Ability.subInstanceLinks[link].title)), false, new GUIContent("Delete"), delegate (Rect buttonRect)
                {
                    int index = link;
                    OnRepaint = delegate ()
                    {
                        RemoveSubInstanceLinkAt(index);
                        OnRepaint = null;
                    };
                    Repaint();
                });
            }

            m_Ability.subInstanceLinks[link].title = EditorGUILayout.TextField(new GUIContent("Title"), m_Ability.subInstanceLinks[link].title);
            m_Ability.subInstanceLinks[link].spawn = (SubInstanceLink.Spawn)EditorGUILayout.EnumPopup(new GUIContent("Instant"), m_Ability.subInstanceLinks[link].spawn);
            m_Ability.subInstanceLinks[link].spawnOffset = EditorGUILayout.Vector3Field(new GUIContent("Spawn Offset"), m_Ability.subInstanceLinks[link].spawnOffset);
            m_Ability.subInstanceLinks[link].obj = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Prefab"), m_Ability.subInstanceLinks[link].obj, typeof(GameObject), false);

            EditorGUILayout.BeginVertical(m_LineBox);
            m_Ability.subInstanceLinks[link].foldout = EditorGUILayout.Foldout(m_Ability.subInstanceLinks[link].foldout, new GUIContent("Components"));
            if (m_Ability.subInstanceLinks[link].foldout)
            {
                Component[] components = m_Ability.subInstanceLinks[link].RegisteredComponents();
                for (int c = 0; c < components.Length; c++)
                {
                    EditorGUILayout.BeginHorizontal(c % 2 == 0 ? m_LineEven : m_LineOdd);
                    GUIContent content = EditorGUIUtility.ObjectContent(components[c], components[c].GetType());
                    content.text = content.text.Replace(components[c].name + " (", "").Replace(")", "");
                    EditorGUILayout.LabelField(new GUIContent(content.image), GUILayout.Width(20f));
                    EditorGUILayout.LabelField(new GUIContent(content.text));
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

            EndInspectorGroup();
        }



        // <>-------------------<Calls>-------------------<>

        private PhaseList AddPhaseList()
        {
            List<PhaseList> lists = new List<PhaseList>(m_Ability.phaseLists);
            PhaseList list = new PhaseList();
            list.title = "New List";
            lists.Add(list);
            m_Ability.phaseLists = lists.ToArray();
            return list;
        }

        private void RemovePhaseListAt(int index)
        {
            List<PhaseList> lists = new List<PhaseList>(m_Ability.phaseLists);
            lists.RemoveAt(index);
            m_Ability.phaseLists = lists.ToArray();
        }

        private SubInstanceLink AddSubInstanceLink()
        {
            List<SubInstanceLink> links = new List<SubInstanceLink>(m_Ability.subInstanceLinks);
            SubInstanceLink link = new SubInstanceLink();
            link.title = "New Link";
            links.Add(link);
            m_Ability.subInstanceLinks = links.ToArray();
            return link;
        }

        private void RemoveSubInstanceLinkAt(int index)
        {
            List<SubInstanceLink> links = new List<SubInstanceLink>(m_Ability.subInstanceLinks);
            links.RemoveAt(index);
            m_Ability.subInstanceLinks = links.ToArray();
        }

        #endregion

        #region Variables Area

        void VariablesArea(Rect rect)
        {
            m_VariablesScroll = EditorGUILayout.BeginScrollView(m_VariablesScroll, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
            GUILayout.Space(5f);
            EditorGUILayout.BeginVertical(GUILayout.Width(rect.width - 14f));

            BeginInspectorGroup(new GUIContent("Shared Variables"), true, new GUIContent("Add New"), SharedVariablesDropdown);
            
            for (int v = 0; v < m_Ability.sharedVariables.Length; v++)
            {
                DrawSharedVariable(v);
            }

            EndInspectorGroup();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawSharedVariable(int variable, bool noButton = false)
        {
            // Cast Rule Area
            if (noButton)
            {
                BeginInspectorGroup(new GUIContent(variable + " " + m_Ability.sharedVariables[variable].GetType().Name + " " + ObjectNames.NicifyVariableName(m_Ability.sharedVariables[variable].title)));
            }
            else
            {
                BeginInspectorGroup(new GUIContent(variable + " " + m_Ability.sharedVariables[variable].GetType().Name + " " + ObjectNames.NicifyVariableName(m_Ability.sharedVariables[variable].title)), false, new GUIContent("Delete"), delegate (Rect buttonRect)
                {
                    int index = variable;
                    OnRepaint = delegate ()
                    {
                        RemoveSharedVariableAt(index);
                        OnRepaint = null;
                    };
                    Repaint();
                });
            }

            EditorGUI.BeginChangeCheck();
            Editor editor = Editor.CreateEditor(m_Ability.sharedVariables[variable]);
            editor.DrawDefaultInspector();
            DestroyImmediate(editor);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_Ability.sharedVariables[variable]);
            }

            EndInspectorGroup();
        }



        // <>-------------------<Calls>-------------------<>

        private void SharedVariablesDropdown(Rect pos)
        {
            List<SharedVariable> variables = new List<SharedVariable>(m_Ability.sharedVariables);
            string[] guids = AssetDatabase.FindAssets("t:MonoScript");
            List<System.Type> types = new List<System.Type>();
            for (int g = 0; g < guids.Length; g++)
            {
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guids[g]));
                System.Type scriptType = script.GetClass();
                if (scriptType != null && scriptType.IsSubclassOf(typeof(SharedVariable)) && !scriptType.IsGenericType)
                {
                    types.Add(scriptType);
                }
            }

            GenericMenu menu = new GenericMenu();
            for (int t = 0; t < types.Count; t++)
            {
                menu.AddItem(new GUIContent(types[t].Name), false, AddSharedVariableOfType, types[t]);
            }
            menu.DropDown(pos);
        }

        private void AddSharedVariableOfType(object obj)
        {
            System.Type type = (System.Type)obj;
            List<SharedVariable> variables = new List<SharedVariable>(m_Ability.sharedVariables);
            SharedVariable variable = (SharedVariable)CreateInstance(type);
            variables.Add(variable);
            m_Ability.sharedVariables = variables.ToArray();
            AssetDatabase.AddObjectToAsset(variable, m_Ability);
        }

        private void RemoveSharedVariableAt(int index)
        {
            List<SharedVariable> variables = new List<SharedVariable>(m_Ability.sharedVariables);
            SharedVariable variable = variables[index];
            variables.RemoveAt(index);
            DestroyImmediate(variable, true);
            m_Ability.sharedVariables = variables.ToArray();
        }

        #endregion

        #region Inspector Area

        void InspectorArea(Rect rect)
        {
            m_InspectorScroll = EditorGUILayout.BeginScrollView(m_InspectorScroll, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
            GUILayout.Space(5f);
            EditorGUILayout.BeginVertical(GUILayout.Width(rect.width - 14f));

            if (m_SelectedPhase != null)
            {
                // TODO: reflective editor
                Editor editor = Editor.CreateEditor(m_SelectedPhase);
                editor.DrawDefaultInspector();
                DestroyImmediate(editor);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        #endregion
    }
}