using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Random = UnityEngine.Random;
using INab.Demo;


[Serializable]
public struct ComboDicStruct
{
    public WeaponType weaponType;
    public ComboList comboList;
}
public class PlayerCombatController : CombatControllerBase
{
    #region 组件
    private CharacterController controller;
    private ThirdPersonController thirdPersonController;
    private AttackCheckGizmos attackCheck;

    private InputAction movementInputAction;
    private InputAction attackAction;
    #endregion

    public float attack = 1f;
    private AttackType attackType = AttackType.Common;
    public WeaponType weaponType = WeaponType.Empty;
    [SerializeField] private ComboDicStruct[] comboDicStructs;
    private Dictionary<WeaponType, ComboList> comboListDict;
    [SerializeField] private bool canPlayHitAnim;
    [Header("无敌帧")]
    [SerializeField] private int invincibleFrame;
    private int countInvincibleFrame;
    private bool startToCountInvincibleFrame;
    [Header("完美闪避")]
    [SerializeField] private float perfecDodgeTime;
    [SerializeField] private float canPerfectDodgeTime;
    [SerializeField][Range(0f, 1f)] private float perfectDodgeTimeScale;
    [SerializeField] public bool isPerfectDodging;
    [SerializeField] private string perfectDodgeAudioClipPath;
    private int rollHash;

    private void Awake()
    {
        base.Awake();   
        comboListDict = new Dictionary<WeaponType, ComboList>();
        foreach (ComboDicStruct comboDict in comboDicStructs)
        {
            comboListDict.Add(comboDict.weaponType, comboDict.comboList);
        }
    }
    void Start()
    {
        base.Start();
        controller = GetComponent<CharacterController>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        attackCheck = GetComponent<AttackCheckGizmos>();
        movementInputAction = GetComponent<PlayerInput>().actions["PlayerMovement"];
        attackAction = GetComponent<PlayerInput>().actions["Attack"];

        rollHash = Animator.StringToHash("Roll");
        canPlayHitAnim = true;
        startToCountInvincibleFrame = false;
        countInvincibleFrame = invincibleFrame;//初始化无敌帧
        isPerfectDodging = false;
        // 确保武器类型和连招表
        if (weaponType != WeaponType.Empty)
        {
            SwitchComboList(weaponType);
        }
        else
        {
            Debug.LogWarning("武器类型为空，请设置weaponType");
        }

        // 检查关键组件
        if (currentComboList == null)
        {
            Debug.LogError("currentComboList未设置！请在Inspector中配置comboDicStructs");
        }
        if (attackCheckSystem == null)
        {
            Debug.LogError("未找到AttackCheckGizmos组件！");
        }
    }

    private void Update()
    {
        //Debug.Log(weaponType.ToString());
       // Debug.Log(canBeHit.ToString());
        base.Update();
    }
    private void FixedUpdate()
    {
        CountInvincibleFrame();
    }

    public void SwitchComboList(WeaponType _weaponType)
    {
        if (!comboListDict.ContainsKey(_weaponType))
        {
            return;
        }
        currentComboList = comboListDict[_weaponType];
    }

    //玩家受击
    public void PlayerOnHit(EnemyAttackDetectionConfig attackConfig, Transform attackerTransform)
    {
        if (!canBeHit)
            return;
        canBeHit = false;
        Debug.Log($"Player受击！武器类型: {weaponType}");
        //停止攻击检测（防止攻击被打断时攻击检测一直开启）
        attackCheckSystem.EndAttacking();

        //禁用玩家移动和攻击输入
        movementInputAction.Disable();
        attackAction.Disable();

        int damage = attackConfig.damage + Random.Range(-10, 10);
        //TODO: 扣除生命值等逻辑
        Debug.Log("玩家受到了" + damage + "点伤害!");

        //播放切换装备动画、大剑的攻击动画时不播放受击动画（大剑攻击有硬直）
        if (canPlayHitAnim && !animator.GetCurrentAnimatorStateInfo(0).IsTag("GSAttack") &&
            !animator.GetCurrentAnimatorStateInfo(0).IsTag("Equip"))
        {
            Vector3 dir = (attackerTransform.position - this.transform.position).normalized;

            // 计算与前方和右侧的夹角
            float angleForward = Vector3.Angle(dir, transform.forward);
            float angleRight = Vector3.Angle(dir, transform.right);

            // 判断方位
            if (angleForward <= 45f) // 前方90度范围内
            {
                animator.Play("Hit_Front_" + weaponType.ToString());
            }
            else if (angleForward >= 135f) // 后方90度范围内
            {
                animator.Play("Hit_Back_" + weaponType.ToString());
            }
            else if (angleRight <= 45f) // 右侧90度范围内
            {
                animator.Play("Hit_Right_" + weaponType.ToString());
            }
            else if (angleRight >= 135f) // 左侧90度范围内
            {
                animator.Play("Hit_Left_" + weaponType.ToString());
            }
        }

        //生成受击特效
        string hitFXName = hitFXList[0].TryGetHitFXName();
        FXManager.Instance.PlayOneHitFX(hitFXName, hitTransform.position, hitFXScale);

        //无敌时间计时
        StartCoroutine(IE_HitCoolDown(hitCoolDown));
    }

    //受击无敌保护
    private IEnumerator IE_HitCoolDown(float coolDownTime)
    {
        while (coolDownTime > 0)
        {
            yield return null;
            coolDownTime -= Time.deltaTime;
        }
        canBeHit = true;
        //启用玩家移动和攻击输入
        movementInputAction.Enable();
        attackAction.Enable();
    }

    //计算无敌帧
    private void CountInvincibleFrame()
    {
        if (startToCountInvincibleFrame)
        {
            if (countInvincibleFrame > 0)
            {
                countInvincibleFrame--;
                if (countInvincibleFrame <= 0)
                {
                    canBeHit = true;//无敌结束可以被攻击
                    startToCountInvincibleFrame = false;//停止计算无敌帧
                    countInvincibleFrame = invincibleFrame;//重置无敌帧
                }
            }
        }
    }

    public void PerfectDodge()
    {
        if (isPerfectDodging || canBeHit || !startToCountInvincibleFrame) return;
        isPerfectDodging = true;
        Time.timeScale = perfectDodgeTimeScale;
        //播放完美闪避音效
        audioSource.PlayOneShot(Resources.Load<AudioClip>(perfectDodgeAudioClipPath), 0.5f);

        StartCoroutine(IE_CountPerfectDodge(perfecDodgeTime));
    }
    //控制完美闪避生效时间
    IEnumerator IE_CountPerfectDodge(float duration)
    {
        yield return new WaitForSeconds(duration);
        isPerfectDodging = false;
        Debug.Log("完美闪避结束");
        Time.timeScale = 1f;
    }

    #region 公共接口
    public float GetCanPerfectDodgeTime() => canPerfectDodgeTime;
    #endregion
    public void StartInvincibleFrame()
    {
        canBeHit = false;
        startToCountInvincibleFrame = true;
    }
    #region 玩家输入相关
    public void GetAttackInput(InputAction.CallbackContext context)
    {
        //Debug.Log(weaponType.ToString());
        if (context.started && weaponType != WeaponType.Empty)
        {
            ExcuteCombo();
        }
    }

    //获取玩家闪避输入
    public void GetSlideInput(InputAction.CallbackContext context)
    {
        Debug.Log($"canExecuteCombo: {canExcuteCombo}"); // 检查变量值
        if (context.interaction is TapInteraction && canExcuteCombo)
        {
            Debug.Log("Trigger Roll"); // 确认条件通过
            animator.SetTrigger(rollHash);
        }
    }
    #endregion
}

