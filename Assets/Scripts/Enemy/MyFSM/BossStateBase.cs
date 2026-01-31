using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateBase : IState
{
    private BossFSM fsm;
    //private Boss boss;
    //³õÊ¼»¯º¯Êý
    public void Init(BossFSM bossFSM)
    {
        fsm = bossFSM;
    }
    public virtual void OnEnter()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnExit()
    {
    }
}
