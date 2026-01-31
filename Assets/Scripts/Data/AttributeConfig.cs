using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttributeConfig", menuName = "Game/Attribute Config")]
public class AttributeConfig : ScriptableObject
{
    [Header("基础属性配置")]
    public float baseHp = 100f;
    public float baseMp = 50f;
    public float baseAttack = 10f;
    public float baseDefense = 5f;
    public float baseSpeed = 1f;

    [Header("成长配置")]
    public AnimationCurve hpGrowthCurve;
    public AnimationCurve attackGrowthCurve;
    public AnimationCurve defenseGrowthCurve;

    [Header("其他配置")]
    public float maxHpLimit = 99999f;
    public float maxMpLimit = 9999f;
}