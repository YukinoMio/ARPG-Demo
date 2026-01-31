using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerConfig",menuName ="Game/Player Config")]
public class PlayerConfig : ScriptableObject
{
    [Header("角色数据")]
    public string characterName;//角色名字
    public int level;//等级
    public WeaponType weaponType;//武器类型
    public ElementType weapon;//元素类型
    [Header("移动速度")]
    public float crouchSpeed = 0.8f;
    public float walkSpeed = 1.27f;
    public float runSpeed = 4.2f;

    public float GetWalkSpeed() => walkSpeed;
    public float GetRunSpeed() => runSpeed;
    [Header("动画阈值")]
    public float standThreshold = 0;
    public float crouchThreshold = 1.0f;
    public float midairThreshold = 2.2f;
    public float landingThreshold = 1f;

    [Header("跳跃参数")]
    public float gravity = -9.8f;
    public float maxJumpHeight = 1.5f;
    public float jumpCD = 0.15f;
    public float fallMultiplier = 1.5f;
    public float longJumpMultiplier = 2.5f;
    //角色跌落的最小高度 小于此高度则不会切换到跌落姿态
    public float fallHeight = 0.5f;

    [Header("检测参数")]
    public float groundCheckOffset = 0.5f;

}
