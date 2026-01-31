using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//属性类型枚举
public enum AttributeType
{
    //基础属性
    Hp,               //生命值
    MaxHp,            //最大生命值
    Attack,           //攻击力          
    Defense,          //防御力  
    Speed,            //速度
    CritRate,         //暴击率  
    CritDamage,       //暴击伤害  
    AttackMultiplier  //攻击倍率（每段普通）
}
//数值修改类型
public enum DataModifyType
{
    CurrentValue,     //当前值（Current
    AdditionalValue,  //附加值
    PercentageValue   //百分比
}
// 数值操作类型
public enum DataOperateType
{
    Addition,               // 加法
    Set,                    // 设置(只针对当前值)
    Percentage,             // 百分比(基于目标属性)
    PercentageTrigger,      // 百分比(基于施法者属性)
}
// 数值层级
public enum AttributeLayer
{
    Base,           //基础层
    Equipment,      //装备层
    Buff            //Buff层
}