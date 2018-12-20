using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.VersionControl;

namespace Matki.AbilityDesigner.Edit
{
    public class AbilityDesignerWindow : EditorWindow
    {
        private static Color PARENT_COLOR = new Color(0.7f, 0.7f, 0.7f);

        #region Skin Redirects

        private static float INSPECTOR_WIDTH
        {
            get
            {
                return EditorSettings.INSTANCE.editorSkin.inspectorWidth;
            }
        }

        private static float PHASELIST_WIDTH
        {
            get
            {
                return EditorSettings.INSTANCE.editorSkin.phaseListWidth;
            }
        }
        private static Vector2 PHASELIST_SPACING
        {
            get
            {
                return EditorSettings.INSTANCE.editorSkin.phaseListSpacing;
            }
        }

        private static string SUBINSTANCELINK_COLOR
        {
            get
            {
                return "<color=#" + ColorUtility.ToHtmlStringRGB(EditorSettings.INSTANCE.editorSkin.subInstanceLink_Color) + ">";
            }
        }
        private static string PHASELIST_COLOR
        {
            get
            {
                return "<color=#" + ColorUtility.ToHtmlStringRGB(EditorSettings.INSTANCE.editorSkin.phaseList_Color) + ">";
            }
        }
        private static string SHAREDVARIABLE_COLOR
        {
            get
            {
                return "<color=#" + ColorUtility.ToHtmlStringRGB(EditorSettings.INSTANCE.editorSkin.sharedVariable_Color) + ">";
            }
        }

        private static float PHASETITLE_HEIGHT
        {
            get
            {
                return EditorSettings.INSTANCE.editorSkin.phaseTitleHeight;
            }
        }
        private static float PHASE_SPACING
        {
            get
            {
                return EditorSettings.INSTANCE.editorSkin.phaseSpacing;
            }
        }
        private static EditorSkin.PhaseIconLayout PHASEICON_LAYOUT
        {
            get
            {
                return EditorSettings.INSTANCE.editorSkin.phaseIconLayout;
            }
        }

        #endregion

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
        private Phases.PhaseCore m_SelectedPhase;

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

        GUIStyle m_SharedVariablePopup, m_SharedVariableToggle;

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
            m_HeaderTitle.richText = true;

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
            m_LineText.richText = true;

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

            m_SharedVariablePopup = new GUIStyle("ToolbarPopup");// "Popup");
            m_SharedVariablePopup.padding = new RectOffset(8, 8, 0, 0);
            m_SharedVariablePopup.richText = true;

            m_SharedVariableToggle = new GUIStyle("toolbarbutton");// "Radio");
            m_SharedVariableToggle.alignment = TextAnchor.MiddleCenter;
            m_SharedVariableToggle.padding = new RectOffset(0, 0, 0, 0);
            m_SharedVariableToggle.richText = true;
            m_SharedVariableToggle.fixedWidth = 0f;

            m_StylesInitialized = true;
        }

        #endregion

        #region Helper Methods

