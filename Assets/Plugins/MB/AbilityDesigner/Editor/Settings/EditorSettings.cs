using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Matki.AbilityDesigner.Edit
{
    public class EditorSettings : ScriptableObject
    {

        private const string DATA_NAME = "Editor/Settings";

        #region Singleton

        private static EditorSettings s_CachedSettings;
        internal static EditorSettings INSTANCE
        {
            get
            {
                if (s_CachedSettings != null)
                {
                    return s_CachedSettings;
                }
                s_CachedSettings = Resources.Load<EditorSettings>(Content.DataPath(true) + DATA_NAME);
                if (s_CachedSettings != null)
                {
                    return s_CachedSettings;
                }
                EditorSettings settings = CreateInstance<EditorSettings>();
                string path = Content.DataPath() + DATA_NAME + ".asset";
                AssetDatabase.CreateAsset(settings, path);
                s_CachedSettings = settings;
                return s_CachedSettings;
            }
        }

        #endregion

        #region Skin Settings
        #if UNITY_EDITOR
        [SerializeField]
        private EditorSkin m_EditorSkin;
        internal EditorSkin editorSkin
        {
            get {
                if (m_EditorSkin == null)
                {
                    m_EditorSkin = GetDefaultSkin();
                }
                return m_EditorSkin; }
            set { m_EditorSkin = value; }
        }

        EditorSkin GetDefaultSkin()
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
            return skins[0];
        }
        #endif
        #endregion

        #region Shortcut Settings

        [SerializeField]
        private Shortcut m_GeneralTabShortcut;
        internal Shortcut generalTabShortcut { get { return m_GeneralTabShortcut; } set { m_GeneralTabShortcut = value; } }
        [SerializeField]
        private Shortcut m_StructureTabShortcut;
        internal Shortcut structureTabShortcut { get { return m_StructureTabShortcut; } set { m_StructureTabShortcut = value; } }
        [SerializeField]
        private Shortcut m_VariablesTabShortcut;
        internal Shortcut variablesTabShortcut { get { return m_VariablesTabShortcut; } set { m_VariablesTabShortcut = value; } }
        [SerializeField]
        private Shortcut m_InspectorTabShortcut;
        internal Shortcut inspectorTabShortcut { get { return m_InspectorTabShortcut; } set { m_InspectorTabShortcut = value; } }

        [SerializeField]
        private Shortcut m_MoveLeftShortcut;
        internal Shortcut moveLeftShortcut { get { return m_MoveLeftShortcut; } set { m_MoveLeftShortcut = value; } }
        [SerializeField]
        private Shortcut m_MoveRightShortcut;
        internal Shortcut moveRightShortcut { get { return m_MoveRightShortcut; } set { m_MoveRightShortcut = value; } }
        [SerializeField]
        private Shortcut m_MoveUpShortcut;
        internal Shortcut moveUpShortcut { get { return m_MoveUpShortcut; } set { m_MoveUpShortcut = value; } }
        [SerializeField]
        private Shortcut m_MoveDownShortcut;
        internal Shortcut moveDownShortcut { get { return m_MoveDownShortcut; } set { m_MoveDownShortcut = value; } }

        [SerializeField]
        private Shortcut m_MoveSelectionLeftShortcut;
        internal Shortcut moveSelectionLeftShortcut { get { return m_MoveSelectionLeftShortcut; } set { m_MoveSelectionLeftShortcut = value; } }
        [SerializeField]
        private Shortcut m_MoveSelectionRightShortcut;
        internal Shortcut moveSelectionRightShortcut { get { return m_MoveSelectionRightShortcut; } set { m_MoveSelectionRightShortcut = value; } }
        [SerializeField]
        private Shortcut m_MoveSelectionUpShortcut;
        internal Shortcut moveSelectionUpShortcut { get { return m_MoveSelectionUpShortcut; } set { m_MoveSelectionUpShortcut = value; } }
        [SerializeField]
        private Shortcut m_MoveSelectionDownShortcut;
        internal Shortcut moveSelectionDownShortcut { get { return m_MoveSelectionDownShortcut; } set { m_MoveSelectionDownShortcut = value; } }

        [SerializeField]
        private Shortcut m_DeleteSelectedShortcut;
        internal Shortcut deleteSelectedShortcut { get { return m_DeleteSelectedShortcut; } set { m_DeleteSelectedShortcut = value; } }
        [SerializeField]
        private Shortcut m_DeleteHoveredShortcut;
        internal Shortcut deleteHoveredShortcut { get { return m_DeleteHoveredShortcut; } set { m_DeleteHoveredShortcut = value; } }

        internal Shortcut[] allShortcuts
        {
            get
            {
                return new Shortcut[]
                {
                    generalTabShortcut,
                    structureTabShortcut,
                    variablesTabShortcut,
                    inspectorTabShortcut,
                    moveLeftShortcut,
                    moveRightShortcut,
                    moveUpShortcut,
                    moveDownShortcut,
                    moveSelectionLeftShortcut,
                    moveSelectionRightShortcut,
                    moveSelectionUpShortcut,
                    moveSelectionDownShortcut,
                    deleteSelectedShortcut,
                    deleteHoveredShortcut
                };
            }
        }
        #endregion
    }
}