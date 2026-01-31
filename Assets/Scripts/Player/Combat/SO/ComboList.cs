using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ComboList",menuName="ScriptableObject/Combo/ComboList")]
public class ComboList : ScriptableObject
{
    [SerializeField] private ComboConfig[] comboList;

    public int TryGetComboListCount() => comboList.Length;
    //获取攻击名字
    public string TryGetComboName(int index)
    {
        return index>=comboList.Length? null : comboList[index].name;    
    }
    //获取冷却时间
    public float TryGetCoolDownTime(int index)
    {
        return index >= comboList.Length ? 0 : comboList[index].coolDownTime;
    }
    //获取攻击交互
    public ComboInteractionConfig TryGetComboInteractionConfig(int index,int eventIndex)
    {
        if (index >= comboList.Length)
        return null;
        if(eventIndex >= comboList[index].comboInteractionConfigs.Length)
            return null;
        return comboList[index].comboInteractionConfigs[eventIndex];
    }

    //获取特效
    public FXConfig TryGetFXConfig(int index, int eventIndex)
    {
        if (index >= comboList.Length)
            return null;
        if (eventIndex >=comboList[index].fxConfigs.Length)
            return null;
        return comboList[index].fxConfigs[eventIndex];
    }

    //获取音效
    public ClipConfig TryGetClipConfig(int index, int eventIndex)
    {
        if (index >= comboList.Length)
            return null;
        if (eventIndex >= comboList[index].clipConfigs.Length)
            return null;
        return comboList[index].clipConfigs[eventIndex];
    }
    //攻击反馈
    public AttackFeedbackConfig TryGetAttackFeedbackConfig(int index, int eventIndex)
    {
        if (index >= comboList.Length)
            return null;
        if (eventIndex >=comboList[index].attackFeedbackConfigs.Length)
            return null;
        return comboList[index].attackFeedbackConfigs[eventIndex];
    }
    //获得自身位移补偿
    public SelfMoveOffsetConfig TryGetSelfMoveOffsetConfig(int index, int eventIndex)
    {
        if (index >= comboList.Length)
            return null;
        if (eventIndex >= comboList[index].selfMoveOffsetConfigs.Length)
            return null;
        return comboList[index].selfMoveOffsetConfigs[eventIndex];
    }
    //获取目标位移补偿
    public TargetMoveOffsetConfig TryGetTargetMoveOffsetConfig(int index, int eventIndex)
    {
        if (index >= comboList.Length)
            return null;
        if (eventIndex >= comboList[index].targetMoveOffsetConfigs.Length)
            return null;
        return comboList[index].targetMoveOffsetConfigs[eventIndex];
    }
}
