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

        private const float PHASELIST_WIDTH = 300f;
        private const float PHASELIST_YSPACING = 10f;
        private const float PHASELIST_XSPACING = 20f;

        #region Structure Fields

        private enum Tab { General, Structure, Variables, Inspector };
        private Tab m_CurrentlyActive;

        private event System.Action OnRepaint;

        private Vector2 m_GeneralScroll;
        private Vector2 m_StructureScroll;
        private Vector2 m_VariablesScroll;
        private Vector2 m_InspectorScroll;
        
        private float m_EditorHeight;
        private Vector2 m_EditorScroll;

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
        GUIStyle m_LineText;
        GUIStyle m_DropArea;
        GUIStyle m_Phase, m_PhaseSelected;
        GUIStyle m_PhaseTitle, m_ListTitle;
        GUIStyle m_AddButton;

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

            m_LineText = new GUIStyle(EditorStyles.label);
            m_LineText.alignment = TextAnchor.MiddleLeft;

            m_DropArea = new GUIStyle(EditorStyles.helpBox);
            m_DropArea.alignment = TextAnchor.MiddleCenter;
            m_DropArea.fontStyle = FontStyle.Bold;
            m_DropArea.fontSize = 12;

            m_Phase = new GUIStyle("flow node 0");
            m_Phase.fixedWidth = 0f;
            m_Phase.fixedHeight = 0f;
            m_Phase.padding = new RectOffset(5,5,5,5);
            m_Phase.margin = new RectOffset(0, 0, 0, 0);
            m_Phase.alignment = TextAnchor.MiddleCenter;

            m_PhaseSelected = new GUIStyle("flow node 0 on");
            m_PhaseSelected.fixedWidth = 0f;
            m_PhaseSelected.fixedHeight = 0f;
            m_PhaseSelected.padding = new RectOffset(5, 5, 5, 5);
            m_PhaseSelected.margin = new RectOffset(0, 0, 0, 0);
            m_PhaseSelected.alignment = TextAnchor.MiddleCenter;

            m_AddButton = new GUIStyle("NotificationBackground");
            m_AddButton.fixedHeight = 0f;
            m_AddButton.fixedWidth = 0f;
            m_AddButton.fontSize = 12;
            m_AddButton.padding = new RectOffset(10, 10, 10, 10);
            m_AddButton.margin = new RectOffset(0, 0, 0, 0);
            m_AddButton.fontStyle = FontStyle.Bold;

            m_PhaseTitle = new GUIStyle(EditorStyles.largeLabel);
            m_PhaseTitle.alignment = TextAnchor.MiddleCenter;
            m_PhaseTitle.fontSize = 12;
            m_PhaseTitle.fontStyle = FontStyle.Bold;

            m_ListTitle = new GUIStyle(EditorStyles.largeLabel);
            m_ListTitle.alignment = TextAnchor.MiddleCenter;
            m_ListTitle.fontSize = 14;
            m_ListTitle.fontStyle = FontStyle.Bold;

            m_StylesInitialized = true;
        }

        #endregion

        #region Helper Methods

        private bool BeginInspectorGroupFoldable(GUIContent content, bool foldout, bool parent = false, GUIContent buttonInfo = null, System.Action<Rect> onButtonCall = null)
        {
            GUILayout.BeginVertical();
            GUI.backgroundColor = parent ? PARENT_COLOR : Color.white;
            GUILayout.BeginHorizontal(GUILayout.Height(25f));
            GUILayout.BeginHorizontal(m_Header, GUILayout.Height(25f));
            EditorGUILayout.LabelField(content, m_HeaderTitle, GUILayout.ExpandHeight(true));
            Rect rect = GUILayoutUtility.GetLastRect();
            if (GUI.Button(rect, GUIContent.none, m_HeaderTitle))
            {
                foldout = !foldout;
            }
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
            return foldout;
        }

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

        private static T[] DropAreaGUILayout<T>(GUIContent content, bool multiObject, GUIStyle style, params GUILayoutOption[] options) where T : Object
        {
            Rect rect = GUILayoutUtility.GetRect(content, style, options);
            EditorGUI.LabelField(rect, content, style);
            return DropAreaGUI<T>(rect, multiObject);
        }

        private static Object[] DropAreaGUILayout(GUIContent content, bool multiObject, System.Type validType, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(content, style, options);
            EditorGUI.LabelField(rect, content, style);
            return DropAreaGUI(rect, multiObject, validType);
        }

        private static T[] DropAreaGUI<T>(Rect rect, bool multiObject) where T : Object
        {
            Object[] objects = DropAreaGUI(rect, multiObject, typeof(T));
            if (objects == null)
            {
                return null;
            }
            T[] results = new T[objects.Length];
            for (int o = 0; o < objects.Length; o++)
            {
                results[o] = (T)objects[o];
            }
            return results;
        }

        private static Object[] DropAreaGUI(Rect rect, bool multiObject, System.Type validType)
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!rect.Contains(evt.mousePosition))
                        return null;

                    if ((!multiObject ? DragAndDrop.objectReferences.Length == 1 : true) && AreOfType(validType, DragAndDrop.objectReferences))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            return DragAndDrop.objectReferences;
                        }
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }

                    break;
            }
            return null;
        }

        private static bool AreOfType(System.Type type, Object[] objects)
        {
            for (int o = 0; o < objects.Length; o++)
            {
                if (objects[o].GetType() == type)
                {
                    return true;
                }
            }
            return false;
        }
        
        private static void DrawLine(Vector2 a, Vector2 b, float contrastValue = 0.9f)
        {
            // Start with skin: free color
            Color lineColor = new Color(1f - contrastValue, 1f - contrastValue, 1f - contrastValue);

            // If pro skin is enabled change Color acordingly
            if (EditorGUIUtility.isProSkin)
            {
                lineColor = new Color(contrastValue, contrastValue, contrastValue);
            }

            // Draw the line
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = lineColor;
            Handles.DrawLine(a, b);
            Handles.color = oldColor;
            Handles.EndGUI();
        }

        private static void DrawLineThick(Vector2 a, Vector2 b, Color color, float width = 1f)
        {
            // Draw the line
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = color;
            Handles.DrawBezier(a, b, a, b, color, null, width);
            Handles.color = oldColor;
            Handles.EndGUI();
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
            OnSelectionChange();
        }

        private void OnFocus()
        {
            OnSelectionChange();
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

            m_EditorHeight = GetMaxHeight();
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
            EditorToolbar();
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
            Rect edtiorRect = new Rect(INSPECTOR_WIDTH, rect.height, position.width - INSPECTOR_WIDTH, position.height - rect.height);
            GUILayout.BeginArea(edtiorRect, m_PhaseBackground);
            EditorArea(new Rect(0f, 0f, edtiorRect.width, edtiorRect.height));
            GUILayout.EndArea();

            if (OnRepaint != null)
            {
                OnRepaint.Invoke();
            }
        }

        #endregion

        // <>-------------------<Left Area>-------------------<>

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
                m_Ability.subInstanceLinks[link].foldout = BeginInspectorGroupFoldable(new GUIContent(link + " " + ObjectNames.NicifyVariableName(m_Ability.subInstanceLinks[link].title)), m_Ability.subInstanceLinks[link].foldout);
            }
            else
            {
                m_Ability.subInstanceLinks[link].foldout = BeginInspectorGroupFoldable(new GUIContent(link + " " + ObjectNames.NicifyVariableName(m_Ability.subInstanceLinks[link].title)), m_Ability.subInstanceLinks[link].foldout,
                    false, new GUIContent("Delete"), delegate (Rect buttonRect)
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

            if (m_Ability.subInstanceLinks[link].foldout)
            {
                m_Ability.subInstanceLinks[link].title = EditorGUILayout.TextField(new GUIContent("Title"), m_Ability.subInstanceLinks[link].title);
                EditorGUILayout.BeginVertical(m_LineBox);
                EditorGUILayout.LabelField(new GUIContent("Spawn"), EditorStyles.boldLabel);
                m_Ability.subInstanceLinks[link].spawn = (SubInstanceLink.Spawn)EditorGUILayout.EnumPopup(new GUIContent("Instant"), m_Ability.subInstanceLinks[link].spawn);
                m_Ability.subInstanceLinks[link].spawnOffset = EditorGUILayout.Vector3Field(new GUIContent("Offset"), m_Ability.subInstanceLinks[link].spawnOffset);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical(m_LineBox);
            EditorGUILayout.BeginHorizontal(m_LineOdd);
            GUIContent goContent = EditorGUIUtility.ObjectContent(m_Ability.subInstanceLinks[link].obj, typeof(GameObject));
            EditorGUILayout.LabelField(new GUIContent(goContent.image), GUILayout.Width(20f));
            EditorGUILayout.LabelField(new GUIContent(goContent.text), EditorStyles.boldLabel);
            Rect titleRect = GUILayoutUtility.GetLastRect();
            m_Ability.subInstanceLinks[link].obj = (GameObject)EditorGUI.ObjectField(titleRect, m_Ability.subInstanceLinks[link].obj, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();
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
            SubInstanceLink link = CreateInstance<SubInstanceLink>();
            link.title = "New Link";
            links.Add(link);
            m_Ability.subInstanceLinks = links.ToArray();
            AssetDatabase.AddObjectToAsset(link, m_Ability);
            return link;
        }

        private void RemoveSubInstanceLinkAt(int index)
        {
            List<SubInstanceLink> links = new List<SubInstanceLink>(m_Ability.subInstanceLinks);
            SubInstanceLink link = links[index];
            links.RemoveAt(index);
            DestroyImmediate(link, true);
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
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5f);
            EditorGUILayout.BeginVertical(GUILayout.Width(rect.width - 24f));

            if (m_SelectedPhase != null)
            {
                // TODO: reflective editor
                Editor editor = Editor.CreateEditor(m_SelectedPhase);
                editor.DrawDefaultInspector();
                DestroyImmediate(editor);
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(5f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        #endregion

        // <>-------------------<Right Area>-------------------<>

        #region Right Area

        void EditorToolbar()
        {
            if (GUILayout.Button(new GUIContent(m_Ability.name), m_CurrentlyActive == Tab.General ? m_ToolbarButtonSelected : m_ToolbarButton, GUILayout.Width(100f)))
            {
                Repaint();
            }
            if (GUILayout.Button(new GUIContent("Select"), m_CurrentlyActive == Tab.General ? m_ToolbarButtonSelected : m_ToolbarButton, GUILayout.Width(100f)))
            {
                Repaint();
            }
        }

        #endregion

        #region Editor Area

        void EditorArea(Rect rect)
        {
            Event e = Event.current;
            if (e.isMouse && e.button == 0 && e.type == EventType.MouseDown)
            {
                if (rect.Contains(e.mousePosition))
                {
                    m_SelectedPhase = null;
                }
            }

            float viewWidth = 20f + (PHASELIST_WIDTH + PHASELIST_XSPACING) * m_Ability.phaseLists.Length;
            m_EditorScroll = GUI.BeginScrollView(rect, m_EditorScroll, new Rect(0f, 0f, viewWidth, m_EditorHeight + 40f), true, true);

            EditorGUI.BeginChangeCheck();

            for (int l = 0; l < m_Ability.phaseLists.Length; l++)
            {
                float listHeight = GetListHeight(l);
                Rect listRect = new Rect((PHASELIST_WIDTH + PHASELIST_XSPACING) * l, 20f, PHASELIST_WIDTH + PHASELIST_XSPACING * 2f, listHeight);
                GUILayout.BeginArea(listRect);
                DrawPhaseList(new Rect(PHASELIST_XSPACING, 20f, listRect.width - PHASELIST_XSPACING * 2f, listRect.height), l);
                GUILayout.EndArea();
            }

            if (EditorGUI.EndChangeCheck())
            {
                m_EditorHeight = GetMaxHeight();
            }

            GUI.EndScrollView();

            if (e.isMouse && e.button == 0 && e.type == EventType.MouseDown)
            {
                if (rect.Contains(e.mousePosition))
                {
                    Repaint();
                }
            }
        }

        void DrawPhaseList(Rect rect, int list)
        {
            float width = rect.width;

            Vector2 startPos = new Vector2(PHASELIST_XSPACING + rect.width / 2f, 0f);
            Vector2 endPos = new Vector2(PHASELIST_XSPACING + rect.width / 2f, rect.height - 40f);
            DrawLineThick(startPos, endPos, new Color(0.1f, 0.1f, 0.1f, 0.8f), 4f);

            GUILayout.BeginArea(new Rect(PHASELIST_XSPACING, 0f, width, 40f), m_Phase);
            EditorGUILayout.LabelField(new GUIContent(list + " " + m_Ability.phaseLists[list].title), m_ListTitle, GUILayout.ExpandHeight(true));
            GUILayout.EndArea();

            float height = 40f + PHASELIST_YSPACING;
            for (int p = 0; p < m_Ability.phaseLists[list].phases.Length; p++)
            {
                float phaseHeight = GetPhaseHeight(list, p);
                Phases.Phase currentPhase = m_Ability.phaseLists[list].phases[p];
                GUILayout.BeginArea(new Rect(PHASELIST_XSPACING, height, width, phaseHeight), m_SelectedPhase == currentPhase ? m_PhaseSelected : m_Phase);

                EditorGUI.DrawTextureTransparent(new Rect(width / 2f - 25f, 10f, 50f, 50f), EditorGUIUtility.whiteTexture);

                // Title
                string title = ObjectNames.NicifyVariableName(currentPhase.GetType().Name);
                string customTitle = currentPhase.customTitle;
                title = string.IsNullOrEmpty(customTitle) ? title : customTitle;
                EditorGUI.LabelField(new Rect(0f, 60f, width, 20f), new GUIContent(title), m_PhaseTitle);

                // Sub Instance Links
                float linksHeight = (currentPhase.runForSubInstances.Length + 1) * 20f;
                float linksWidth = width - 20f;
                GUILayout.BeginArea(new Rect(10f, 80f, linksWidth, linksHeight), m_LineBox);
                GUILayout.BeginArea(new Rect(1f, 1f, linksWidth - 2f, 18f), m_LineOdd);
                EditorGUI.LabelField(new Rect(0f, 0f, linksWidth - 60f, 18f), new GUIContent("Run for Sub Instances"), m_LineText);
                Rect setRect = new Rect(linksWidth - 51f, 0f, 50f, 17.5f);
                if (GUI.Button(setRect, new GUIContent("Set")))
                {
                    LinksDropdown(setRect, list, p);
                }
                GUILayout.EndArea();
                for (int l = 0; l < currentPhase.runForSubInstances.Length; l++)
                {
                    GUILayout.BeginArea(new Rect(1f, 19f + l * 20f, linksWidth - 2f, 20f), l % 2 == 0 ? m_LineEven : m_LineOdd);
                    EditorGUI.LabelField(new Rect(0f, 0f, linksWidth - 2f, 18f), new GUIContent(currentPhase.runForSubInstances[l].title), m_LineText);
                    GUILayout.EndArea();
                }
                GUILayout.EndArea();

                // Events
                Event e = Event.current;
                Rect mouseRect = new Rect(0f, 0f, width, phaseHeight);
                if (e.isMouse && e.button == 0 && e.type == EventType.MouseDown)
                {
                    if (mouseRect.Contains(e.mousePosition))
                    {
                        if (m_SelectedPhase == currentPhase)
                        {
                            m_SelectedPhase = null;
                            Repaint();
                        }
                        else
                        {
                            m_SelectedPhase = currentPhase;
                            Repaint();
                        }
                    }
                }

                if (e.isKey && e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
                {
                    if (m_SelectedPhase == currentPhase)
                    {
                        int listIndex = list;
                        int phaseIndex = p;
                        m_SelectedPhase = null;
                        OnRepaint = delegate ()
                        {
                            RemovePhaseAt(listIndex, phaseIndex);
                            OnRepaint = null;
                        };
                        Repaint();
                    }
                }

                GUILayout.EndArea();
                height += phaseHeight + PHASELIST_YSPACING;
            }

            Rect buttonRect = new Rect(PHASELIST_XSPACING + rect.width / 2f - 50f, rect.height - 40f, 100f, 40f);
            if (GUI.Button(buttonRect, new GUIContent("Add"), m_AddButton))
            {
                PhasesDropdown(buttonRect, list);
            }
        }



        // <>-------------------<Calls>-------------------<>

        void LinksDropdown(Rect pos, int list, int phase)
        {
            int indexPhase = phase;
            int indexList = list;
            List<SubInstanceLink> links = new List<SubInstanceLink>(m_Ability.subInstanceLinks);
            List<SubInstanceLink> activeLinks = new List<SubInstanceLink>(m_Ability.phaseLists[list].phases[phase].runForSubInstances);
            GenericMenu menu = new GenericMenu();
            for (int l = 0; l < links.Count; l++)
            {
                SubInstanceLink link = links[l];
                bool active = activeLinks.Contains(links[l]);
                menu.AddItem(new GUIContent(l + " " + links[l].title), active, delegate ()
                {
                    if (active)
                    {
                        RemoveLinkFromPhase(indexList, indexPhase, link);
                    }
                    else
                    {
                        AddLinkToPhase(indexList, indexPhase, link);
                    }
                });
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("All"), links.Count == activeLinks.Count, delegate ()
            {
                m_Ability.phaseLists[indexList].phases[indexPhase].runForSubInstances = links.ToArray();
            });
            menu.AddItem(new GUIContent("Clear"), activeLinks.Count == 0, delegate ()
            {
                m_Ability.phaseLists[indexList].phases[indexPhase].runForSubInstances = new SubInstanceLink[0];
            });
            menu.DropDown(pos);
        }

        private void AddLinkToPhase(int list, int phase, SubInstanceLink link)
        {
            List<SubInstanceLink> links = new List<SubInstanceLink>(m_Ability.phaseLists[list].phases[phase].runForSubInstances);
            links.Add(link);
            m_Ability.phaseLists[list].phases[phase].runForSubInstances = links.ToArray();
        }

        private void RemoveLinkFromPhase(int list, int phase, SubInstanceLink link)
        {
            List<SubInstanceLink> links = new List<SubInstanceLink>(m_Ability.phaseLists[list].phases[phase].runForSubInstances);
            links.Remove(link);
            m_Ability.phaseLists[list].phases[phase].runForSubInstances = links.ToArray();
        }

        void PhasesDropdown(Rect pos, int list)
        {
            string[] guids = AssetDatabase.FindAssets("t:MonoScript");
            List<System.Type> types = new List<System.Type>();
            for (int g = 0; g < guids.Length; g++)
            {
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guids[g]));
                System.Type scriptType = script.GetClass();
                if (scriptType != null && scriptType.IsSubclassOf(typeof(Phases.Phase)) && !scriptType.IsAbstract)
                {
                    types.Add(scriptType);
                }
            }

            GenericMenu menu = new GenericMenu();
            for (int t = 0; t < types.Count; t++)
            {
                System.Type type = types[t];
                int index = list;
                menu.AddItem(new GUIContent(types[t].Name), false, delegate() { AddPhaseOfType(type, index); });
            }
            menu.DropDown(pos);
        }

        private void AddPhaseOfType(System.Type type, int list)
        {
            List<Phases.Phase> phases = new List<Phases.Phase>(m_Ability.phaseLists[list].phases);
            Phases.Phase phase = (Phases.Phase)CreateInstance(type);
            phases.Add(phase);
            m_Ability.phaseLists[list].phases = phases.ToArray();
            AssetDatabase.AddObjectToAsset(phase, m_Ability);
            m_EditorHeight = GetMaxHeight();
        }

        private void RemovePhaseAt(int list, int index)
        {
            List<Phases.Phase> phases = new List<Phases.Phase>(m_Ability.phaseLists[list].phases);
            Phases.Phase phase = phases[index];
            phases.RemoveAt(index);
            DestroyImmediate(phase, true);
            m_Ability.phaseLists[list].phases = phases.ToArray();
            m_EditorHeight = GetMaxHeight();
        }



        // <>-------------------<Helper>-------------------<>

        float GetMaxHeight()
        {
            float maxHeight = 0f;
            for (int l = 0; l < m_Ability.phaseLists.Length; l++)
            {
                float listHeight = GetListHeight(l);
                if (listHeight > maxHeight)
                {
                    maxHeight = listHeight;
                }
            }
            return maxHeight;
        }

        float GetListHeight(int list)
        {
            float height = 80f + PHASELIST_YSPACING * 2f;
            for (int p = 0; p < m_Ability.phaseLists[list].phases.Length; p++)
            {
                height += GetPhaseHeight(list, p);
                if (p < m_Ability.phaseLists[list].phases.Length - 1)
                {
                    height += PHASELIST_YSPACING;
                }
            }
            return height;
        }

        float GetPhaseHeight(int list, int phase)
        {
            float height = 90f;
            height += (m_Ability.phaseLists[list].phases[phase].runForSubInstances.Length + 1) * 20f;
            return height;
        }

        #endregion
    }
}