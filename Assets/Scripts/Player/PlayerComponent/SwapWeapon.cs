using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class SwapWeapon :MonoBehaviour
{
    #region 组件
    private Animator animator;
    private PlayerController playerController;
    public Transform effectTransform;
    public TwoBoneIKConstraint[] rightHandIKConstraints;
    private TwoBoneIKConstraint currentRightHandIKConstraint;
    public TwoBoneIKConstraint[] leftHandIKConstraints;
    private TwoBoneIKConstraint currentLeftHandIKConstraint;
    private AttackCheckGizmos attackCheck;
    private PlayerCombatController playerCombatController;
    #endregion

    private AttackType attackType = AttackType.Common;
    private WeaponType weaponType = WeaponType.Empty;
    public WeaponType WeaponType => weaponType;
    public GameObject[] weaponOnBack;
    public GameObject[] weaponInHand;

    private int equipHash;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        currentRightHandIKConstraint = rightHandIKConstraints[0];
        currentLeftHandIKConstraint = leftHandIKConstraints[0];
        attackCheck=GetComponent<AttackCheckGizmos>();
        playerCombatController=GetComponent<PlayerCombatController>(); 
        animator=GetComponent<Animator>();
        equipHash = Animator.StringToHash("WeaponType");
    }

   
    public void Update()
    {
        SetAnimator();
        //Debug.Log(weaponType.ToString());
    }

    private void SetAnimator()
    {
        //装备状态
        animator.SetInteger(equipHash, (int)weaponType);
        //控制掏出武器和收起武器的右手IK权重
        currentRightHandIKConstraint.weight = animator.GetFloat("Right Hand Weight");
        currentLeftHandIKConstraint.weight = animator.GetFloat("Left Hand Weight");
    }

    #region 动画片段调用函数
    /// <summary>
    /// 切换背部武器和手部武器的显示
    /// </summary>
    /// <param name="weaponType">表示武器的位置是在背上0还是1\2\3</param>
    public void PutGrabWeapon(int weaponType)
    {
        //isOnBack为true时是装备武器 为false时是收回武器
        bool isOnBack = weaponOnBack[weaponType].activeSelf;
        weaponOnBack[weaponType].SetActive(!isOnBack);
        weaponInHand[weaponType].SetActive(isOnBack);
    }
    #endregion

    #region 玩家输入相关
    private bool IsInputValid()
    {
        if (playerController.StateManager.CurrentArmState != PlayerStateManagerComponent.ArmState.Equip)
        {
            return false;
        }
        return true;
    }

    //获取玩家武器装备输入
    public void GetKatanaInput(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
        {
            //若当前手上没有武器
            if(weaponType==WeaponType.Empty)
            {
                weaponType = WeaponType.Katana;
                playerController.InputHandler.IsEquipPressed = true;
                //将当前有效的IK约束设置为Katana的IK约束
                currentRightHandIKConstraint = rightHandIKConstraints[(int) WeaponType.Katana];
                currentLeftHandIKConstraint = leftHandIKConstraints[(int) WeaponType.Katana];   
            }
            //若手上有武器则收回该武器
            else
            {
                weaponType = WeaponType.Empty;
                playerController.InputHandler.IsEquipPressed = false;
            }
            attackCheck.weaponType=weaponType;
            playerCombatController.weaponType=weaponType;
            //切换连招表
            playerCombatController.SwitchComboList(weaponType);
        }
    }

    public void GetGreatSwordInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (weaponType == WeaponType.Empty)
            {
                weaponType = WeaponType.GreatSword;
                playerController.InputHandler.IsEquipPressed = true;
                //将当前有效的IK约束设置为Katana的IK约束
                currentRightHandIKConstraint = rightHandIKConstraints[(int)WeaponType.Katana];
                currentLeftHandIKConstraint = leftHandIKConstraints[(int)WeaponType.Katana];
            }
            else
            {
                weaponType=WeaponType.Empty;
                playerController.InputHandler.IsEquipPressed = false;

            }
            attackCheck.weaponType = weaponType;
            //切换连招表
            playerCombatController.SwitchComboList(weaponType) ;
        }
    }

    public void GetBowInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (weaponType == WeaponType.Empty)
            {
                weaponType = WeaponType.Bow;
                playerController.InputHandler.IsEquipPressed = true;
                //将当前有效的IK约束设置为Bow的IK约束
                currentRightHandIKConstraint = rightHandIKConstraints[(int)WeaponType.Bow];
                currentLeftHandIKConstraint = leftHandIKConstraints[(int)WeaponType.Bow];
            }
            else
            {
                weaponType = WeaponType.Empty;
                playerController.InputHandler.IsEquipPressed = false;
            }
            attackCheck.weaponType = weaponType;

            playerCombatController.weaponType = weaponType;
            //切换连招表
            playerCombatController.SwitchComboList(weaponType);
        }
    }
    #endregion
}
