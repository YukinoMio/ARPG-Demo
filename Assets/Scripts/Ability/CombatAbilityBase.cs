using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatAbilityBase : ScriptableObject
{
    [SerializeField] protected string abilityName;
    [SerializeField] protected int abilityID;
    [SerializeField] protected float abilityCD;
    [SerializeField] protected float abilityUseDistance;
    [SerializeField] protected bool abilityIsAvailable;

    #region 组件
    protected Animator animator;
    protected EnemyCombatController combatController;
    protected EnemyMovementController enemyMovementController;
    protected EnemyBase enemyParameter;

    #endregion

    #region 动画状态机哈希值
    protected int verticalHash = Animator.StringToHash("Vertical");
    protected int horizontalHash = Animator.StringToHash("Horizontal");
    protected int moveSpeedHash = Animator.StringToHash("MoveSpeed");
    #endregion

    /// <summary>
    /// 调用技能
    /// </summary>
    public abstract void InvokeAbility();

    /// <summary>
    /// 使用技能
    /// </summary>
    protected void UseAbility()
    {
        //动画机中播放技能动画
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Motion"))
        {
            animator.CrossFade(abilityName, 0.1f);
        }
        abilityIsAvailable = false;
        //将自己从可用技能列表中移除
        combatController.availableAbilityList.Remove(this);
        //技能CD
        AbilityCoolDown();
    }

    /// <summary>
    /// 技能CD
    /// </summary>
    public void AbilityCoolDown()
    {
        Timer timer = CachePoolManager.Instance.GetObject("Tool/Timer").GetComponent<Timer>();
        timer.CreateTime(abilityCD, () =>
        {
            //CD结束
            abilityIsAvailable = true;    //技能设置为可用
            combatController.availableAbilityList.Add(this);
        });
    }
    #region 公共接口

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="combatController"></param>
    /// <param name="enemyMovementController"></param>
    /// <param name="enemyParameter"></param>
    public void Init(Animator animator, EnemyCombatController combatController,
      EnemyMovementController enemyMovementController, EnemyBase enemyParameter)
    {
        this.animator = animator;
        this.combatController = combatController;
        this.enemyMovementController = enemyMovementController;
        this.enemyParameter = enemyParameter;
    }

    public string GetAbilityName() => abilityName;
    public int GetAbilityID() => abilityID;
    public float GetAbilityCD() => abilityCD;

    public float GetAbilityUseDistance() => abilityUseDistance;
    public bool GetAbilityAvailable() => abilityIsAvailable;
    public void SetAbilityAvailable(bool isDone) { abilityIsAvailable = isDone; }   
    #endregion

}
