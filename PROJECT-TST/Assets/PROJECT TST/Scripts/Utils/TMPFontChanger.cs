using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;

public class TMPFontChanger : EditorWindow
{
    private TMP_FontAsset newTMPFont;

    [MenuItem("Tools/Change TMP Font In Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<TMPFontChanger>("Change TMP Font In Prefabs");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select the new TMP Font Asset", EditorStyles.boldLabel);
        newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font", newTMPFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Change TMP Fonts In All Prefabs"))
        {
            ChangeFontsInPrefabs();
        }
    }

    private void ChangeFontsInPrefabs()
    {
        if (newTMPFont == null)
        {
            Debug.LogError("Please assign a TMP Font Asset before running.");
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GameObject instance = PrefabUtility.LoadPrefabContents(path);

            bool modified = false;

            foreach (TextMeshProUGUI tmp in instance.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (tmp.font != newTMPFont)
                {
                    tmp.font = newTMPFont;
                    modified = true;
                }
            }

            if (modified)
            {
                PrefabUtility.SaveAsPrefabAsset(instance, path);
                Debug.Log("Updated TMP font in prefab: " + path);
            }

            PrefabUtility.UnloadPrefabContents(instance);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}