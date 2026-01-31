using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 集成了事件系统的数值系统
/// </summary>
public class AdvancedAttributeSystem : PlayerComponentBase
{
    [Header("事件调试")]
    [SerializeField] private bool enableEventLogging = true;

    private Dictionary<AttributeType, AttributeData> attributes = new Dictionary<AttributeType, AttributeData>();

    public override void Initialize(PlayerController playerController)
    {
        base.Initialize(playerController);
        InitializeAttributes();
    }

    /// <summary>
    /// 应用数值修改并发布相应事件
    /// </summary>
    public void ApplyModify(DataModify modify, string sourceId, AttributeData casterData = null)
    {
        Debug.Log($"=== ApplyModify调试 ===");
        Debug.Log($"属性类型: {modify.attributeType}");
        Debug.Log($"字典数量: {attributes.Count}");
        Debug.Log($"包含{modify.attributeType}? {attributes.ContainsKey(modify.attributeType)}");
        if (!attributes.ContainsKey(modify.attributeType))
        {
            Debug.LogWarning($"未知属性类型: {modify.attributeType}");
            return;
        }

        var targetData = attributes[modify.attributeType];
        float oldFinalValue = GetAttributeValue(modify.attributeType);
        float oldCurrentValue = targetData.currentValue;

        // 计算实际修改值
        float actualModifyValue = CalculateActualModifyValue(modify, targetData, casterData);

        // 应用修改到数据
        ApplyModifyToData(modify, targetData, actualModifyValue, sourceId);

        // 发布属性变化事件
        PublishAttributeEvents(modify, sourceId, oldFinalValue, oldCurrentValue, actualModifyValue);

        if (enableEventLogging)
        {
            Debug.Log($"属性修改: {modify.attributeType} [{modify.dataModifyType}] {actualModifyValue} (来源: {sourceId})");
        }
    }

    /// <summary>
    /// 发布所有相关的事件
    /// </summary>
    private void PublishAttributeEvents(DataModify modify, string sourceId, float oldFinalValue, float oldCurrentValue, float actualModifyValue)
    {
        var targetData = attributes[modify.attributeType];
        float newFinalValue = GetAttributeValue(modify.attributeType);
        float newCurrentValue = targetData.currentValue;

        // 1. 发布基础属性变化事件
        EventCenter.Instance.Publish(new AttributeEvents.AttributeChanged
        {
            AttributeType = modify.attributeType,
            OldValue = oldFinalValue,
            NewValue = newFinalValue,
            CurrentValue = newCurrentValue,
            Source = sourceId,
            IsPercentageChange = modify.dataModifyType == DataModifyType.PercentageValue
        });

        // 2. 发布特定属性类型的事件
        PublishSpecificAttributeEvents(modify, sourceId, oldCurrentValue, newCurrentValue, actualModifyValue);

        // 3. 发布修改器应用事件
        EventCenter.Instance.Publish(new AttributeEvents.ModifierApplied
        {
            TargetId = player.gameObject.name,
            Modifier = modify,
            Source = sourceId,
            Layer = GetTargetLayer(modify.dataModifyType)
        });
    }

    /// <summary>
    /// 发布特定属性类型的事件（伤害、治疗等）
    /// </summary>
    private void PublishSpecificAttributeEvents(DataModify modify, string sourceId, float oldCurrentValue, float newCurrentValue, float actualModifyValue)
    {
        if (modify.attributeType == AttributeType.Hp)
        {
            if (actualModifyValue < 0) // 伤害事件
            {
                EventCenter.Instance.Publish(new AttributeEvents.DamageDealt
                {
                    TargetId = player.gameObject.name,
                    SourceId = sourceId,
                    DamageAmount = Mathf.Abs(actualModifyValue),
                    IsCritical = CheckIfCritical(),
                    DamageType = DamageType.Physical,
                    HitPosition = player.transform.position
                });
            }
            else if (actualModifyValue > 0 && modify.dataModifyType == DataModifyType.CurrentValue) // 治疗事件
            {
                EventCenter.Instance.Publish(new AttributeEvents.HealingApplied
                {
                    TargetId = player.gameObject.name,
                    SourceId = sourceId,
                    HealAmount = actualModifyValue,
                    IsCritical = CheckIfCriticalHeal()
                });
            }
        }
    }


    //获取基础值（用于计算增长）
    public float GetBaseValue(AttributeType type)
    {
        if(attributes.TryGetValue(type,out var data))
        {
            return data.baseLayer.rootValue;//返回基础层的根植
        }
        return 0f;
    }
    /// <summary>
    /// 检查是否暴击
    /// </summary>
    private bool CheckIfCritical()
    {
        float critRate = GetAttributeValue(AttributeType.CritRate);
        return UnityEngine.Random.value < critRate;
    }

    /// <summary>
    /// 检查是否暴击治疗
    /// </summary>
    private bool CheckIfCriticalHeal()
    {
        // 根据游戏设计决定治疗是否可以暴击
        return false;
    }

    /// <summary>
    /// 获取修改目标层级
    /// </summary>
    private AttributeLayer GetTargetLayer(DataModifyType modifyType)
    {
        return modifyType switch
        {
            DataModifyType.AdditionalValue => AttributeLayer.Buff,
            DataModifyType.PercentageValue => AttributeLayer.Buff,
            DataModifyType.CurrentValue => AttributeLayer.Base,
            _ => AttributeLayer.Base
        };
    }

    // 原有的数值系统方法

    //计算修改值
    private float CalculateActualModifyValue(DataModify modify, AttributeData targetData, AttributeData casterData) { return modify.modify; }
   
    //应用修改
    private void ApplyModifyToData(DataModify modify, AttributeData targetData, float actualModifyValue, string sourceId)
    {
        // 根据修改类型应用到对应层级
        switch (modify.dataModifyType)
        {
            case DataModifyType.AdditionalValue:
                targetData.buffLayer.additional += actualModifyValue;
                break;
            case DataModifyType.PercentageValue:
                targetData.buffLayer.percentage += actualModifyValue;
                break;
            case DataModifyType.CurrentValue:
                targetData.currentValue += actualModifyValue;
                break;
        }
    }
    public float GetAttributeValue(AttributeType type)
    {
        if (attributes.TryGetValue(type, out var data))
        {
            return data.CalculateFinalValue();
        }
        return 0f;
    }
    private void InitializeAttributes() {
        attributes[AttributeType.MaxHp] = new AttributeData(100f);
        attributes[AttributeType.Hp] = new AttributeData(100f);
        attributes[AttributeType.Attack] = new AttributeData(100f);
        attributes[AttributeType.Defense] = new AttributeData(100f);
        attributes[AttributeType.Speed] = new AttributeData(100f);
        attributes[AttributeType.CritRate] = new AttributeData(50.2f);
        attributes[AttributeType.CritDamage] = new AttributeData(200.8f);
        attributes[AttributeType.AttackMultiplier] = new AttributeData(100f);

    }

    private void OnDestroy()
    {
        // 自动清理事件订阅
        EventCenter.Instance.UnsubscribeAll(this);
    }
}