using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPhysicsComponent : PlayerComponentBase
{
    [Header("物理状态")]
    // //角色垂直方向速度
    [SerializeField] private float verticalVelocity;
    //是否落地
    [SerializeField] private bool isLanding=false;

    public float VerticalVelocity => verticalVelocity;
    public bool IsLanding => isLanding;
    public float LandingThreshold { get; private set; }
    ////角色跌落时 能够触发Landing状态的最小速度
    public float LandingMinVelocity { get; private set; }

    //角色跳跃时的左右脚
    private float footTween;

    public override void Initialize(PlayerController playerController)
    {
        base.Initialize(playerController);
        //降落最小速度
        LandingMinVelocity = -Mathf.Sqrt(-2 * player.Config.gravity * player.Config.fallHeight);
        LandingMinVelocity -= 1f;
    }

    public override void OnUpdate()
    {
       CalculateGravity();
        HandleJump();
    }

    //计算重力
    private void CalculateGravity()
    {
        var state = player.StateManager.CurrentPosture;
        bool isGrounded = player.GroundCheck.IsGrounded;
        bool isJumpPressed = player.InputHandler.IsJumpPressed;
        if (state != PlayerStateManagerComponent.PlayerPosture.Jumping && state != PlayerStateManagerComponent.PlayerPosture.Falling)
        {
            // 不跳跃 降落 也不
            if (!isGrounded)
            {
                verticalVelocity += player.Config.gravity * player.Config.fallMultiplier * Time.deltaTime;
            }
            else
            {
                //CharacterController的isGrouond要求角色必须持续有向下的速度
                //站在地面上时 速度不会累加
                verticalVelocity = player.Config.gravity*Time.deltaTime;
            }
        }
        //在跳跃和降落姿态
        else
        {
            //重力加速度
            if(verticalVelocity<=0f)
            {
                verticalVelocity += player.Config.gravity * player.Config.fallMultiplier * Time.deltaTime;
            }
            //上升时
            else
            {
                if(isJumpPressed)
                {
                    verticalVelocity += player.Config.gravity * Time.deltaTime;
                }
                else
                {
                    verticalVelocity += player.Config.gravity * player.Config.longJumpMultiplier * Time.deltaTime;
                }
            }
        }
    }


    //处理跳跃
    private void HandleJump()
    {
        var posture =player.StateManager.CurrentPosture;
        var isJumpPressed=player.InputHandler.IsJumpPressed;
        if((posture==PlayerStateManagerComponent.PlayerPosture.Stand||
            posture==PlayerStateManagerComponent.PlayerPosture.Crouch)&&
            isJumpPressed&&verticalVelocity<2f)// 竖直速度 < 2f 防止连跳和斜坡有微小抖动就跳
        {
            //根据跳跃的最大高度计算初速度
            verticalVelocity = Mathf.Sqrt(-2 * player.Config.gravity * player.Config.maxJumpHeight);
            player.Animator.SetVerticalSpeedImmediate(verticalVelocity);
            //控制左右脚
            footTween = Random.value > 0.5f ? 1f : -1f;
            player.Animator.SetJumpType(footTween);
        }
    }

    //着陆的时候开启冷却协程，
    public void StartLandingCooldown()
    {
        //是否处于着陆缓冲状态
        if(!isLanding)
        StartCoroutine(CoolDownJump());
    }

    //跳跃冷却协程
    private IEnumerator CoolDownJump()
    {
        //根据下落速度来计算落地后跳跃CD状态的阈值，以此设置下蹲动画状态的程度
        //去掉小于-10和大于0的速度(任何小于-10的值会被设为-10,任何大于0的值会被设为0)
        isLanding = true;

        LandingThreshold = Mathf.Clamp(verticalVelocity, -10, 0);
        //限制landingThreshold在[-0.5,0]
        LandingThreshold /= 20f;
        LandingThreshold += 0.5f;
        player.Animator.SetLandingThreshold(LandingThreshold);
        yield return new WaitForSeconds(player.Config.jumpCD);
        isLanding = false;
    }

    //设置竖直速度
    public void SetVerticalVelocity(float velocity)
    {
        verticalVelocity = velocity;
    }
}
