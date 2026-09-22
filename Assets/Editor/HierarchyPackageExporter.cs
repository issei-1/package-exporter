#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public sealed class HierarchyPackageExporter : EditorWindow
{
    [MenuItem("Tools/Hierarchy Package Exporter")]
    private static void Open()
    {
        GetWindow<HierarchyPackageExporter>("Package Export");
    }

    private void OnSelectionChange() { Repaint(); }

    private void OnGUI()
    {
        var selected = Selection.activeGameObject;
        bool valid = Selection.gameObjects.Length == 1 && selected != null
            && !EditorUtility.IsPersistent(selected) && selected.scene.IsValid()
            && !EditorApplication.isPlayingOrWillChangePlaymode;

        EditorGUILayout.LabelField("対象", valid ? selected.name : "Hierarchyで1つ選択してください");
        EditorGUILayout.HelpBox("子オブジェクトと依存アセットを含め、指定した保存先へ出力します。", MessageType.Info);
        using (new EditorGUI.DisabledScope(!valid))
        {
            if (GUILayout.Button(".unitypackage を書き出す", GUILayout.Height(32)))
                Export(selected);
        }
    }

    private static void Export(GameObject source)
    {
        string prefabPath = null;
        GameObject copy = null;
        string packagePath = null;
        string stagingPath = null;
        try
        {
            string name = source.name;
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            name = name.Trim().TrimEnd('.');
            if (string.IsNullOrEmpty(name)) name = "GameObject";

            packagePath = EditorUtility.SaveFilePanel("unitypackage の保存先",
                Path.GetDirectoryName(Application.dataPath), name, "unitypackage");
            if (string.IsNullOrEmpty(packagePath)) return;

            string exportId = Guid.NewGuid().ToString("N");
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(
                "Assets/HierarchyExport_" + name + "_" + exportId + ".prefab");

            // Clone first so existing scene objects and prefab connections stay intact.
            copy = Instantiate(source);
            copy.name = source.name;
            if (PrefabUtility.IsPartOfPrefabInstance(copy))
                PrefabUtility.UnpackPrefabInstance(copy, PrefabUnpackMode.Completely,
                    InteractionMode.AutomatedAction);

            bool saved;
            PrefabUtility.SaveAsPrefabAsset(copy, prefabPath, out saved);
            if (!saved) throw new IOException("Prefabの保存に失敗しました。");

            stagingPath = Path.Combine(Path.GetDirectoryName(packagePath),
                ".HierarchyExport_" + exportId + ".unitypackage");
            // Synchronous export: temporary assets must exist until it finishes.
            AssetDatabase.ExportPackage(prefabPath, stagingPath,
                ExportPackageOptions.IncludeDependencies);
            if (!File.Exists(stagingPath))
                throw new IOException("パッケージが生成されませんでした。");
            // Replace only after export succeeds, preserving existing files on export failure.
            if (File.Exists(packagePath)) File.Replace(stagingPath, packagePath, null);
            else File.Move(stagingPath, packagePath);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog("書き出し失敗", exception.Message, "OK");
            return;
        }
        finally
        {
            if (copy != null) DestroyImmediate(copy);
            if (prefabPath != null)
                AssetDatabase.DeleteAsset(prefabPath);
            if (stagingPath != null && File.Exists(stagingPath))
                File.Delete(stagingPath);
        }
        EditorUtility.RevealInFinder(packagePath);
    }
}
#endif
