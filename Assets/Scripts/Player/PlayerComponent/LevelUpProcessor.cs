using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelUpProcessor 
{ 
   private PlayerController player;
    private AdvancedAttributeSystem attributeSystem;
    private int currentLevel=1;

    public  LevelUpProcessor(PlayerController playerController, AdvancedAttributeSystem attributeSystem)
    {
        this.player = playerController;
        this.attributeSystem = attributeSystem;
        //订阅（添加监听）
       EventCenter.Instance.Subscribe<LevelEvents.LevelUpRequested>(OnLevelUpRequested);
        Debug.Log("LevelUpProcessor 已初始化");
    }

    private void OnLevelUpRequested(LevelEvents.LevelUpRequested e)
    {
        if (e.CharacterId != player.Config.characterName)
        {
            return;
        }
        Debug.Log($"开始处理 {e.CharacterId} 的升级");
        //1.计算新等级
        int newLevel = currentLevel + 1;
        //2.计算属性增长
        Dictionary<AttributeType, float> attributeChanges = CalculateLevelUpBenefits();
        //3.应用属性修改
        ApplyAttributeChanges(attributeChanges);
        //4.更新等级
        currentLevel=newLevel;
        //5.发布等级变化事件
        EventCenter.Instance.Publish(new LevelEvents.LevelChanged
        {
            CharacterId = e.CharacterId,
            OldLevel = currentLevel - 1,
            NewLevel = newLevel,
            AttributeChanges = attributeChanges
        });
        Debug.Log($"升级完成: Lv{currentLevel - 1} → Lv{currentLevel}");
    }
    private Dictionary<AttributeType,float> CalculateLevelUpBenefits()
    {
        var changes=new Dictionary<AttributeType, float>();

        float levelMultiplier = 0.1f;//10%增长
                                     //获取当前基础值
        float baseMaxHp = attributeSystem.GetBaseValue(AttributeType.MaxHp);
        float baseAttack=attributeSystem.GetBaseValue(AttributeType.Attack);
        float baseDenfense = attributeSystem.GetBaseValue(AttributeType.Defense);
        float baseCriRate = attributeSystem.GetBaseValue(AttributeType.CritRate);
        float baseCritDamage = attributeSystem.GetBaseValue(AttributeType.CritDamage);

        changes[AttributeType.MaxHp] = baseMaxHp * levelMultiplier;
        changes[AttributeType.Attack] = baseAttack * levelMultiplier;
        changes[AttributeType.Defense] = baseDenfense * levelMultiplier;
        changes[AttributeType.CritRate] = 1f;
        changes[AttributeType.CritDamage]= 10f;
        return changes;


    }
    private void ApplyAttributeChanges(Dictionary<AttributeType, float> changes)
    {
        foreach(var change in changes)
        {
            //修改数据
            var modify = new DataModify
            {
                attributeType = change.Key,
                modify = change.Value,
                dataModifyType = DataModifyType.AdditionalValue,
                operateType = DataOperateType.Addition
            };
            attributeSystem.ApplyModify(modify, "LevelUp");
        }
       
    }

    public void Dispose()
    {
        EventCenter.Instance.Unsubscribe<LevelEvents.LevelUpRequested>(OnLevelUpRequested);
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}
