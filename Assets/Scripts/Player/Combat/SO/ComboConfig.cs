using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ComboConfig",menuName = "ScriptableObject/Combat/ComboConfig")]
public class ComboConfig : ScriptableObject
{
    [Header("基础数据")]
    public string comboName;
    public float coolDownTime;//冷却时间
    [Header("交互数据")]
    public ComboInteractionConfig[] comboInteractionConfigs;
    [Header("特效数据")]
    public FXConfig[] fxConfigs;
    [Header("音效数据")]
    public ClipConfig[] clipConfigs;
    [Header("攻击反馈数据")]
    public AttackFeedbackConfig[] attackFeedbackConfigs;
    [Header("自身位移补偿数据")]
    public SelfMoveOffsetConfig[] selfMoveOffsetConfigs;
    [Header("目标位移补偿数据")]
    public TargetMoveOffsetConfig[] targetMoveOffsetConfigs;
}
[System.Serializable]
public class ComboInteractionConfig
{
    public float startTime;
    public float endTime;
    public string hitName;
    public string hitAirName;//空中受击
                           
    public WeaponType weaponType;
    public AttackForce attackForce;
    public int healthDamage;
    public int enduranceDamage;//耐力

}
[System.Serializable]
public class AttackDetectionConfig
{
    public float startTime;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}

[System.Serializable]
public class FXConfig
{
    public float startTime;
    public GameObject FXPrefab;
    public string FXName;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}

[System.Serializable]
public class ClipConfig
{
    public float startTime;
    public AudioClip audioClip;
    public float volume;
    public float duration;
}

[System.Serializable]
public class AttackFeedbackConfig
{
    public Vector3 velocity;//屏幕震动速度
    public AudioClip audioClip;//受击音效
    public float audioStartTime;
    public float animatorSpeed;//顿帧速度
    public float stopFrameTime;//顿帧时长
}
[System.Serializable]
public class SelfMoveOffsetConfig
{
    public float startTime;
    public AnimationCurve animationCurve;// 位移曲线
    public MoveOffsetDirection moveOffsetDirection;//方向
    public float scale;
    public float duration;
}

[System.Serializable]
public class TargetMoveOffsetConfig
{
    public float startTime;
    public AnimationCurve animationCurve;
    public MoveOffsetDirection moveOffsetDirection;//方向
    public float scale;
    public float duration;
}