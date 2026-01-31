using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

//数值修改结构
[SerializeField]
public struct DataModify
{
    public AttributeType attributeType;   //修改哪个属性
    public DataModifyType dataModifyType; //修改哪一个数据
    public float modify;                //修改值
    public DataOperateType operateType;//操作类型
    public AttributeType percentageType;//基于属性百分比
    // 便捷构造函数
    public DataModify(AttributeType attrType, DataModifyType modifyType, float value,
                     DataOperateType opType = DataOperateType.Addition,
                     AttributeType percentType = AttributeType.Hp)
    {
        attributeType = attrType;
        dataModifyType = modifyType;
        modify = value;
        operateType = opType;
        percentageType = percentType;
    }
}
//数值记录修改（用于回滚）
[SerializeField]
public struct ModifyRecord
{
    public string sourceId;      //修改来源ID(BuffID、技能ID等)
    public DataModify modify;   //修改内容
    public AttributeLayer layer;//影响的层级
    public float actualValue;   //实际修改的数值（用于回滚）
    public DateTime timestamp;      // 时间戳
}