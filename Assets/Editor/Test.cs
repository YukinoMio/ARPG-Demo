// 把这个脚本放在 Assets/Editor/TestBuild.cs
using System.IO;
using UnityEditor;
using UnityEngine;

public class Test
{
    [MenuItem("Tools/检查UnityEditor引用")]
    static void CheckUnityEditorReferences()
    {
        string[] allScripts = AssetDatabase.FindAssets("t:Script");

        foreach (var guid in allScripts)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // 读取文件内容
            string content = File.ReadAllText(path);

            if (content.Contains("using UnityEditor;"))
            {
                // 检查是否在Editor文件夹中
                bool isInEditorFolder = path.Contains("/Editor/");

                if (!isInEditorFolder)
                {
                    Debug.LogError($"❌ 脚本 {Path.GetFileName(path)} 使用了UnityEditor但没有在Editor文件夹中！\n路径: {path}");
                }
                else
                {
                    Debug.Log($"✅ 正确: {Path.GetFileName(path)} 在Editor文件夹中");
                }
            }
        }
    }
}
