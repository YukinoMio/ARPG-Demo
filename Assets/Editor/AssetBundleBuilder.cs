using UnityEditor;
using System.IO;
using UnityEngine;

public class AssetBundleBuilder : EditorWindow
{
    private BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
    private string outputPath = "AssetBundles";

    [MenuItem("Tools/自定义 AB 打包")]
    public static void ShowWindow()
    {
        GetWindow<AssetBundleBuilder>("AB打包工具");
    }

    void OnGUI()
    {
        GUILayout.Label("基础打包设置", EditorStyles.boldLabel);
        buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("目标平台：", buildTarget);
        outputPath = EditorGUILayout.TextField("输出路径：", outputPath);

        EditorGUILayout.Space();

        if (GUILayout.Button("构建所有AssetBundle"))
        {
            BuildAllBundles();
        }

        if (GUILayout.Button("打开输出文件夹"))
        {
            if (Directory.Exists(outputPath))
            {
                EditorUtility.RevealInFinder(outputPath);
            }
            else
            {
                Debug.LogWarning("输出文件夹不存在，请先构建。");
            }
        }
    }

    void BuildAllBundles()
    {
        // 确保输出目录存在
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        // 关键：添加此选项，可绕过类型序列化问题
        BuildAssetBundleOptions options = BuildAssetBundleOptions.DisableWriteTypeTree;

        // 执行构建
        BuildPipeline.BuildAssetBundles(outputPath, options, buildTarget);
        Debug.Log($"AssetBundle 构建完成！路径：{Path.GetFullPath(outputPath)}");

        // 刷新编辑器，让新文件显示出来
        AssetDatabase.Refresh();
    }
}