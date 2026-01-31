using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("基础信息")]
    public string weaponId;
    public string uid;//唯一标识符
    public string weaponName;
    public WeaponType weaponType; // 单手剑、双手剑、法杖等
    public int weaponLevel;
    public WeaponRarity weaponRarity;//武器星级
    public string description;//描述

    [Header("武器模型")]
    public GameObject weaponPrefab;
    public Vector3 equipPosition=Vector3.zero;
    public Vector3 equipRotation=Vector3.zero;  

    [Header("基础数值")]
   public List<DataModify> baseAttributes=new List<DataModify>();

    [Header("武器特效")]
    public List<WeaponBuff> passiveBuffs; // 被动效果
    //public List<SkillData> weaponSkills;  // 武器专属技能

    [Header("成长数值")]
    public List<DataModify> growthAttributes=new List<DataModify>();//升级成长

    [Header("动作配置")]
    public string attackAnimation;
    public string skillAnimation;
    public string idleAnimation;

    [Header("音效配置")]
    public AudioClip equipSound;
    public AudioClip attackSound;
}

[System.Serializable]
public class WeaponBuff
{
    //public BuffData buffData;
    public float triggerChance; // 触发概率
    //public BuffTriggerType triggerType; // 攻击时、受击时、常驻等
}

public enum WeaponType
{
    Empty,
    Katana,
    Staff,
    GreatSword,
    Bow
}
//武器品质星级
public enum WeaponRarity
{
    TwoStar,
    ThreeStar,
    FourStar,
    FiveStar,
}
