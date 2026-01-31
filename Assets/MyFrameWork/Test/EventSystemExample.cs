using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件系统使用示例和测试类
/// </summary>
public class EventSystemExample : MonoBehaviour
{
    private void Start()
    {
        TestEventSystem();
    }

    /// <summary>
    /// 测试事件系统功能
    /// </summary>
    private void TestEventSystem()
    {
        // 1. 订阅事件
        EventCenter.Instance.Subscribe<AttributeEvents.AttributeChanged>(OnAttributeChanged);
        EventCenter.Instance.Subscribe<AttributeEvents.DamageDealt>(OnDamageDealt);
        EventCenter.Instance.Subscribe<EquipmentEvents.WeaponUpgraded>(OnWeaponUpgraded);

        // 2. 发布测试事件
        PublishTestEvents();

        // 3. 打印调试信息
        Invoke(nameof(PrintDebugInfo), 2f);
    }

    /// <summary>
    /// 属性变化事件处理
    /// </summary>
    private void OnAttributeChanged(AttributeEvents.AttributeChanged e)
    {
        Debug.Log($"属性变化: {e.AttributeType} {e.OldValue} -> {e.NewValue} (来源: {e.Source})");
    }

    /// <summary>
    /// 伤害事件处理
    /// </summary>
    private void OnDamageDealt(AttributeEvents.DamageDealt e)
    {
        Debug.Log($"造成伤害: {e.DamageAmount} 对 {e.TargetId} {(e.IsCritical ? "暴击!" : "")}");
    }

    /// <summary>
    /// 武器升级事件处理
    /// </summary>
    private void OnWeaponUpgraded(EquipmentEvents.WeaponUpgraded e)
    {
        Debug.Log($"武器升级: {e.WeaponId} Lv{e.OldLevel} -> Lv{e.NewLevel} (+{e.AttackBonus}攻击)");
    }

    /// <summary>
    /// 发布测试事件
    /// </summary>
    private void PublishTestEvents()
    {
        // 模拟属性变化
        EventCenter.Instance.Publish(new AttributeEvents.AttributeChanged
        {
            AttributeType = AttributeType.Hp,
            OldValue = 100f,
            NewValue = 80f,
            CurrentValue = 80f,
            Source = "TestDamage",
            IsPercentageChange = false
        });

        // 模拟伤害事件
        EventCenter.Instance.Publish(new AttributeEvents.DamageDealt
        {
            TargetId = "Player1",
            SourceId = "Enemy1",
            DamageAmount = 25f,
            IsCritical = true,
            DamageType = DamageType.Physical,
            HitPosition = transform.position
        });

        // 模拟武器升级
        EventCenter.Instance.Publish(new EquipmentEvents.WeaponUpgraded
        {
            WeaponId = "Sword_001",
            OldLevel = 1,
            NewLevel = 2,
            AttackBonus = 15f,
            CritBonus = 0.05f
        });
    }

    private void PrintDebugInfo()
    {
        EventCenter.Instance.PrintDebugInfo();
    }

    private void OnDestroy()
    {
        // 清理测试订阅
        EventCenter.Instance.UnsubscribeAll(this);
    }
}
