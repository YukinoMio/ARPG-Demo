using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyCombatController : CombatControllerBase
{
    //AI的攻击指令是由AI管理器指派的，非AI自身的行为
    //AI在收到指令，还需要判断自身的情况 ，是否能接受这个指令
    //比如玩家不希望ai去接受，在处决的时候

    [SerializeField] private bool attackCommand;//攻击指令
    private bool canAttackInput;

    /// <summary>
    /// 检查当前AI的自身状态是否允许接受攻击指令
    /// </summary>
    /// <returns></returns>
    private bool CheckAIState()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Hit")) return false;
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Parry")) return false;

        return true;
    }


    //获取AI的攻击指令状态
    public bool GetAttackCommand() => attackCommand;


    //设置指令
    public void SetAttackCommand(bool command)
    {
        attackCommand = command;
        if(!CheckAIState())
        {
            attackCommand = false;
            return;
        }
        if(command)
        {
            ExcuteCombo();
        }
    }


}
