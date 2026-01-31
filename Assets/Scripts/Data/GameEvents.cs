using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 数值系统相关事件
/// </summary>
public struct AttributeEvents
{
    public struct AttributeChanged
    {
        public AttributeType AttributeType;//变化的属性类型
        public float OldValue;             //变化前的值
        public float NewValue;             //变化后的值
        public float CurrentValue;        //当前值（对于HP/MP等）
        public string Source;               //变化来源（技能，装备 ，Buff等）
        public bool IsPercentageChange;     //是否是百分比变化
    }

    /// <summary>
    /// 伤害事件 - 当造成伤害时触发
    /// </summary>
    public struct DamageDealt
    {
        public string TargetId;     //目标ID
        public string SourceId;     //伤害来源Id
        public float DamageAmount;  //伤害量
        public bool IsCritical;     //是否暴击
        public DamageType DamageType; //伤害类型
        public Vector3 HitPosition; //击中位置
    }
    /// <summary>
    /// 治疗事件
    /// </summary>
    public struct HealingApplied
    {
        public string TargetId;     //目标ID
        public string SourceId;     //治疗来源ID
        public float HealAmount;    //治疗量
        public bool IsCritical;     //是否暴击治疗
    }

    /// <summary>
    /// 状态效果事件
    /// </summary>
    public struct StatusEffectApplied
    {
        public string TargetId;     //目标ID
        public string EffectId;     //效果ID
        public float Duration;      //持续时间
        public int Stacks;          //叠加层数
        public EffectType EffectType; //效果类型
    }

    public struct ModifierApplied
    {
        public string TargetId;
        public DataModify Modifier;
        public string Source;
        public AttributeLayer Layer;
    }
}

/// <summary>
/// 武器系统相关事件
/// </summary>
public struct EquipmentEvents
{

    /// <summary>
    /// 武器升级事件 - 当武器升级时触发
    /// </summary>
    public struct WeaponUpgraded
    {
        public string WeaponId;     //武器Id
        public int OldLevel;        //升级前等级
        public int NewLevel;        //升级后等级
        public float AttackBonus;   //攻击力加成
        public float CritBonus;     //暴击加成
    }
    /// <summary>
    /// 武器装备事件 - 当装备或切换武器时触发
    /// </summary>

    public struct WeaponEquipped
    {
        public string CharacterId;  //角色ID
        public string WeaponId;     //武器Id
        public WeaponSlot Slot;     //武器槽位？？
        public bool IsMainHand;     //是否主手武器
    }


    /// <summary>
    /// 装备属性更新 - 当装备属性变化时触发
    /// </summary>
    public struct EquipmentStatsUpdated
    {
        public string CharacterId;      //角色Id
        public Dictionary<AttributeType, float> StatChanges;//属性变化字典
    }

}


/// <summary>
/// UI系统相关事件
/// </summary>
public struct UIEvents
{
    /// <summary>
    /// UI面板打开事件 - 当打开ui面板时触发
    /// </summary>
    public struct PanelOpened
    {
        public string PanelId;      //面板Id
        public UIPanelBase Panel;   //面板实例
    }

    /// <summary>
    /// UI面板关闭
    /// </summary>
    public struct PanleClosed
    {
        public string PanelId;  //  面板Id
    }

    public struct RefreshRequested
    {
        public string TargetPanel;      //目标面板(为空表示所有面板)
        public string Reason;           //刷新原因
    }
}

/// <summary>
/// 角色升级事件
/// </summary>
public struct LevelEvents
{

    /// <summary>
    /// 升级请求事件 - 点击升级 按钮时发布
    /// </summary>
    public struct LevelUpRequested
    {
        public string CharacterId;
        public int CurrentLevel;
    }

    public struct LevelChanged
    {
        public string CharacterId;
        public int OldLevel;
        public int NewLevel;
        public Dictionary<AttributeType, float> AttributeChanges;
    }
}

//枚举类型
public enum DamageType { Physical,Fire,Ice,Water}//伤害类型
public enum WeaponSlot { MainHand,OffHand ,TwoHand}//武器槽位
public enum EffectType { Buff,Debuff}   //效果类型




