using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerAnimatorComponent : PlayerComponentBase
{
    [Header("动画组件")]
    [SerializeField] private Animator animator;

    public Animator Animator => animator;
    private int postureHash;
    private int moveSpeedHash;
    private int turnSpeedHash;
    private int verticalSpeedHash;
    private int jumpTypeHash;

    //上一帧的动画nornalized时间
    private float lastFootCycle = 0f;
    //角色急停
    private float currentFootCycle = 0f;

    public override void Initialize(PlayerController playerController)
    {
        base.Initialize(playerController);
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        InitializeAnimatorHashes();
    }

    public override void OnUpdate()
    {
        UpdateAnimationParameters();
        CheckFootstepSound();
    }

    /// <summary>
    /// 获取Animator哈希值
    /// </summary>
    private void InitializeAnimatorHashes()
    {
        postureHash = Animator.StringToHash("Posture");
        moveSpeedHash = Animator.StringToHash("MoveSpeed");
        turnSpeedHash = Animator.StringToHash("TurnSpeed");
        verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        jumpTypeHash = Animator.StringToHash("JumpType");
    }

    private void UpdateAnimationParameters()
    {
        UpdatePostureParameter();
        UpdateMoveSpeedParameter();
        UpdateTurnSpeedParameter();
        UpdateVerticalSpeedParameter();
    }

    //更新姿势参数
    private void UpdatePostureParameter()
    {
        float postureValue;
        switch (player.StateManager.CurrentPosture)
        {
            case PlayerStateManagerComponent.PlayerPosture.Stand:
                postureValue = player.Config.standThreshold;
                break;
            case PlayerStateManagerComponent.PlayerPosture.Crouch:
                postureValue = player.Config.crouchThreshold;
                break;
            case PlayerStateManagerComponent.PlayerPosture.Jumping:
                postureValue = player.Config.midairThreshold;
                break;
            case PlayerStateManagerComponent.PlayerPosture.Falling:
                postureValue = player.Config.midairThreshold;
                break;
            case PlayerStateManagerComponent.PlayerPosture.Landing:
                postureValue = player.Config.landingThreshold;
                break;
            default:
                postureValue = player.Config.standThreshold;
                break;
        }
        animator.SetFloat(postureHash, postureValue, 0.1f, Time.deltaTime);
    }

    /// <summary>
    /// 更新移动速度参数
    /// </summary>
    private void UpdateMoveSpeedParameter()
    {
        float moveSpeedValue = 0f;
        switch (player.StateManager.CurrentPosture)
        {
            case PlayerStateManagerComponent.PlayerPosture.Stand:
                moveSpeedValue = GetStandMoveSpeed();
                break;
            case PlayerStateManagerComponent.PlayerPosture.Crouch:
                moveSpeedValue = GetCrouchMoveSpeed();
                break;
            case PlayerStateManagerComponent.PlayerPosture.Landing:
                moveSpeedValue = GetLandingMoveSpeed();
                break;
        }
        animator.SetFloat(moveSpeedHash, moveSpeedValue, 0.1f, Time.deltaTime);
    }

    //获取站立状态的速度
    private float GetStandMoveSpeed()
    {
        return player.StateManager.CurrentStandMotion switch
        {
            PlayerStateManagerComponent.StandMotion.Idle => 0f,
            PlayerStateManagerComponent.StandMotion.Run => player.InputHandler.MoveInput.magnitude * player.Config.runSpeed,
            PlayerStateManagerComponent.StandMotion.Walk => player.InputHandler.MoveInput.magnitude * player.Config.walkSpeed,
            _ => 0f
        };
    }


    //获取蹲状态的速度
    private float GetCrouchMoveSpeed()
    {
        return player.StateManager.CurrentStandMotion == PlayerStateManagerComponent.StandMotion.Idle ?
            0f : player.InputHandler.MoveInput.magnitude * player.Config.crouchSpeed;
    }


    //获取着陆速度
    private float GetLandingMoveSpeed()
    {
        return player.StateManager.CurrentStandMotion switch
        {
            PlayerStateManagerComponent.StandMotion.Idle => 0f,
            PlayerStateManagerComponent.StandMotion.Walk => player.Movement.PlayerMovement.magnitude * player.Config.walkSpeed,
            PlayerStateManagerComponent.StandMotion.Run => player.Movement.PlayerMovement.magnitude * player.Config.runSpeed,
            _ => 0f
        };
    }

    /// <summary>
    /// 更新转向速度
    /// </summary>
    private void UpdateTurnSpeedParameter()
    {
        if (player.Movement.PlayerMovement.magnitude > 0.1f)
        {
            float rad = Mathf.Atan2(player.Movement.PlayerMovement.x, player.Movement.PlayerMovement.z);
            animator.SetFloat(turnSpeedHash, rad * 1.3f, 0.1f, Time.deltaTime);
            // 平滑旋转到移动方向
            player.transform.Rotate(0, rad * 240 * Time.deltaTime, 0);
        }
        else
        {
            animator.SetFloat(turnSpeedHash, 0f, 0.1f, Time.deltaTime);
        }
    }

    /// <summary>
    /// 更新垂直方向速度
    /// </summary>
    private void UpdateVerticalSpeedParameter()
    {
        if (player.StateManager.CurrentPosture == PlayerStateManagerComponent.PlayerPosture.Jumping ||
            player.StateManager.CurrentPosture == PlayerStateManagerComponent.PlayerPosture.Falling)
        {
            animator.SetFloat(verticalSpeedHash, player.Physics.VerticalVelocity, 0.1f, Time.deltaTime);
        }
    }

    private void CheckFootstepSound()
    {
        if (player.StateManager.CurrentPosture != PlayerStateManagerComponent.PlayerPosture.Jumping &&
            player.StateManager.CurrentPosture != PlayerStateManagerComponent.PlayerPosture.Falling)
        {
            if (player.StateManager.CurrentStandMotion == PlayerStateManagerComponent.StandMotion.Walk ||
                player.StateManager.CurrentStandMotion == PlayerStateManagerComponent.StandMotion.Run)
            {
                currentFootCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);

                if ((lastFootCycle < 0.1f && currentFootCycle >= 0.1f) ||
                    (lastFootCycle < 0.6f && currentFootCycle >= 0.6f))
                {
                    player.SoundController.PlayFootStepSound();
                }

                lastFootCycle = currentFootCycle;
            }
        }
    }

    public void SetVerticalSpeedImmediate(float velocity)
    {
        animator.SetFloat(verticalSpeedHash, velocity);
    }

    public void SetJumpType(float jumpType)
    {
        animator.SetFloat(jumpTypeHash, jumpType);
    }

    public void SetLandingThreshold(float threshold)
    {
        // 在UpdatePostureParameter中处理
    }

    public void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }
}
