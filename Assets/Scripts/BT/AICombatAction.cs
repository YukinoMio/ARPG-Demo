using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
public class AICombatAction : Action
{
   private MeleeEnemyCombatController meleeEnemyCombatController;
    public override void OnAwake()
    {
        base.OnAwake();
        meleeEnemyCombatController = GetComponent<MeleeEnemyCombatController>();    
    }
}
