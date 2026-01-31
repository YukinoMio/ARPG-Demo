using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem;

public class PlayerInputComponent : PlayerComponentBase
{

    [Header("输入状态")]
    [SerializeField] PlayerController player;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private bool isRunPressed=false;
    [SerializeField] private bool isCrouchPressed=false;
    [SerializeField] private bool isJumpPressed = false;
    [SerializeField] private bool isEquipPressed = false;
    [SerializeField] private bool isCharacterPanelPressed=false;
    [SerializeField] private bool isPackagePanelPressed = false;
    [SerializeField] private bool isAttackPressed = false;

    public Vector2 MoveInput => moveInput;
    public Vector2 GetMoveInput() => moveInput;
    public bool IsRunPressed => isRunPressed;
    public bool IsCrouchPressed => isCrouchPressed;
    public bool IsJumpPressed => isJumpPressed;
    public bool IsEquipPressed { get;  set; }
    public bool IsAttackPressed=>isAttackPressed;   
    //装备相关
    public bool IsKatana { get; private set; }
    public bool IsGreatSword { get; private set; }
    public bool IsBow { get; private set; }

 
    //private void Awake()
    //{
    //    Initialize();
    //}
    #region 玩家输入
    //获取玩家输入
    public void GetMoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
    //获取玩家奔跑状态和空手状态下闪避的输入
    public void GetRunInput(InputAction.CallbackContext ctx)
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null!", this);
            return;
        }

        if (player.StateManager == null)
        {
            Debug.LogError($"StateManager is null on player {player.name}", player);
            return;
        }
        if (ctx.interaction is HoldInteraction)
            isRunPressed = ctx.ReadValueAsButton();
        else if (ctx.interaction is TapInteraction)
        {
           
                player.StateManager.RequestDodge();
        }
    }
    //获取玩家下蹲状态输入
    public void GetCrouchInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isCrouchPressed = !isCrouchPressed;

        }
    }
    public void GetAttackInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isAttackPressed = true;
        } 
    }
    //获取玩家装备武器状态输入
    //TODO: 该方法有待移除
    //public void GetEquipInput(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.started)
    //    {
    //        //isEquipPressed = !isEquipPressed;
    //    }
    //}
    //获取玩家跳跃输入
    public void GetJumpInput(InputAction.CallbackContext ctx)
    {
        isJumpPressed = ctx.ReadValueAsButton();
    }

    public void GetCharacterPanelInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isCharacterPanelPressed == false)
            {
                isCharacterPanelPressed = true;
                UIManager.Instance.OpenPanel("CharacterPanel");
            }
            else
            {
                isCharacterPanelPressed = false;
                UIManager.Instance.ClosePanel("CharacterPanel");
            }
        }
    }

    public void GetPackagePanelInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (isCharacterPanelPressed == false)
            {
                isPackagePanelPressed = true;
                UIManager.Instance.OpenPanel("PackagePanel");
            }
            else
            {
                isPackagePanelPressed = false;
                UIManager.Instance.ClosePanel("PackagePanel");
            }
        }
    }
    #endregion
}
