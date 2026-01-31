using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理单个属性某一层级的数值计算
[SerializeField]
public class AttributeLayerData 
{
    [Header("基础值")]
    public float rootValue;     //根值：默认属性
    public float baseValue;     //基础值：如等级属性点加的值

    [Header("修饰值")]
    public float percentage;    //百分比加成：被动技能，天赋，种族等带来的百分比加成，装备层就是装备百分比加成
    public float additional;    //附加值
    public AttributeLayerData(float root = 0, float bas = 0, float perc = 0, float add = 0)
    {
        rootValue = root;
        baseValue = bas;
        percentage = perc;
        additional = add;
    }

    //计算该层的数值
    public float CalculateLayerValue(float previousLayerValue)//上一层的数据
    {
        return (previousLayerValue+baseValue)*(1+percentage)+additional;
    }

    //重置修饰值(用于回滚)
    public void ResetModifiers()
    {
        percentage = 0;
        additional = 0;     
    }

    //应用修改到哪一层  TODO
    public void ApplyModify(DataModify modify,ref float actualValue)
    {
        switch(modify.dataModifyType)
        {
            case DataModifyType.AdditionalValue:
                additional += modify.modify;
                actualValue = modify.modify;
                break;
            case DataModifyType.PercentageValue:
                percentage += modify.modify;
                actualValue = modify.modify;
                break;
        }
    }
}
