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

        #region Editor Settings
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
    }
}