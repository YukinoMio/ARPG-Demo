using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeModifier 
{
    //计算实际修改值
    public float CalculateActualModifyValue(DataModify modify,AttributeData targetData,AttributeData casterData)
    {
        switch (modify.operateType)
        {
            case DataOperateType.PercentageTrigger:
                return targetData.CalculateFinalValue() * modify.modify;
                break;
            case DataOperateType.Percentage:
                // 基于施法者属性的百分比
                if (casterData != null)
                {
                    // 这里需要获取施法者的对应属性值
                    // 暂时返回0，实际使用时需要传入完整的属性系统
                    return 0f;
                }
                return 0f;
            case DataOperateType.Addition:
            case DataOperateType.Set:
            default:
                return modify.modify;
        }
    }
    // 获取修改目标层级
    public AttributeLayer GetTargetLayer(DataModifyType modifyType)
    {
        return modifyType switch
        {
            DataModifyType.AdditionalValue => AttributeLayer.Buff,
            DataModifyType.PercentageValue => AttributeLayer.Buff,
            DataModifyType.CurrentValue => AttributeLayer.Base, // 当前值修改属于基础层
            _ => AttributeLayer.Base
        };
    }
}
