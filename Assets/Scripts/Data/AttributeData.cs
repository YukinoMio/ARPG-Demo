using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理一个属性的三层计算和当前值
[SerializeField]
public class AttributeData 
{
    [Header("基础层")]
    public AttributeLayerData baseLayer=new AttributeLayerData();
    [Header("装备层")]
    public AttributeLayerData equipmentLayer=new AttributeLayerData();
    [Header("Buff层")]
    public AttributeLayerData buffLayer=new AttributeLayerData();
    [Header("当前值")]
    public float currentValue;      //当前值（比如当前生命值）

    //修改记录（用于回滚）
    public List<ModifyRecord> modifyRecords= new List<ModifyRecord>();

    public AttributeData(float rootValue = 0)
    {
        baseLayer .rootValue = rootValue;
        currentValue = rootValue;
    }
    //计算最终数值
    public float CalculateFinalValue()
    {
        float result=baseLayer.CalculateLayerValue(baseLayer.rootValue);
        result = equipmentLayer.CalculateLayerValue(result);
        result=buffLayer.CalculateLayerValue(result);
        return result;
    }

    //获取当前层的数据
    public AttributeLayerData GetLayerData(AttributeLayer layer)
    {
        return layer switch
        {
            AttributeLayer.Base => baseLayer,
            AttributeLayer.Equipment => equipmentLayer,
            AttributeLayer.Buff => buffLayer,
            _ => baseLayer,
        };
    }

    //应用当前值修改
    public void ApplyCurrentValueModify(DataModify modify,float actualValue)
    {
        switch (modify.operateType)
        {
            case DataOperateType.Addition:
                currentValue += actualValue;
                break;
            case DataOperateType.Set:
                currentValue = actualValue;
                break;
            case DataOperateType.Percentage:
                break;
            case DataOperateType.PercentageTrigger:
                break;
            default:
                break;
        }
        //限制当前值在合理范围之内
        float maxValue=CalculateFinalValue();
        currentValue=Mathf.Clamp(currentValue,0,maxValue);  
    }
    //添加修改记录
    public void AddModifyRecord(ModifyRecord record)
    {
        modifyRecords.Add(record);
    }
    //移除指定来源的修改记录
    public void RemoveModifyRecords(string sourceId)
    {
        modifyRecords.RemoveAll(record => record.sourceId == sourceId);
    }
}
