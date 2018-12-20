using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Matki.AbilityDesigner.Edit
{
    public class SettingsWindow : EditorWindow
    {
        #region Structure Fields

        private enum Tab { Editor, Pooling, About };
        private Tab m_CurrentlyActive;

        private event System.Action OnRepaint;

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
            m_HeaderTitle.richText = true;

            m_HeaderButton = new GUIStyle("HeaderLabel");
            m_HeaderButton.alignment = TextAnchor.MiddleRight;

            m_SeperationBox = new GUIStyle("GroupBox");
            m_SeperationBox.padding = new RectOffset(5, 5, 5, 5);
            m_SeperationBox.margin = new RectOffset(0, 0, 0, 5);

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
            m_Phase.padding = new RectOffset(5, 5, 5, 5);
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

        [MenuItem("Tools/Ability Designer/Settings")]
        private static void Init()
        {
            SettingsWindow window = CreateInstance<SettingsWindow>();
            window.ShowUtility();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Ability Designer Settings");
            minSize = maxSize = new Vector2(600f, 400f);
            CheckForShortcutErrors();
        }

        private void OnGUI()
        {
            Event e = Event.current;
            if (e.isMouse && e.button == 0 && e.type == EventType.MouseDown)
            {
                GUI.FocusControl("");
            }

            InitStyles();

            // Toolbar Area
            Rect rect = new Rect(0f, 0f, position.width, m_Toolbar.fixedHeight);
            GUILayout.BeginArea(new Rect(0f, 0f, position.width, m_Toolbar.fixedHeight), m_Toolbar);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5f);
            TabArea();
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            // Left Area
            Rect leftRect = new Rect(5f, rect.height + 5f, position.width - 10f, position.height - rect.height - 10f);
            GUILayout.BeginArea(leftRect);
            SelectEditorArea(new Rect(0f, 0f, leftRect.width, leftRect.height));
            GUILayout.EndArea();

            if (OnRepaint != null)
            {
                OnRepaint.Invoke();
            }

            if (e.isMouse && e.button == 0 && e.type == EventType.MouseDown)
            {
                Repaint();
            }
        }

        #region Area Selection

        void TabArea()
        {
            if (GUILayout.Button(new GUIContent("Editor"), m_CurrentlyActive == Tab.Editor ? m_ToolbarButtonSelected : m_ToolbarButton))
            {
                m_CurrentlyActive = Tab.Editor;
                Repaint();
            }
            if (GUILayout.Button(new GUIContent("Pooling"), m_CurrentlyActive == Tab.Pooling ? m_ToolbarButtonSelected : m_ToolbarButton))
            {
                m_CurrentlyActive = Tab.Pooling;
                Repaint();
            }
            if (GUILayout.Button(new GUIContent("About"), m_CurrentlyActive == Tab.About ? m_ToolbarButtonSelected : m_ToolbarButton))
            {
                m_CurrentlyActive = Tab.About;
                Repaint();
            }
        }

        void SelectEditorArea(Rect rect)
        {
            switch (m_CurrentlyActive)
            {
                case Tab.Editor:
                    EditorArea(rect);
                    break;
                case Tab.Pooling:
                    break;
                case Tab.About:
                    break;
            }
        }

        #endregion

        void EditorArea(Rect rect)
        {
            EditorSettings settings = EditorSettings.INSTANCE;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Editor Skin"));
            Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(""), EditorStyles.popup);
            if (GUI.Button(buttonRect, new GUIContent(EditorSettings.INSTANCE.editorSkin.title), EditorStyles.popup))
            {
                EditorSkinsDropdown(buttonRect);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Shortcuts"));
            EditorGUILayout.Space();
            ShortcutField(new GUIContent("General Tab"), settings.generalTabShortcut);
            ShortcutField(new GUIContent("Structure Tab"), settings.structureTabShortcut);
            ShortcutField(new GUIContent("Variables Tab"), settings.variablesTabShortcut);
            ShortcutField(new GUIContent("Inspector Tab"), settings.inspectorTabShortcut);
            EditorGUILayout.Space();
            ShortcutField(new GUIContent("Move Selection Left"), settings.moveSelectionLeftShortcut);
            ShortcutField(new GUIContent("Move Selection Right"), settings.moveSelectionRightShortcut);
            ShortcutField(new GUIContent("Move Selection Up"), settings.moveSelectionUpShortcut);
            ShortcutField(new GUIContent("Move Selection Down"), settings.moveSelectionDownShortcut);
            EditorGUILayout.Space();
            ShortcutField(new GUIContent("Move Left"), settings.moveLeftShortcut);
            ShortcutField(new GUIContent("Move Right"), settings.moveRightShortcut);
            ShortcutField(new GUIContent("Move Up"), settings.moveUpShortcut);
            ShortcutField(new GUIContent("Move Down"), settings.moveDownShortcut);
            EditorGUILayout.Space();
            ShortcutField(new GUIContent("Delete Selected"), settings.deleteSelectedShortcut);
            ShortcutField(new GUIContent("Delete Hovered"), settings.deleteHoveredShortcut);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            if (EditorGUI.EndChangeCheck())
            {
                CheckForShortcutErrors();
                EditorUtility.SetDirty(settings);
                AbilityDesignerWindow.OrderSkinChange();
                Focus();
            }
        }

        void ShortcutField(GUIContent content, Shortcut shortcut)
        {
            GUI.backgroundColor = shortcut.m_Error ? Color.red : Color.white;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent(content));
            shortcut.m_ControlKey = (Shortcut.ControlKey)EditorGUILayout.EnumPopup(shortcut.m_ControlKey);
            shortcut.m_Key = (KeyCode)EditorGUILayout.EnumPopup(shortcut.m_Key);
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
        }

        void CheckForShortcutErrors()
        {
            Shortcut[] shortcuts = EditorSettings.INSTANCE.allShortcuts;
            for (int s = 0; s < shortcuts.Length; s++)
            {
                shortcuts[s].m_Error = false;
            }
            for (int s = 0; s < shortcuts.Length; s++)
            {
                for (int c = s + 1; c < shortcuts.Length; c++)
                {
                    if (shortcuts[s].m_Error)
                    {
                        continue;
                    }
                    bool error = shortcuts[s].Equals(shortcuts[c]);
                    shortcuts[s].SetError(error);
                    if (shortcuts[c].m_Error)
                    {
                        continue;
                    }
                    shortcuts[c].SetError(error);
                }
            }
        }

        void EditorSkinsDropdown(Rect pos)
        {
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
            List<EditorSkin> skins = new List<EditorSkin>();
            for (int g = 0; g < guids.Length; g++)
            {
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guids[g]));
                if (obj != null && obj.GetType() == typeof(EditorSkin))
                {
                    skins.Add((EditorSkin)obj);
                }
            }
            skins.Sort((x, y) => x.orderValue.CompareTo(y.orderValue));

            GenericMenu menu = new GenericMenu();
            for (int t = 0; t < skins.Count; t++)
            {
                EditorSkin skin = skins[t];
                menu.AddItem(new GUIContent(skin.title), EditorSettings.INSTANCE.editorSkin == skin,
                    delegate() {
                        EditorSettings.INSTANCE.editorSkin = skin;
                        EditorUtility.SetDirty(EditorSettings.INSTANCE);
                        AbilityDesignerWindow.OrderSkinChange();
                        Focus();
                    });
            }
            menu.DropDown(pos);
        }
    }
}