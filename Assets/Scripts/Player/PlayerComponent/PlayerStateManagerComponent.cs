using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateManagerComponent : PlayerComponentBase
{
    [Header("角色状态")]
    [SerializeField] private PlayerPosture currentPosture = PlayerPosture.Stand;
    [SerializeField] private StandMotion currentStandMotion = StandMotion.Idle;
    [SerializeField] private ArmState currentArmState = ArmState.Normal;

    public PlayerPosture CurrentPosture => currentPosture;
    public StandMotion CurrentStandMotion => currentStandMotion;
    public ArmState CurrentArmState => currentArmState;

    //背上的武器
    public GameObject weaponOnBack;
    //手里的武器
    public GameObject weaponInHand;
    //角色姿态 蹲 站 降落 跳 着陆
    public enum PlayerPosture { Crouch, Stand, Falling, Jumping, Landing }
    //站立时的动作  待机 走 跑 
    public enum StandMotion { Idle, Walk, Run }
    //是否装备武器
    public enum ArmState { Normal = 0, Equip = 1 }


    public override void OnUpdate()
    {
        UpdatePostureState();
        UpdateStandMotionState();
    }

    private void UpdatePostureState()
    {
        switch (currentPosture)
        {
            case PlayerPosture.Stand:
                if (player.Physics.VerticalVelocity > 0)
                    currentPosture = PlayerPosture.Jumping;
                else if (!player.GroundCheck.IsGrounded && player.GroundCheck.CouldFall)//垂直速度小于0，不在地面上且可以 降落
                    currentPosture = PlayerPosture.Falling;
                else if(player.InputHandler.IsCrouchPressed)
                    currentPosture= PlayerPosture.Crouch;
                break;
            case PlayerPosture.Crouch:
                if (!player.GroundCheck.IsGrounded && player.GroundCheck.CouldFall)
                    currentPosture = PlayerPosture.Falling;
                else if (!player.InputHandler.IsCrouchPressed)
                    currentPosture = PlayerPosture.Stand;
                break;
            case PlayerPosture.Falling:
                if (player.Physics.VerticalVelocity <= player.Physics.LandingMinVelocity && player.GroundCheck.IsGrounded)
                    player.Physics.StartLandingCooldown();//TODO:为什么降落也要 开启协程
                if (player.Physics.IsLanding)
                    currentPosture = PlayerPosture.Landing;
                break;
            case PlayerPosture.Jumping:
                if (player.Physics.VerticalVelocity < 0 && player.GroundCheck.IsGrounded)
                    player.Physics.StartLandingCooldown();
                if (player.Physics.IsLanding)
                    currentPosture = PlayerPosture.Landing;
                break;
            case PlayerPosture.Landing:
                if (!player.Physics.IsLanding)
                    currentPosture = PlayerPosture.Stand;
                break;
            default:
                break;
        }
    }


    private void UpdateStandMotionState()
    {
        if (player.InputHandler.MoveInput.magnitude == 0)
            currentStandMotion = StandMotion.Idle;
        else if (player.InputHandler.IsRunPressed)
            currentStandMotion = StandMotion.Run;
        else
            currentStandMotion = StandMotion.Walk;
    }

    //判断是否闪避
    public void RequestDodge()
    {
        if (currentArmState == ArmState.Normal && player.GroundCheck.IsGrounded)
        {
            player.Animator.SetTrigger("Roll");
        }
    }

    public void SwitchArmState(ArmState newState)
    {
        currentArmState = newState;
        UpdateWeaponDisplay();
    }
    private void UpdateWeaponDisplay()
    {
        if (weaponOnBack != null && weaponInHand != null)
        {
            weaponOnBack.SetActive(currentArmState == ArmState.Normal);
            weaponInHand.SetActive(currentArmState == ArmState.Equip);
        }
    }
}
