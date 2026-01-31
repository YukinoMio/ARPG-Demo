using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterPanel : UIPanelBase
{
    private Transform UIExitBtn;
    private Transform UICharactereName;
    //private Transform UIStars;
    private Transform UILevel;
    private Transform UIMaxHp;
    private Transform UIAttack;
    private Transform UIDefense;
    private Transform UICritical;
    private Transform UICriticalDamage;
    private Transform UILevelUpBtn;

    private Text levelText;
    private Text maxHpText;
    private Text attackText;
    private Text defenseText;
    private Text criticalText;
    private Text criticalDamageText;

    private PlayerController player;
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        InitUI();
        InitClick();
        //RefreshAllData();
        EventCenter.Instance.Subscribe<AttributeEvents.AttributeChanged>(OnAttributeChanged);
        EventCenter.Instance.Subscribe<LevelEvents.LevelChanged>(OnLevelChanged);
    }
    public override void OpenPanel()
    {
        base.OpenPanel();
        RefreshAllData();
    }
    private void InitUI()
    {
        InitUIName();
    }
    private void InitUIName()
    {
        UIExitBtn = transform.Find("Top/Exit");
        UICharactereName = transform.Find("MiddleRight/Name/Text");
        //UIStars = transform.Find("MiddleRight/Stars/Text");
        UILevel = transform.Find("MiddleRight/Level/CurLevel");
        UIMaxHp = transform.Find("MiddleRight/MaxHp/Hp");
        UIAttack = transform.Find("MiddleRight/Attack/CurAttack");
        UIDefense = transform.Find("MiddleRight/Defense/CurDenfense");
        UICritical = transform.Find("MiddleRight/Critical/CurCritical");
        UICriticalDamage=transform.Find("MiddleRight/CriticalDamage/CurCriticalDamage");
        UILevelUpBtn = transform.Find("MiddleRight/LevelUp");

        // 添加安全检查
        if (UIExitBtn == null) Debug.LogError("找不到 UIExitBtn: Top/Exit");
        if (UICharactereName == null) Debug.LogError("找不到 UICharactereName: MiddleRight/Name/Text");
        //if (UIStars == null) Debug.LogError("找不到 UIStars: MiddleRight/Stars/Text");
        if (UILevel == null) Debug.LogError("找不到 UILevel: MiddleRight/Level/Text");
        if (UIMaxHp == null) Debug.LogError("找不到 UIMaxHp: MiddleRight/MaxHp/Hp");
        if (UIAttack == null) Debug.LogError("找不到 UIAttack: MiddleRight/Attack/CurAttack");
        if (UIDefense == null) Debug.LogError("找不到 UIDefense: MiddleRight/Defense/CurDefense");
        if (UICritical == null) Debug.LogError("找不到 UICritical: MiddleRight/Critical/CurCritical");
        if (UICriticalDamage == null) Debug.LogError("找不到 UICriticalDamage: MiddleRight/Critical/CurCriticalDamage");
        if (UILevelUpBtn == null) Debug.LogError("找不到 UILevelUpBtn: MiddleRight/LevelUp");


        levelText = UILevel.GetComponent<Text>();
        maxHpText = UIMaxHp.GetComponent<Text>();
        attackText = UIAttack.GetComponent<Text>();
        defenseText = UIDefense.GetComponent<Text>();
        criticalText = UICritical.GetComponent<Text>();
        criticalDamageText = UICriticalDamage.GetComponent<Text>();
    }

    private void InitClick()
    {
        UIExitBtn.GetComponent<Button>().onClick.AddListener(OnClickExit);
        UILevelUpBtn.GetComponent<Button>().onClick.AddListener(OnClickLevelUp);
    }


    private void OnClickExit()
    {
        // 关闭角色面板
        UIManager.Instance.ClosePanel("CharacterPanel");
    }

    private void OnClickLevelUp()
    {
        //发布升级请求事件  触发升级请求事件
        EventCenter.Instance.Publish(new LevelEvents.LevelUpRequested
        {
            CharacterId = player.Config.characterName
        });
        Debug.Log("发送升级请求");
    }

    /// <summary>
    /// 属性变化事件处理
    /// </summary>
    /// <param name="e"></param>
    public override void OnAttributeChanged(AttributeEvents.AttributeChanged e)
    {
        switch (e.AttributeType)
        {
            case AttributeType.MaxHp:
                maxHpText.text =e.NewValue.ToString("F0");
                break;
            case AttributeType.Attack:
                attackText.text = e.NewValue.ToString("F0");
                break;
            case AttributeType.Defense:
                defenseText.text = e.NewValue.ToString("F0");
                break;
            case AttributeType.CritRate:
                criticalText.text = e.NewValue.ToString("F2");
                break;
            case AttributeType.CritDamage:
                criticalDamageText.text = e.NewValue.ToString("F2");
                break;
            default:
                break;
        }
    }


    public virtual void OnLevelChanged(LevelEvents.LevelChanged e)
    {
        levelText.text = e.NewLevel.ToString("F0");
         // 显示属性变化
        foreach (var change in e.AttributeChanges)
        {
            Debug.Log(change.Key + " 增加了 " + change.Value);
        }
        //播放升级音效
        PlayerLevelUpAudio();
    }


    /// <summary>
    /// 刷新所有数据
    /// </summary>
    public override void Refresh()
    {
        RefreshAllData();
    }
    
    private void RefreshAllData()
    {
        if (player?.AdvancedAttributeSystem == null) return;
        float maxHp = player.AdvancedAttributeSystem.GetAttributeValue(AttributeType.MaxHp);
        float attack = player.AdvancedAttributeSystem.GetAttributeValue(AttributeType.Attack);
        float defense = player.AdvancedAttributeSystem.GetAttributeValue(AttributeType.Defense);
        float critRate = player.AdvancedAttributeSystem.GetAttributeValue(AttributeType.CritRate);
        float critDamage= player.AdvancedAttributeSystem.GetAttributeValue(AttributeType.CritDamage);
        // 更新UI显示
        maxHpText.text = maxHp.ToString("F0");
        attackText.text = attack.ToString("F0");
        defenseText.text = defense.ToString("F0");
        criticalText.text = critRate .ToString("F0");
        criticalDamageText.text=critDamage.ToString("F0");
        int currentLevel = GetCurrentLevel();
        if (levelText != null)
            levelText.text = currentLevel.ToString();
    }
    
    //获取当前等级
    private int GetCurrentLevel()
    {
        if (player?.LevelUpProcessor != null)
        {
            return player.LevelUpProcessor.GetCurrentLevel();
        }
        return 1;
    }





    /// <summary>
    /// 升级音效
    /// </summary>
   private void PlayerLevelUpAudio()
    {
        Debug.Log("播放升级音效");
    }
}
