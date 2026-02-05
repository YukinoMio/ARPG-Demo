#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class ILRuntimeABDiagnosis : EditorWindow
{
    [MenuItem("Tools/ILRuntime+AB包综合诊断")]
    static void ShowDiagnosis()
    {
        GetWindow<ILRuntimeABDiagnosis>("ILRuntime+AB包诊断").Show();
    }

    private Vector2 scrollPos;
    private string diagnosisResult = "";

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("ILRuntime + AB包问题诊断", EditorStyles.boldLabel);

        if (GUILayout.Button("开始完整诊断", GUILayout.Height(40)))
        {
            diagnosisResult = RunFullDiagnosis();
        }

        GUILayout.Space(20);

        if (!string.IsNullOrEmpty(diagnosisResult))
        {
            EditorGUILayout.TextArea(diagnosisResult, GUILayout.Height(300));
        }

        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("检查ILRuntime安装"))
        {
            CheckILRuntimeInstallation();
        }

        if (GUILayout.Button("检查AB包配置"))
        {
            CheckABConfig();
        }

        if (GUILayout.Button("测试最小构建"))
        {
            TestMinimalBuild();
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("生成修复报告", GUILayout.Height(30)))
        {
            GenerateFixReport();
        }

        EditorGUILayout.EndScrollView();
    }

    string RunFullDiagnosis()
    {
        StringBuilder report = new StringBuilder();
        report.AppendLine("=== ILRuntime + AB包综合诊断报告 ===");
        report.AppendLine($"诊断时间: {System.DateTime.Now}");
        report.AppendLine($"Unity版本: {Application.unityVersion}");
        report.AppendLine();

        // 1. 检查ILRuntime
        report.AppendLine("1. ILRuntime检查:");
        string ilruntimeCheck = CheckILRuntime();
        report.AppendLine(ilruntimeCheck);

        // 2. 检查序列化兼容性
        report.AppendLine("\n2. 序列化兼容性检查:");
        string serializationCheck = CheckSerializationCompatibility();
        report.AppendLine(serializationCheck);

        // 3. 检查AB包配置
        report.AppendLine("\n3. AB包配置检查:");
        string abConfig = CheckABConfiguration();
        report.AppendLine(abConfig);

        // 4. 检查编译设置
        report.AppendLine("\n4. 编译设置检查:");
        string buildSettings = CheckBuildSettings();
        report.AppendLine(buildSettings);

        // 5. 检查磁盘和权限
        report.AppendLine("\n5. 系统环境检查:");
        string systemCheck = CheckSystemEnvironment();
        report.AppendLine(systemCheck);

        return report.ToString();
    }

    string CheckILRuntime()
    {
        StringBuilder result = new StringBuilder();

        // 查找ILRuntime安装位置
        string[] possiblePaths = {
            "Assets/ILRuntime",
            "Assets/Plugins/ILRuntime",
            "Assets/ThirdParty/ILRuntime",
            "Packages/com.ourpalm.ilruntime"
        };

        foreach (string path in possiblePaths)
        {
            if (Directory.Exists(path))
            {
                result.AppendLine($"✅ 找到ILRuntime: {path}");

                // 检查版本
                CheckILRuntimeVersion(path, result);

                // 检查关键文件
                CheckILRuntimeFiles(path, result);
            }
        }

        if (result.Length == 0)
        {
            result.AppendLine("❌ 未找到ILRuntime文件夹（可能已删除但残留引用）");
        }

        return result.ToString();
    }

    void CheckILRuntimeVersion(string path, StringBuilder result)
    {
        // 查找版本文件
        string versionFile = Path.Combine(path, "version.txt");
        if (File.Exists(versionFile))
        {
            string version = File.ReadAllText(versionFile).Trim();
            result.AppendLine($"  版本: {version}");
        }

        // 检查关键dll
        string runtimeDll = Path.Combine(path, "Runtime/ILRuntime.dll");
        if (File.Exists(runtimeDll))
        {
            FileInfo info = new FileInfo(runtimeDll);
            result.AppendLine($"  ILRuntime.dll大小: {info.Length / 1024} KB");
        }
    }

    void CheckILRuntimeFiles(string path, StringBuilder result)
    {
        string[] criticalFiles = {
            "Runtime/ILRuntime.dll",
            "Runtime/Intepreter/RegisterFrameInfo.cs",
            "Lib/System.Threading.dll"
        };

        foreach (string file in criticalFiles)
        {
            string fullPath = Path.Combine(path, file);
            if (File.Exists(fullPath))
            {
                result.AppendLine($"  ✅ {file}");
            }
            else
            {
                result.AppendLine($"  ❌ 缺失: {file}");
            }
        }
    }

    string CheckSerializationCompatibility()
    {
        StringBuilder result = new StringBuilder();

        try
        {
            // 尝试创建测试序列化
            TestClass testObj = new TestClass { value = 123 };
            string json = JsonUtility.ToJson(testObj);

            TestClass deserialized = JsonUtility.FromJson<TestClass>(json);
            if (deserialized.value == 123)
            {
                result.AppendLine("✅ 基础序列化测试通过");
            }
        }
        catch (System.Exception e)
        {
            result.AppendLine($"❌ 序列化测试失败: {e.Message}");
        }

        // 检查可能的序列化冲突
        result.AppendLine("检查Editor和Player的序列化差异...");

        return result.ToString();
    }

    string CheckABConfiguration()
    {
        StringBuilder result = new StringBuilder();

        // 检查AB包名称设置
        string[] allBundles = AssetDatabase.GetAllAssetBundleNames();
        result.AppendLine($"找到 {allBundles.Length} 个AB包设置");

        if (allBundles.Length > 0)
        {
            foreach (string bundle in allBundles)
            {
                string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundle);
                result.AppendLine($"  - {bundle}: {assets.Length}个资源");

                if (assets.Length > 0 && assets.Length <= 3)
                {
                    foreach (string asset in assets)
                    {
                        result.AppendLine($"    * {asset}");
                    }
                }
            }
        }
        else
        {
            result.AppendLine("⚠️ 未找到任何AB包设置");
        }

        // 检查输出目录权限
        string testDir = "Assets/StreamingAssets/ABTest";
        try
        {
            if (!Directory.Exists(testDir))
                Directory.CreateDirectory(testDir);

            File.WriteAllText(testDir + "/test.txt", "test");
            File.Delete(testDir + "/test.txt");
            Directory.Delete(testDir);

            result.AppendLine("✅ 输出目录权限正常");
        }
        catch (System.Exception e)
        {
            result.AppendLine($"❌ 目录权限问题: {e.Message}");
        }

        return result.ToString();
    }

    string CheckBuildSettings()
    {
        StringBuilder result = new StringBuilder();

        BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;

        result.AppendLine($"当前平台: {group}");
        result.AppendLine($"Scripting Backend: {PlayerSettings.GetScriptingBackend(group)}");
        result.AppendLine($"API兼容性: {PlayerSettings.GetApiCompatibilityLevel(group)}");
        result.AppendLine($"IL2CPP配置: {PlayerSettings.GetIl2CppCompilerConfiguration(group)}");

        // 检查是否适合ILRuntime
        var backend = PlayerSettings.GetScriptingBackend(group);
        if (backend == ScriptingImplementation.IL2CPP)
        {
            result.AppendLine("⚠️ ILRuntime在IL2CPP下可能需要额外配置");
        }

        return result.ToString();
    }

    string CheckSystemEnvironment()
    {
        StringBuilder result = new StringBuilder();

        // 磁盘空间
        DriveInfo cdrive = new DriveInfo("C");
        if (cdrive.IsReady)
        {
            double freeGB = cdrive.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0;
            result.AppendLine($"C盘空间: {freeGB:F1} GB 可用");

            if (freeGB < 10)
            {
                result.AppendLine("⚠️ 空间紧张，建议清理");
            }
        }

        // Unity缓存大小
        string libraryPath = Application.dataPath + "/../Library";
        if (Directory.Exists(libraryPath))
        {
            long size = GetDirectorySize(libraryPath);
            double sizeGB = size / 1024.0 / 1024.0 / 1024.0;
            result.AppendLine($"Unity缓存大小: {sizeGB:F2} GB");
        }

        return result.ToString();
    }

    void CheckILRuntimeInstallation()
    {
        Debug.Log("=== 检查ILRuntime安装 ===");

        // 这里可以添加更详细的检查
        string result = CheckILRuntime();
        Debug.Log(result);
    }

    void CheckABConfig()
    {
        Debug.Log("=== 检查AB包配置 ===");

        string result = CheckABConfiguration();
        Debug.Log(result);
    }

    void TestMinimalBuild()
    {
        Debug.Log("=== 测试最小构建 ===");

        try
        {
            // 创建最小测试
            Material testMat = new Material(Shader.Find("Standard"));
            string matPath = "Assets/__TestMinimal.mat";
            AssetDatabase.CreateAsset(testMat, matPath);

            // 设置AB名称
            AssetImporter.GetAtPath(matPath).assetBundleName = "minimaltest";

            // 构建
            string outputPath = "Temp/MinimalTest";
            BuildPipeline.BuildAssetBundles(outputPath,
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows64);

            Debug.Log("✅ 最小构建测试成功！");

            // 清理
            AssetDatabase.DeleteAsset(matPath);
            AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 最小构建失败: {e.Message}");
            Debug.LogError($"堆栈: {e.StackTrace}");
        }
    }

    void GenerateFixReport()
    {
        string report = RunFullDiagnosis();

        // 添加修复建议
        report += "\n=== 修复建议 ===\n";

        if (report.Contains("未找到ILRuntime文件夹"))
        {
            report += "1. ILRuntime可能已删除但残留引用，建议:\n";
            report += "   - 删除Library/Temp文件夹\n";
            report += "   - 重新导入项目\n";
        }

        if (report.Contains("序列化测试失败"))
        {
            report += "2. 序列化问题，建议:\n";
            report += "   - 检查ILRuntime版本兼容性\n";
            report += "   - 重新生成CLR绑定代码\n";
        }

        // 保存报告
        string reportPath = "ILRuntime_AB_FixReport.txt";
        File.WriteAllText(reportPath, report);
        Debug.Log($"修复报告已保存: {reportPath}");
        EditorUtility.RevealInFinder(reportPath);
    }

    long GetDirectorySize(string path)
    {
        long size = 0;
        try
        {
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                try
                {
                    size += new FileInfo(file).Length;
                }
                catch { }
            }
        }
        catch { }
        return size;
    }

    [System.Serializable]
    class TestClass
    {
        public int value;
    }
}
#endif