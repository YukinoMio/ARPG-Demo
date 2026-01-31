using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 属性监听接口 - 只包含真实存在的事件
/// </summary>
public interface IAttributeListener
{
    void OnAttributeChanged(AttributeEvents.AttributeChanged e);
    // 只有这一个方法，因为CurrentValueChanged不存在
}

/// <summary>
/// 战斗监听接口
/// </summary>
public interface ICombatListener
{
    void OnDamageDealt(AttributeEvents.DamageDealt e);
    void OnHealingApplied(AttributeEvents.HealingApplied e);
}

/// <summary>
/// 状态效果监听接口
/// </summary>
public interface IStatusEffectListener
{
    void OnModifierApplied(AttributeEvents.ModifierApplied e);
}
