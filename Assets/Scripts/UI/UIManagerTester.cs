using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerTester : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 快速测试 ===");

        // 1. 检查UIManager
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui == null)
        {
            Debug.LogError("❌ 找不到UIManager");
            return;
        }

        // 2. 检查配置
        Debug.Log($"配置数量: {ui._panelConfig.Length}");
        foreach (var config in ui._panelConfig)
        {
            Debug.Log($"配置: {config.panelPrefab?.name}, 路径: {config.path}");
        }

        // 3. 检查Resources文件
        string testPath = ui.rootPath + "CharacterPanel";
        GameObject prefab = Resources.Load<GameObject>(testPath);
        Debug.Log($"Resources加载: {prefab != null} (路径: {testPath})");

        // 4. 直接测试打开
        ui.OpenPanel("CharacterPanel");
    }
}