        private bool BeginInspectorGroupFoldable(GUIContent content, bool foldout, bool parent = false, GUIContent buttonInfo = null, System.Action<Rect> onButtonCall = null)
        {
            GUILayout.BeginVertical();
            GUI.backgroundColor = parent ? PARENT_COLOR : Color.white;
            GUILayout.BeginHorizontal(GUILayout.Height(25f));
            GUILayout.BeginHorizontal(m_Header, GUILayout.Height(25f), GUILayout.ExpandWidth(true));
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
            GUILayout.BeginHorizontal(m_Header, GUILayout.Height(25f), GUILayout.ExpandWidth(true));
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

        private void OnLostFocus()
        {
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

            m_SelectedPhase = null;
            m_EditorHeight = GetMaxHeight();
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            Event e = Event.current;
            if (e.isMouse && e.button == 0 && e.type == EventType.MouseDown)
            {
                GUI.FocusControl("");
            }

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

            if (EditorGUI.EndChangeCheck())
            {
                if (Provider.isActive && Provider.enabled)
                {
                    if (!Provider.IsOpenForEdit(Provider.GetAssetByPath(AssetDatabase.GetAssetPath(m_Ability))))
                    {
                        Provider.Checkout(m_Ability, CheckoutMode.Asset);
                    }
                }
            }

            if (OnRepaint != null)
            {
                OnRepaint.Invoke();
            }
            
            if (e.isMouse && e.button == 0 && e.type == EventType.MouseDown)
            {
                Repaint();
            }
        }

        internal static void OrderSkinChange()
        {
            AbilityDesignerWindow window = GetWindow<AbilityDesignerWindow>();
            window.Focus();
            window.m_StylesInitialized = false;
            window.Repaint();
        }

        #endregion

        // <>-------------------<Left Area>-------------------<>

        #region Left Area

        void TabArea()
        {
            Event e = Event.current;
            if (GUILayout.Button(new GUIContent("General"), m_CurrentlyActive == Tab.General ? m_ToolbarButtonSelected : m_ToolbarButton)
                || EditorSettings.INSTANCE.generalTabShortcut.Triggered(e))
            {
                OnRepaint = delegate ()
                {
                    m_CurrentlyActive = Tab.General;
                    OnRepaint = null;
                };
                Repaint();
            }
            if (GUILayout.Button(new GUIContent("Structure"), m_CurrentlyActive == Tab.Structure ? m_ToolbarButtonSelected : m_ToolbarButton)
                || EditorSettings.INSTANCE.structureTabShortcut.Triggered(e))
            {
                OnRepaint = delegate ()
                {
                    m_CurrentlyActive = Tab.Structure;
                    OnRepaint = null;
                };
                Repaint();
            }
            if (GUILayout.Button(new GUIContent("Variables"), m_CurrentlyActive == Tab.Variables ? m_ToolbarButtonSelected : m_ToolbarButton)
                || EditorSettings.INSTANCE.variablesTabShortcut.Triggered(e))
            {
                OnRepaint = delegate ()
                {
                    m_CurrentlyActive = Tab.Variables;
                    OnRepaint = null;
                };
                Repaint();
            }
            if (GUILayout.Button(new GUIContent("Inspector"), m_CurrentlyActive == Tab.Inspector ? m_ToolbarButtonSelected : m_ToolbarButton)
                || EditorSettings.INSTANCE.inspectorTabShortcut.Triggered(e))
            {
                OnRepaint = delegate ()
                {
                    m_CurrentlyActive = Tab.Inspector;
                    OnRepaint = null;
                };
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
            PhaseList currentList = m_Ability.phaseLists[list];
            // Cast Rule Area
            if (noButton)
            {
                BeginInspectorGroup(new GUIContent(PHASELIST_COLOR + "[" + currentList.id + "]</color> " + ObjectNames.NicifyVariableName(currentList.title)));
            }
            else
            {
                BeginInspectorGroup(new GUIContent(PHASELIST_COLOR + "[" + currentList.id + "]</color> " + ObjectNames.NicifyVariableName(currentList.title)), false, new GUIContent("Delete"), delegate (Rect buttonRect)
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
            SubInstanceLink currentLink = m_Ability.subInstanceLinks[link];

            // Cast Rule Area
            if (noButton)
            {
                m_Ability.subInstanceLinks[link].foldout = BeginInspectorGroupFoldable(new GUIContent(SUBINSTANCELINK_COLOR + "[" + currentLink.id + "]</color> " + ObjectNames.NicifyVariableName(currentLink.title)), m_Ability.subInstanceLinks[link].foldout);
            }
            else
            {
                m_Ability.subInstanceLinks[link].foldout = BeginInspectorGroupFoldable(new GUIContent(SUBINSTANCELINK_COLOR + "[" + currentLink.id + "]</color> " + ObjectNames.NicifyVariableName(currentLink.title)), m_Ability.subInstanceLinks[link].foldout,
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
            int id = 0;
            for (int l = 0; l < lists.Count; l++)
            {
                if (lists[l].id == id)
                {
                    id++;
                }
            }
            PhaseList list = new PhaseList(id);
            list.title = "New List";
            lists.Add(list);
            lists.Sort((x, y) => x.id.CompareTo(y.id));
            m_Ability.phaseLists = lists.ToArray();
            return list;
        }

        private void RemovePhaseListAt(int index)
        {
            List<PhaseList> lists = new List<PhaseList>(m_Ability.phaseLists);
            lists[index].Destroy();
            lists.RemoveAt(index);
            m_Ability.phaseLists = lists.ToArray();
        }

        private SubInstanceLink AddSubInstanceLink()
        {
            List<SubInstanceLink> links = new List<SubInstanceLink>(m_Ability.subInstanceLinks);
            int id = 0;
            for (int l = 0; l < links.Count; l++)
            {
                if (links[l].id == id)
                {
                    id++;
                }
            }
            SubInstanceLink link = SubInstanceLink.CreateInstance(id);
            link.title = "New Link";
            links.Add(link);
            links.Sort((x, y) => x.id.CompareTo(y.id));
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
            CheckForEmptySubInstanceLinkReferences();
        }

        private void CheckForEmptySubInstanceLinkReferences()
        {
            List<SubInstanceLink> links = new List<SubInstanceLink>(m_Ability.subInstanceLinks);
            for (int l = 0; l < m_Ability.phaseLists.Length; l++)
            {
                for (int p = 0; p < m_Ability.phaseLists[l].phases.Length; p++)
                {
                    List<SubInstanceLink> localLinks = new List<SubInstanceLink>(m_Ability.phaseLists[l].phases[p].runForSubInstances);
                    for (int ll = localLinks.Count - 1; ll >= 0; ll--)
                    {
                        if (!links.Contains(localLinks[ll]))
                        {
                            localLinks.RemoveAt(ll);
                        }
                    }
                    m_Ability.phaseLists[l].phases[p].runForSubInstances = localLinks.ToArray();
                }
            }
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
            SharedVariable currentVariable = m_Ability.sharedVariables[variable];
            // Cast Rule Area
            if (noButton)
            {
                BeginInspectorGroup(new GUIContent(SHAREDVARIABLE_COLOR + "[" + currentVariable.id + "]</color> " + currentVariable.GetType().Name +
                    " " + ObjectNames.NicifyVariableName(m_Ability.sharedVariables[variable].title)));
            }
            else
            {
                BeginInspectorGroup(new GUIContent(SHAREDVARIABLE_COLOR + "[" + currentVariable.id + "]</color> " + currentVariable.GetType().Name +
                    " " + ObjectNames.NicifyVariableName(m_Ability.sharedVariables[variable].title)), false, new GUIContent("Delete"), delegate (Rect buttonRect)
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
            int id = 0;
            for (int l = 0; l < variables.Count; l++)
            {
                if (variables[l].id == id)
                {
                    id++;
                }
            }
            SharedVariable variable = SharedVariable.CreateInstance(type, id);
            variables.Add(variable);
            variables.Sort((x, y) => x.id.CompareTo(y.id));
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
                ReflectiveEditor(m_SelectedPhase);
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(5f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        [System.NonSerialized]
        Phases.PhaseCore m_CachedPhase;
        [System.NonSerialized]
        FieldInfo[] m_CachedFields;

        void ReflectiveEditor(Phases.PhaseCore target)
        {
            if (target != m_CachedPhase)
            {
                m_CachedPhase = target;
                FieldInfo[]  cachedFields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                List<FieldInfo> cachedFieldList = new List<FieldInfo>();
                for (int f = 0; f < cachedFields.Length; f++)
                {
                    SerializeField attribute = cachedFields[f].GetCustomAttribute<SerializeField>(false);
                    if (attribute != null)
                    {
                        cachedFieldList.Add(cachedFields[f]);
                        continue;
                    }
                    if (cachedFields[f].IsPublic)
                    {
                        cachedFieldList.Add(cachedFields[f]);
                        continue;
                    }
                }
                m_CachedFields = cachedFieldList.ToArray();
            }

            GUILayout.BeginVertical(EditorStyles.helpBox);
            target.customTitle = EditorGUILayout.TextField(new GUIContent("Custom Title"), target.customTitle);
            target.customColor = EditorGUILayout.ColorField(new GUIContent("Custom Color"), target.customColor);
            target.breakOnFail = EditorGUILayout.Toggle(new GUIContent("Break On Fail"), target.breakOnFail);
            GUILayout.EndVertical();

            SerializedObject serializedObject = new SerializedObject(target);
            
            for (int f = 0; f < m_CachedFields.Length; f++)
            {
                if (m_CachedFields[f].FieldType.IsSubclassOf(typeof(SharedVariable)))
                {
                    DrawSharedVariableProperty(m_CachedFields[f]);
                    continue;
                }
                if (m_CachedFields[f].FieldType.IsEquivalentTo(typeof(PhaseListLink)))
                {
                    DrawPhaseListLinkProperty(m_CachedFields[f]);
                    continue;
                }
                if (IsSubclassOfRawGeneric(typeof(SubVariable<>), m_CachedFields[f].FieldType))
                {
                    DrawSubVariableProperty(m_CachedFields[f]);
                    continue;
                }

                // Drau default property
                SerializedProperty property = serializedObject.FindProperty(m_CachedFields[f].Name);
                EditorGUILayout.PropertyField(property);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private bool IsSubclassOfRawGeneric(System.Type generic, System.Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                System.Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        private void HandleAttributes(FieldInfo info)
        {
            HeaderAttribute headerAttribute = info.GetCustomAttribute<HeaderAttribute>(false);
            if (headerAttribute != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(new GUIContent(headerAttribute.header), EditorStyles.boldLabel);
            }
        }

        private void DrawSubVariableProperty(FieldInfo info)
        {
            List<SubInstanceLink> usedLinks = new List<SubInstanceLink>(m_SelectedPhase.runForSubInstances);

            EditorGUILayout.BeginVertical(m_LineBox);

            System.Type[] genTypes = info.FieldType.BaseType.GenericTypeArguments;
            object value = info.GetValue(m_SelectedPhase);

            MethodInfo prepareMethod = value.GetType().GetMethod("Prepare", BindingFlags.Instance | BindingFlags.NonPublic);
            prepareMethod.Invoke(value, new object[] { usedLinks.ToArray() });

            MethodInfo getMethod = value.GetType().GetMethod("GetValue", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo setMethod = value.GetType().GetMethod("SetValue", BindingFlags.Instance | BindingFlags.NonPublic);

            EditorGUILayout.BeginHorizontal(m_LineOdd);
            EditorGUILayout.LabelField(new GUIContent(ObjectNames.NicifyVariableName(info.Name) + " for:"), m_LineText);
            EditorGUILayout.EndHorizontal();

            for (int l = 0; l < m_SelectedPhase.runForSubInstances.Length; l++)
            {
                SubInstanceLink currentLink = m_SelectedPhase.runForSubInstances[l];
                EditorGUILayout.BeginHorizontal(l % 2 == 0 ? m_LineEven : m_LineOdd);
                EditorGUILayout.LabelField(new GUIContent(SUBINSTANCELINK_COLOR + "[" + currentLink.id + "]</color> " + ObjectNames.NicifyVariableName(currentLink.title)), m_LineText);
                object result = DrawFieldAuto(getMethod.Invoke(value, new object[] { m_SelectedPhase.runForSubInstances[l] }));
                setMethod.Invoke(value, new object[] { m_SelectedPhase.runForSubInstances[l], result });
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private object DrawFieldAuto(object value)
        {
            if (value.GetType().IsEquivalentTo(typeof(int)))
            {
                return EditorGUILayout.IntField((int)value);
            }
            if (value.GetType().IsEquivalentTo(typeof(float)))
            {
                return EditorGUILayout.FloatField((float)value);
            }
            if (value.GetType().IsEquivalentTo(typeof(string)))
            {
                return EditorGUILayout.TextField((string)value);
            }
            if (value.GetType().IsEquivalentTo(typeof(bool)))
            {
                return EditorGUILayout.Toggle((bool)value);
            }
            if (value.GetType().IsEquivalentTo(typeof(double)))
            {
                return EditorGUILayout.DoubleField((double)value);
            }
            if (value.GetType().IsSubclassOf(typeof(Object)))
            {
                return EditorGUILayout.ObjectField((Object)value, value.GetType(), false);
            }
            if (value.GetType().IsEnum)
            {
                return EditorGUILayout.EnumPopup((System.Enum)value);
            }
            return null;
        }

        private void DrawSharedVariableProperty(FieldInfo info)
        {
            HandleAttributes(info);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(ObjectNames.NicifyVariableName(info.Name));

            bool linked = false;
            SharedVariable variable = (SharedVariable)info.GetValue(m_CachedPhase);
            List<SharedVariable> variables = new List<SharedVariable>(m_Ability.sharedVariables);
            if (variable == null || variables.Contains(variable))
            {
                linked = true;
            }

            linked = true;

            if (linked)
            {
                GUIContent content = new GUIContent(variable == null ? "none" : (SHAREDVARIABLE_COLOR + "<b>[" + variable.id + "]</b></color> " + variable.title));
                Rect rect = GUILayoutUtility.GetRect(content, m_SharedVariablePopup);
                GUI.backgroundColor = variable == null ? Color.red : Color.white;
                if (GUI.Button(rect, content, m_SharedVariablePopup))
                {
                    SharedVariableDropdown(rect, info, m_CachedPhase);
                }
                GUI.backgroundColor = Color.white;
            }
            else
            {
                SerializedObject serializedObject = new SerializedObject(variable);
                SerializedProperty property = serializedObject.FindProperty("m_Value");
                EditorGUILayout.PropertyField(property, GUIContent.none);
                serializedObject.ApplyModifiedProperties();
            }
            /*
            if (GUILayout.Button(new GUIContent("▶"), m_SharedVariableToggle, GUILayout.Width(20f)))
            {
                if (linked)
                {
                    SharedVariable newVariable = SharedVariable.CreateInstance(info.FieldType, -1);
                    AssetDatabase.AddObjectToAsset(newVariable, m_Ability);
                    info.SetValue(m_CachedPhase, newVariable);
                }
                else
                {
                    DestroyImmediate(variable, true);
                    info.SetValue(m_CachedPhase, null);
                }
            }*/
            GUILayout.EndHorizontal();
        }

        public void SharedVariableDropdown(Rect pos, FieldInfo value, Phases.PhaseCore target)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("(none)"), value.GetValue(target) == null, delegate () { value.SetValue(target, null); });
            for (int s = 0; s < m_Ability.sharedVariables.Length; s++)
            {
                if (m_Ability.sharedVariables[s].GetType() == value.FieldType)
                {
                    SharedVariable variable = m_Ability.sharedVariables[s];
                    SharedVariable targetVar = (SharedVariable)value.GetValue(target);
                    menu.AddItem(new GUIContent("[" + variable.id + "] " + variable.title), targetVar == variable, delegate () { value.SetValue(target, variable); });
                }
            }
            menu.DropDown(pos);
        }

        private void DrawPhaseListLinkProperty(FieldInfo info)
        {
            HandleAttributes(info);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(ObjectNames.NicifyVariableName(info.Name));
            
            PhaseListLink variable = (PhaseListLink)info.GetValue(m_CachedPhase);
            if (variable == null)
            {
                variable = new PhaseListLink();
                variable.id = -1;
            }

            List<PhaseList> variables = new List<PhaseList>(m_Ability.phaseLists);
            PhaseList foundList = variables.Find(value => value.id == variable.id);
            if (foundList == null)
            {
                variable.id = -1;
            }


            GUIContent content = new GUIContent(variable.id == -1 ? "none" : (PHASELIST_COLOR + "<b>[" + variable.id + "]</b></color> " + foundList.title));
            Rect rect = GUILayoutUtility.GetRect(content, m_SharedVariablePopup);
            GUI.backgroundColor = variable.id == -1 ? Color.red : Color.white;
            if (GUI.Button(rect, content, m_SharedVariablePopup))
            {
                PhaseListLinkDropdown(rect, info, m_CachedPhase);
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.EndHorizontal();
        }

        public void PhaseListLinkDropdown(Rect pos, FieldInfo value, Phases.PhaseCore target)
        {
            GenericMenu menu = new GenericMenu();
            PhaseListLink variable = (PhaseListLink)value.GetValue(m_CachedPhase);
            menu.AddItem(new GUIContent("(none)"), variable.id == -1, delegate () { variable.id = -1; });
            for (int l = 0; l < m_Ability.phaseLists.Length; l++)
            {
                PhaseList foundList = m_Ability.phaseLists[l];
                menu.AddItem(new GUIContent("[" + foundList.id + "] " + foundList.title), variable.id == foundList.id, delegate () { variable.id = foundList.id; });
            }
            menu.DropDown(pos);
        }


        #endregion

        // <>-------------------<Right Area>-------------------<>

        #region Right Area

        void EditorToolbar()
        {
            if (GUILayout.Button(new GUIContent(m_Ability.name), m_ToolbarButton, GUILayout.Width(100f)))
            {
                Repaint();
            }
            if (GUILayout.Button(new GUIContent("Select"), m_ToolbarButton, GUILayout.Width(100f)))
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

            float viewWidth = 20f + (PHASELIST_WIDTH + PHASELIST_SPACING.x) * m_Ability.phaseLists.Length;
            m_EditorScroll = GUI.BeginScrollView(rect, m_EditorScroll, new Rect(0f, 0f, viewWidth, m_EditorHeight + 40f), true, true);

            EditorGUI.BeginChangeCheck();

            for (int l = 0; l < m_Ability.phaseLists.Length; l++)
            {
                float listHeight = GetListHeight(l);
                Rect listRect = new Rect((PHASELIST_WIDTH + PHASELIST_SPACING.x) * l, 20f, PHASELIST_WIDTH + PHASELIST_SPACING.x * 2f, listHeight);
                GUILayout.BeginArea(listRect);
                DrawPhaseList(new Rect(PHASELIST_SPACING.x, 20f, listRect.width - PHASELIST_SPACING.x * 2f, listRect.height), l);
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

            Vector2 startPos = new Vector2(PHASELIST_SPACING.x + rect.width / 2f, 0f);
            Vector2 endPos = new Vector2(PHASELIST_SPACING.x + rect.width / 2f, rect.height - 40f);
            DrawLineThick(startPos, endPos, new Color(0.1f, 0.1f, 0.1f, 0.8f), 4f);

            GUILayout.BeginArea(new Rect(PHASELIST_SPACING.x, 0f, width, 40f), m_Phase);
            EditorGUILayout.LabelField(new GUIContent(list + " " + m_Ability.phaseLists[list].title), m_ListTitle, GUILayout.ExpandHeight(true));
            GUILayout.EndArea();

            float height = 40f + PHASELIST_SPACING.y;
            for (int p = 0; p < m_Ability.phaseLists[list].phases.Length; p++)
            {
                float phaseHeight = GetPhaseHeight(list, p);

                Rect phaseRect = new Rect(PHASELIST_SPACING.x, height, width, phaseHeight);
                PhaseArea(phaseRect, list, p);

                height += phaseHeight + PHASELIST_SPACING.y;
            }

            Rect buttonRect = new Rect(PHASELIST_SPACING.x + rect.width / 2f - 50f, rect.height - 40f, 100f, 40f);
            if (GUI.Button(buttonRect, new GUIContent("Add"), m_AddButton))
            {
                PhasesDropdown(buttonRect, list);
            }
        }

        private void PhaseArea(Rect rect, int list, int phase)
        {
            Phases.PhaseCore currentPhase = m_Ability.phaseLists[list].phases[phase];

            GUILayout.BeginArea(rect, m_SelectedPhase == currentPhase ? m_PhaseSelected : m_Phase);
            GUILayout.EndArea();
            Color backgroundColor = currentPhase.customColor;
            backgroundColor.a = 1f;
            GUI.backgroundColor = backgroundColor;
            GUILayout.BeginArea(rect, m_Phase);
            GUI.backgroundColor = Color.white;

            int listIndex = list;
            int phaseIndex = phase;

            // Events
            Event e = Event.current;
            Rect mouseRect = new Rect(0f, 0f, rect.width, rect.height);
            if (e.isMouse && e.button == 0 && e.type == EventType.MouseDown)
            {
                if (mouseRect.Contains(e.mousePosition))
                {
                    m_SelectedPhase = currentPhase;
                    Repaint();
                }
            }
            HandlePhaseContextMenu(mouseRect, list, phase);
            HandlePhaseShortcuts(mouseRect, list, phase);

            Rect iconRect = rect;
            Rect titleRect = rect;
            float iconHeight = PHASETITLE_HEIGHT;
            switch (PHASEICON_LAYOUT)
            {
                case EditorSkin.PhaseIconLayout.Centered:
                    iconHeight = PHASETITLE_HEIGHT - 30f;
                    iconRect = new Rect(rect.width / 2f - iconHeight / 2f, PHASE_SPACING, iconHeight, iconHeight);
                    titleRect = new Rect(0f, iconHeight + PHASE_SPACING * 2f, rect.width, 20f);
                    break;
                case EditorSkin.PhaseIconLayout.Flat:
                    iconRect = new Rect(PHASE_SPACING, PHASE_SPACING, PHASETITLE_HEIGHT, PHASETITLE_HEIGHT);
                    titleRect = new Rect(PHASETITLE_HEIGHT + PHASE_SPACING, PHASE_SPACING, rect.width - PHASETITLE_HEIGHT - PHASE_SPACING * 2f, PHASETITLE_HEIGHT);
                    break;
                case EditorSkin.PhaseIconLayout.NoIcon:
                    iconRect = new Rect(PHASE_SPACING, PHASE_SPACING, 0f, 0f);
                    titleRect = new Rect(PHASE_SPACING, PHASE_SPACING, rect.width - PHASE_SPACING * 2f, PHASETITLE_HEIGHT);
                    break;
            }

            // Icon
            PhaseIconAttribute iconAttribute = m_Ability.phaseLists[list].phases[phase].GetType().GetCustomAttribute<PhaseIconAttribute>(true);
            if (iconAttribute != null)
            {
                Texture iconTexture = Resources.Load<Texture>(Content.GetResourcesPath(iconAttribute.path));
                GUI.DrawTexture(iconRect, iconTexture);
            }

            // Title
            string title = ObjectNames.NicifyVariableName(currentPhase.GetType().Name);
            string customTitle = currentPhase.customTitle;
            title = string.IsNullOrEmpty(customTitle) ? title : customTitle;
            EditorGUI.LabelField(titleRect, new GUIContent(title), m_PhaseTitle);

            DefaultSubInstanceLinkOnlyAttribute subInstancesDeactive = m_Ability.phaseLists[list].phases[phase].GetType().GetCustomAttribute<DefaultSubInstanceLinkOnlyAttribute>(true);
            if (subInstancesDeactive == null)
            {
                // Sub Instance Links
                float linksHeight = (currentPhase.runForSubInstances.Length + 1) * 20f;
                float linksWidth = rect.width - PHASE_SPACING * 2f;
                GUILayout.BeginArea(new Rect(PHASE_SPACING, PHASETITLE_HEIGHT + PHASE_SPACING * 2f, linksWidth, linksHeight), m_LineBox);
                GUILayout.BeginArea(new Rect(1f, 1f, linksWidth - 2f, 18f), m_LineOdd);
                EditorGUI.LabelField(new Rect(0f, 0f, linksWidth - 60f, 18f), new GUIContent("Run for Sub Instances"), m_LineText);
                Rect setRect = new Rect(linksWidth - 51f, -1f, 50f, 18.5f);
                if (GUI.Button(setRect, new GUIContent("Set")))
                {
                    LinksDropdown(setRect, list, phase);
                }
                GUILayout.EndArea();
                for (int l = 0; l < currentPhase.runForSubInstances.Length; l++)
                {
                    GUILayout.BeginArea(new Rect(1f, 19f + l * 20f, linksWidth - 2f, 20f), l % 2 == 0 ? m_LineEven : m_LineOdd);
                    EditorGUI.LabelField(new Rect(0f, 0f, linksWidth - 2f, 18f), new GUIContent(SUBINSTANCELINK_COLOR + "<b>[" + currentPhase.runForSubInstances[l].id + "]</b></color> " +
                        ObjectNames.NicifyVariableName(currentPhase.runForSubInstances[l].title)), m_LineText);
                    GUILayout.EndArea();
                }
                GUILayout.EndArea();
            }
            else
            {
                m_Ability.phaseLists[list].phases[phase].runForSubInstances = new SubInstanceLink[] { m_Ability.subInstanceLinks[0] };
            }

            GUILayout.EndArea();
        }



        // <>-------------------<Calls>-------------------<>

        void HandlePhaseContextMenu(Rect rect, int list, int phase)
        {
            EditorSettings settings = EditorSettings.INSTANCE;

            Phases.PhaseCore currentPhase = m_Ability.phaseLists[list].phases[phase];
            Event e = Event.current;

            if (e.isMouse && e.button == 1 && e.type == EventType.MouseDown)
            {
                if (rect.Contains(e.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    Phases.PhaseCore selection = currentPhase;
                    if (currentPhase == m_SelectedPhase)
                    {
                        menu.AddItem(new GUIContent("Deselect"), false, delegate ()
                        {
                            m_SelectedPhase = null;
                            Repaint();
                        });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Select"), false, delegate ()
                        {
                            m_SelectedPhase = currentPhase;
                            Repaint();
                        });
                    }
                    if (m_SelectedPhase != null && m_SelectedPhase != currentPhase)
                    {
                        menu.AddItem(new GUIContent("Move Selected Above"), false, delegate () { MoveSelectedPhaseTo(list, phase); });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Move Selected Above"), false, null);
                    }
                    if (m_SelectedPhase != null && m_SelectedPhase != currentPhase && phase < m_Ability.phaseLists[list].phases.Length)
                    {
                        menu.AddItem(new GUIContent("Move Selected Below"), false, delegate () { MoveSelectedPhaseTo(list, phase + 1); });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Move Selected Below"), false, null);
                    }
                    menu.AddSeparator("");
                    InsertPhasesDropdown(menu, "Insert Above", list, phase);
                    InsertPhasesDropdown(menu, "Insert Below", list, phase + 1);
                    menu.AddSeparator("");
                    if (phase > 0)
                    {
                        menu.AddItem(new GUIContent("Move Up\t" + settings.moveUpShortcut.ToString()), false, delegate () { MovePhaseUp(list, phase); });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Move Up\t" + settings.moveUpShortcut.ToString()), false, null);
                    }
                    if (phase < m_Ability.phaseLists[list].phases.Length - 1)
                    {
                        menu.AddItem(new GUIContent("Move Down\t" + settings.moveDownShortcut.ToString()), false, delegate () { MovePhaseDown(list, phase); });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Move Down\t" + settings.moveDownShortcut.ToString()), false, null);
                    }
                    if (list > 0)
                    {
                        menu.AddItem(new GUIContent("Move Left\t" + settings.moveLeftShortcut.ToString()), false, delegate () { MovePhaseLeft(list, phase); });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Move Left\t" + settings.moveLeftShortcut.ToString()), false, null);
                    }
                    if (list < m_Ability.phaseLists.Length - 1)
                    {
                        menu.AddItem(new GUIContent("Move Right\t" + settings.moveRightShortcut.ToString()), false, delegate () { MovePhaseRight(list, phase); });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Move Right\t" + settings.moveRightShortcut.ToString()), false, null);
                    }
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Remove\t" + (m_SelectedPhase == currentPhase ? settings.deleteSelectedShortcut.ToString() : settings.deleteHoveredShortcut.ToString())),
                        false, delegate () { RemovePhaseAt(list, phase); Repaint(); });
                    menu.ShowAsContext();
                }
            }
        }

        void HandlePhaseShortcuts(Rect rect, int list, int phase)
        {
            EditorSettings settings = EditorSettings.INSTANCE;
            Phases.PhaseCore currentPhase = m_Ability.phaseLists[list].phases[phase];
            Event e = Event.current;


            if (e.isKey && e.type == EventType.KeyDown)
            {
                // Shortcuts for hovered node
                if (rect.Contains(e.mousePosition))
                {
                    if (settings.deleteHoveredShortcut.Triggered(e))
                    {
                        OnRepaint = delegate ()
                        {
                            RemovePhaseAt(list, phase, m_SelectedPhase == currentPhase);
                            OnRepaint = null;
                        };
                        Repaint();
                    }
                }
                if (m_SelectedPhase == currentPhase)
                {
                    // Shortcuts for selected node
                    if (settings.deleteSelectedShortcut.Triggered(e))
                    {
                        OnRepaint = delegate ()
                        {
                            RemovePhaseAt(list, phase);
                            OnRepaint = null;
                        };
                        Repaint();
                    }
                    else if (settings.moveUpShortcut.Triggered(e))
                    {
                        OnRepaint = delegate ()
                        {
                            MovePhaseUp(list, phase);
                            OnRepaint = null;
                        };
                        Repaint();
                    }
                    else if (settings.moveDownShortcut.Triggered(e))
                    {
                        OnRepaint = delegate ()
                        {
                            MovePhaseDown(list, phase);
                            OnRepaint = null;
                        };
                        Repaint();
                    }
                    else if (settings.moveLeftShortcut.Triggered(e))
                    {
                        OnRepaint = delegate ()
                        {
                            MovePhaseLeft(list, phase);
                            OnRepaint = null;
                        };
                        Repaint();
                    }
                    else if (settings.moveRightShortcut.Triggered(e))
                    {
                        OnRepaint = delegate ()
                        {
                            MovePhaseRight(list, phase);
                            OnRepaint = null;
                        };
                        Repaint();
                    }
                    else if (settings.moveSelectionUpShortcut.Triggered(e))
                    {
                        OnRepaint = delegate ()
                        {
                            MoveSelectionUp(list, phase);
                            OnRepaint = null;
                        };
                        Repaint();
                    }
                    else if (settings.moveSelectionDownShortcut.Triggered(e))
                    {
                        OnRepaint = delegate ()
                        {
                            MoveSelectionDown(list, phase);
                            OnRepaint = null;
                        };
                        Repaint();
                    }
                    else if (settings.moveSelectionLeftShortcut.Triggered(e))
                    {
                        OnRepaint = delegate ()
                        {
                            MoveSelectionLeft(list, phase);
                            OnRepaint = null;
                        };
                        Repaint();
                    }
                    else if (settings.moveSelectionRightShortcut.Triggered(e))
                    {
                        OnRepaint = delegate ()
                        {
                            MoveSelectionRight(list, phase);
                            OnRepaint = null;
                        };
                        Repaint();
                    }
                }
            }
        }

        void MoveSelectedPhaseTo(int list, int phase)
        {
            if (m_SelectedPhase == null)
            {
                return;
            }

            int selectedList = -1;
            int selectedPhase = -1;
            for (int l = 0; l < m_Ability.phaseLists.Length; l++)
            {
                for (int p = 0; p < m_Ability.phaseLists[l].phases.Length; p++)
                {
                    if (m_Ability.phaseLists[l].phases[p] == m_SelectedPhase)
                    {
                        selectedList = l;
                        selectedPhase = p;
                    }
                }
            }

            if (selectedPhase == -1 || selectedList == -1)
            {
                return;
            }

            MovePhaseTo(selectedList, selectedPhase, list, phase);
        }

        void MovePhaseTo(int inputList, int inputPhase, int outputList, int outputPhase)
        {
            if (inputList != outputList)
            {
                Phases.PhaseCore currentPhase = m_Ability.phaseLists[inputList].phases[inputPhase];
                List<Phases.PhaseCore> tempInputList = new List<Phases.PhaseCore>(m_Ability.phaseLists[inputList].phases);
                List<Phases.PhaseCore> tempOutputList = new List<Phases.PhaseCore>(m_Ability.phaseLists[outputList].phases);
                tempInputList.RemoveAt(inputPhase);
                tempOutputList.Insert(outputPhase, currentPhase);
                m_Ability.phaseLists[inputList].phases = tempInputList.ToArray();
                m_Ability.phaseLists[outputList].phases = tempOutputList.ToArray();
            }
            else
            {
                if (inputPhase < outputPhase)
                {
                    Phases.PhaseCore currentPhase = m_Ability.phaseLists[inputList].phases[inputPhase];
                    List<Phases.PhaseCore> tempList = new List<Phases.PhaseCore>(m_Ability.phaseLists[inputList].phases);
                    tempList.RemoveAt(inputPhase);
                    tempList.Insert(outputPhase - 1, currentPhase);
                    m_Ability.phaseLists[inputList].phases = tempList.ToArray();
                }
                else
                {
                    Phases.PhaseCore currentPhase = m_Ability.phaseLists[inputList].phases[inputPhase];
                    List<Phases.PhaseCore> tempList = new List<Phases.PhaseCore>(m_Ability.phaseLists[inputList].phases);
                    tempList.RemoveAt(inputPhase);
                    tempList.Insert(outputPhase, currentPhase);
                    m_Ability.phaseLists[inputList].phases = tempList.ToArray();
                }
            }
        }

        void MovePhaseUp(int list, int phase)
        {
            if (phase <= 0)
            {
                return;
            }
            Phases.PhaseCore currentPhase = m_Ability.phaseLists[list].phases[phase];
            List<Phases.PhaseCore> tempList = new List<Phases.PhaseCore>(m_Ability.phaseLists[list].phases);
            tempList.RemoveAt(phase);
            tempList.Insert(phase - 1, currentPhase);
            m_Ability.phaseLists[list].phases = tempList.ToArray();
        }

        void MoveSelectionUp(int list, int phase)
        {
            if (phase <= 0)
            {
                return;
            }
            m_SelectedPhase = m_Ability.phaseLists[list].phases[phase - 1];
        }

        void MovePhaseDown(int list, int phase)
        {
            if (phase >= m_Ability.phaseLists[list].phases.Length - 1)
            {
                return;
            }
            Phases.PhaseCore currentPhase = m_Ability.phaseLists[list].phases[phase];
            List<Phases.PhaseCore> tempList = new List<Phases.PhaseCore>(m_Ability.phaseLists[list].phases);
            tempList.RemoveAt(phase);
            tempList.Insert(phase + 1, currentPhase);
            m_Ability.phaseLists[list].phases = tempList.ToArray();
        }

        void MoveSelectionDown(int list, int phase)
        {
            if (phase >= m_Ability.phaseLists[list].phases.Length - 1)
            {
                return;
            }
            m_SelectedPhase = m_Ability.phaseLists[list].phases[phase + 1];
        }

        void MovePhaseLeft(int list, int phase)
        {
            if (list <= 0)
            {
                return;
            }
            Phases.PhaseCore currentPhase = m_Ability.phaseLists[list].phases[phase];

            int targetIndex = Mathf.Clamp(phase, 0, m_Ability.phaseLists[list - 1].phases.Length - 1);
            if (targetIndex < 0)
            {
                targetIndex = 0;
            }
            List<Phases.PhaseCore> tempOriginList = new List<Phases.PhaseCore>(m_Ability.phaseLists[list].phases);
            List<Phases.PhaseCore> tempTargetList = new List<Phases.PhaseCore>(m_Ability.phaseLists[list - 1].phases);
            tempOriginList.RemoveAt(phase);
            tempTargetList.Insert(targetIndex, currentPhase);
            m_Ability.phaseLists[list].phases = tempOriginList.ToArray();
            m_Ability.phaseLists[list - 1].phases = tempTargetList.ToArray();
        }

        void MoveSelectionLeft(int list, int phase)
        {
            if (list <= 0)
            {
                return;
            }
            int targetListIndex = list;
            for (int l = list - 1; l >= 0; l--)
            {
                if (targetListIndex == list && m_Ability.phaseLists[l].phases.Length > 0)
                {
                    targetListIndex = l;
                }
            }
            phase = Mathf.Clamp(phase, 0, m_Ability.phaseLists[targetListIndex].phases.Length - 1);
            m_SelectedPhase = m_Ability.phaseLists[targetListIndex].phases[phase];
        }

        void MovePhaseRight(int list, int phase)
        {
            if (list >= m_Ability.phaseLists.Length - 1)
            {
                return;
            }
            Phases.PhaseCore currentPhase = m_Ability.phaseLists[list].phases[phase];

            int targetIndex = Mathf.Clamp(phase, 0, m_Ability.phaseLists[list + 1].phases.Length - 1);
            if (targetIndex < 0)
            {
                targetIndex = 0;
            }
            List<Phases.PhaseCore> tempOriginList = new List<Phases.PhaseCore>(m_Ability.phaseLists[list].phases);
            List<Phases.PhaseCore> tempTargetList = new List<Phases.PhaseCore>(m_Ability.phaseLists[list + 1].phases);
            tempOriginList.RemoveAt(phase);
            tempTargetList.Insert(targetIndex, currentPhase);
            m_Ability.phaseLists[list].phases = tempOriginList.ToArray();
            m_Ability.phaseLists[list + 1].phases = tempTargetList.ToArray();
        }

        void MoveSelectionRight(int list, int phase)
        {
            if (list >= m_Ability.phaseLists.Length - 1)
            {
                return;
            }
            int targetListIndex = list;
            for (int l = list + 1; l < m_Ability.phaseLists.Length; l++)
            {
                if (targetListIndex == list && m_Ability.phaseLists[l].phases.Length > 0)
                {
                    targetListIndex = l;
                }
            }
            phase = Mathf.Clamp(phase, 0, m_Ability.phaseLists[targetListIndex].phases.Length - 1);
            m_SelectedPhase = m_Ability.phaseLists[targetListIndex].phases[phase];
        }

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
                menu.AddItem(new GUIContent("[" + links[l].id + "] " + links[l].title), active, delegate ()
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
            links.Sort((x, y) => x.id.CompareTo(y.id));
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
            SortedDictionary<string, System.Type> types = new SortedDictionary<string, System.Type>();
            for (int g = 0; g < guids.Length; g++)
            {
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guids[g]));
                System.Type scriptType = script.GetClass();
                if (scriptType != null && scriptType.IsSubclassOf(typeof(Phases.PhaseCore)) && !scriptType.IsAbstract)
                {
                    string menuName = scriptType.Name;
                    PhaseCategoryAttribute category = scriptType.GetCustomAttribute<PhaseCategoryAttribute>(true);
                    if (category != null)
                    {
                        menuName = category.path + "/" + menuName;
                    }

                    types.Add(menuName, scriptType);
                }
            }

            GenericMenu menu = new GenericMenu();
            foreach (KeyValuePair<string, System.Type> pair in types)
            {
                int index = list;
                
                menu.AddItem(new GUIContent(pair.Key), false, delegate() { AddPhaseOfType(pair.Value, index); });
            }
            menu.DropDown(pos);
        }

        void InsertPhasesDropdown(GenericMenu menu, string path, int list, int phase)
        {
            string[] guids = AssetDatabase.FindAssets("t:MonoScript");
            SortedDictionary<string, System.Type> types = new SortedDictionary<string, System.Type>();
            for (int g = 0; g < guids.Length; g++)
            {
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guids[g]));
                System.Type scriptType = script.GetClass();
                if (scriptType != null && scriptType.IsSubclassOf(typeof(Phases.PhaseCore)) && !scriptType.IsAbstract)
                {
                    string menuName = scriptType.Name;
                    PhaseCategoryAttribute category = scriptType.GetCustomAttribute<PhaseCategoryAttribute>(true);
                    if (category != null)
                    {
                        menuName = category.path + "/" + menuName;
                    }

                    types.Add(menuName, scriptType);
                }
            }
            
            foreach (KeyValuePair<string, System.Type> pair in types)
            {
                int index = list;

                menu.AddItem(new GUIContent(path + "/" + pair.Key), false, delegate () { InsertPhaseOfType(pair.Value, index, phase); });
            }
        }

        private void InsertPhaseOfType(System.Type type, int list, int index)
        {
            List<Phases.PhaseCore> phases = new List<Phases.PhaseCore>(m_Ability.phaseLists[list].phases);
            Phases.PhaseCore phase = (Phases.PhaseCore)CreateInstance(type);
            phases.Insert(index, phase);
            m_Ability.phaseLists[list].phases = phases.ToArray();
            AssetDatabase.AddObjectToAsset(phase, m_Ability);
            m_EditorHeight = GetMaxHeight();
        }

        private void AddPhaseOfType(System.Type type, int list)
        {
            List<Phases.PhaseCore> phases = new List<Phases.PhaseCore>(m_Ability.phaseLists[list].phases);
            Phases.PhaseCore phase = (Phases.PhaseCore)CreateInstance(type);
            phases.Add(phase);
            m_Ability.phaseLists[list].phases = phases.ToArray();
            AssetDatabase.AddObjectToAsset(phase, m_Ability);
            m_EditorHeight = GetMaxHeight();
        }

        private void RemovePhaseAt(int list, int index, bool selectionChange = true)
        {
            List<Phases.PhaseCore> phases = new List<Phases.PhaseCore>(m_Ability.phaseLists[list].phases);
            Phases.PhaseCore phase = phases[index];
            bool autoselect = m_SelectedPhase == phase;
            phases.RemoveAt(index);
            DestroyImmediate(phase, true);
            m_Ability.phaseLists[list].phases = phases.ToArray();
            m_EditorHeight = GetMaxHeight();

            if (selectionChange)
            {
                m_SelectedPhase = null;
            }
            if (autoselect)
            {
                index = Mathf.Clamp(index, 0, m_Ability.phaseLists[list].phases.Length - 1);
                if (index >= 0)
                {
                    m_SelectedPhase = m_Ability.phaseLists[list].phases[index];
                }
            }
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
            float height = 80f + PHASELIST_SPACING.y * 2f;
            for (int p = 0; p < m_Ability.phaseLists[list].phases.Length; p++)
            {
                height += GetPhaseHeight(list, p);
                if (p <= m_Ability.phaseLists[list].phases.Length - 1)
                {
                    height += PHASELIST_SPACING.y;
                }
            }
            return height;
        }

        float GetPhaseHeight(int list, int phase)
        {
            DefaultSubInstanceLinkOnlyAttribute subInstancesDeactive = m_Ability.phaseLists[list].phases[phase].GetType().GetCustomAttribute<DefaultSubInstanceLinkOnlyAttribute>(true);
            if (subInstancesDeactive == null)
            {
                float height = PHASETITLE_HEIGHT + PHASE_SPACING * 3f;
                height += (m_Ability.phaseLists[list].phases[phase].runForSubInstances.Length + 1) * 20f;
                return height;
            }
            else
            {
                return PHASETITLE_HEIGHT + PHASE_SPACING * 2f;
            }
        }

        #endregion
    }
}