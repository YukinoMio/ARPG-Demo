using UnityEngine;

/// <summary>
/// UI面板基类 - 所有UI面板的基类
/// </summary>
public abstract class UIPanelBase : MonoBehaviour
{
    [SerializeField] protected string panelId;

    public string PanelId => panelId;

    public virtual void Initialize(string id)
    {
        panelId = id;
    }

    public virtual void OpenPanel()
    {
        gameObject.SetActive(true);
    }

    public virtual void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 属性变化时调用（可选实现）
    /// </summary>
    public virtual void OnAttributeChanged(AttributeEvents.AttributeChanged e) { }

    /// <summary>
    /// 武器升级时调用（可选实现）
    /// </summary>
    public virtual void OnWeaponUpgraded(EquipmentEvents.WeaponUpgraded e) { }

    /// <summary>
    /// 等级变化时调用（可选实现）
    /// </summary>
    public virtual void OnLevelChanged(LevelEvents.LevelChanged e) { }

    /// <summary>
    /// 刷新面板数据（可选实现）
    /// </summary>
    public virtual void Refresh() { }
}
