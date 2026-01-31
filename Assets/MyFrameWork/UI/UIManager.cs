using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LevelEvents;

[Serializable]
public struct PanelConfig
{
    public GameObject panelPrefab;
    public string path;
}

public class UIManager : SingletonMonoBase<UIManager>
{
    public PanelConfig[] _panelConfig = Array.Empty<PanelConfig>();
    [SerializeField] public string rootPath;
    [SerializeField] public Transform _root;//UI的挂载根节点
    private Dictionary<string, string> pathDict;
    private Dictionary<string, GameObject> prefabDict;//存储UI预制体
    public Dictionary<string, UIPanelBase> activePanelDict;//存储当前已经打开的界面
    public Dictionary<string, UIPanelBase> negativePanelDict;//存储当前关闭的界面

    protected override void Awake()
    {
        base.Awake();
        InitDicts();
        RegisterEventListeners();
    }

    public Transform UIRoot
    {
        get
        {
            //TODO: 这里还是需要挂载
            if (!_root)
            {
                _root = new GameObject("RootUICanvas").transform;
                Canvas canvas = _root.gameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                CanvasScaler canvasScaler = _root.gameObject.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);

                _root.gameObject.AddComponent<GraphicRaycaster>();
            }
            return _root;
        }
    }

    /// <summary>
    /// 注册事件监听
    /// </summary>
    private void RegisterEventListeners()
    {
        EventCenter.Instance.Subscribe<AttributeEvents.AttributeChanged>(OnAttributeChanged);
        EventCenter.Instance.Subscribe<EquipmentEvents.WeaponUpgraded>(OnWeaponUpgraded);
        // 新增等级事件
        EventCenter.Instance.Subscribe<LevelEvents.LevelChanged>(OnLevelChanged);
    }

    /// <summary>
    /// 属性变化事件处理
    /// </summary>
    private void OnAttributeChanged(AttributeEvents.AttributeChanged e)
    {
        foreach (var panel in activePanelDict.Values)
        {
            panel.OnAttributeChanged(e);
        }
    }


    // 新增等级变化处理
    private void OnLevelChanged(LevelEvents.LevelChanged e)
    {
        foreach (var panel in activePanelDict.Values)
        {
            panel.OnLevelChanged(e);
        }
    }

    /// <summary>
    /// 武器升级事件处理
    /// </summary>
    private void OnWeaponUpgraded(EquipmentEvents.WeaponUpgraded e)
    {
        foreach (var panel in activePanelDict.Values)
        {
            panel.OnWeaponUpgraded(e);
        }
    }


    /// <summary>
    /// 打开UI界面
    /// </summary>
    /// <param name="panelName"></param>
    /// <returns></returns>
    public UIPanelBase OpenPanel(string panelName)
    {
        UIPanelBase panel = null;
        //查看已打开UI字典，检查该UI是否已经打开
        if (activePanelDict.TryGetValue(panelName, out panel))
        {
            Debug.LogWarning(panelName + " is already open");
            return panel;
        }
        //若关闭界面字典中存在，说明已经实例化过，则取出来并设为active即可
        if (negativePanelDict.TryGetValue(panelName, out panel))
        {
            negativePanelDict.Remove(panelName);
            activePanelDict.Add(panelName, panel);
            panel.OpenPanel();
            return panel;
        }
        //检查路径是否存在UIConfig配置类中
        string panelPath = "";
        if (!pathDict.TryGetValue(panelName, out panelPath))
        {
            Debug.LogError(panelName + " doesn't exist");
            return null;
        }
        //若预制件缓存字典中存在，则直接使用，否则进行加载
        GameObject panelPrefab = null;
        if (!prefabDict.TryGetValue(panelName, out panelPrefab))
        {
            string realPath = rootPath + panelPath;
            panelPrefab = Resources.Load<GameObject>(realPath);
            prefabDict.Add(panelName, panelPrefab);
        }
        //打开界面
        GameObject panelObject = Instantiate(panelPrefab, UIRoot, false);
        panel = panelObject.GetComponent<UIPanelBase>();
        activePanelDict.Add(panelName, panel);
        panel.OpenPanel();
        return panel;
    }

    public bool ClosePanel(string panelName)
    {
        UIPanelBase panel = null;
        if (!activePanelDict.TryGetValue(panelName, out panel))
        {
            Debug.LogWarning(panelName + " is already closed");
            return false;
        }

        activePanelDict.Remove(panelName);
        negativePanelDict.Add(panelName, panel);
        panel.ClosePanel();
        return true;
    }

    private void InitDicts()
    {
        prefabDict = new Dictionary<string, GameObject>();
        activePanelDict = new Dictionary<string, UIPanelBase>();
        negativePanelDict = new Dictionary<string, UIPanelBase>();
        pathDict = new Dictionary<string, string>();
        foreach (var config in _panelConfig)
        {
            pathDict.Add(config.panelPrefab.name, config.path);
        }
    }

    public UIPanelBase GetPanel(string panelName)
    {
        //若关闭界面字典中存在，说明已经实例化过，则取出来并设为active即可
        if (activePanelDict.TryGetValue(panelName, out UIPanelBase   panel))
        {
            return panel;
        }
        return null;
    }

    //protected override void OnDestroy()
    //{
    //    EventCenter.Instance.UnsubscribeAll(this);
    //    base.OnDestroy();
    //}
}