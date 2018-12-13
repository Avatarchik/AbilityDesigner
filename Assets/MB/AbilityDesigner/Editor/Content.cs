using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Matki.AbilityDesigner.Edit
{
    public class Content : MonoBehaviour
    {

        private const string RESOUCES_PATH = "AbilityDesigner/";
        private const string DATA_PATH = "Data/";
        private const string DARK_SKIN_PATH = "DarkSkin/";
        private const string BRIGHT_SKIN_PATH = "DarkSkin/";
        private const string ICONS_PATH = "Icons/";
        private const string ICONS_PATH_DARK = ICONS_PATH + DARK_SKIN_PATH;
        private const string ICONS_PATH_BRIGHT = ICONS_PATH + BRIGHT_SKIN_PATH;
        private const string META_PATH = "Meta/";
        private const string XML_PATH = META_PATH + "ABILITYDESIGNER_VERSION";

        private const string SKIN_ALIAS = "{SkinColor}";

        internal const string ASSET_TAG = "asset";
        internal const string DESCRIPTION_TAG = "description";
        internal const string VERSION_TAG = "version";
        internal const string AUTHOR_TAG = "author";
        internal const string YEAR_TAG = "year";
        internal const string URL_TAG = "url";
        internal const string DOC_TAG = "doc";
        internal const string LOGO_TAG = "logo";
        internal const string INSTRUCTION_TAG = "instruction";

        [System.NonSerialized]
        private static string s_RootPath = null;
        internal static string ROOT_PATH
        {
            get
            {
                if (s_RootPath != null)
                {
                    return s_RootPath;
                }
                string[] guids = AssetDatabase.FindAssets("t:MonoScript");
                for (int g = 0; g < guids.Length; g++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[g]);
                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                    if (script.GetClass() == typeof(AbilityDesignerResourceRoot))
                    {
                        s_RootPath = path.Replace("AbilityDesignerResourceRoot.cs", "");
                    }
                }
                return s_RootPath;
            }
        }

        internal static string GetPath(string varPath)
        {
            varPath = varPath.Replace(SKIN_ALIAS, EditorGUIUtility.isProSkin ? DARK_SKIN_PATH : BRIGHT_SKIN_PATH);
            return varPath;
        }

        internal static string GetXML()
        {
            TextAsset textAsset = Resources.Load<TextAsset>(RESOUCES_PATH + XML_PATH);
            return textAsset.text;
        }

        internal static string GetXMLTag(string tag)
        {
            return GetXML().Expose(tag);
        }

        internal static string DataPath(bool resource = false)
        {
            if (resource)
            {
                return RESOUCES_PATH + DATA_PATH;
            }
            else
            {
                return ROOT_PATH + DATA_PATH;
            }
        }
    }
}