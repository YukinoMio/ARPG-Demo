using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;


//招式 控制基类 根据索引找对应动画
public class CombatControllerBase : MonoBehaviour
{

    protected AttackCheckGizmos attackCheckSystem;
    protected AudioSource audioSource;
    protected Animator animator;
    [SerializeField] protected ComboList currentComboList;

    protected int currentComboIndex = 0;
    protected int nextComboIndex = 0;
    protected bool canExcuteCombo;
    protected RunningEventIndex runningEventIndex;
    [SerializeField] protected float multiplier = 1.2f;
    [SerializeField] protected bool canBeHit;
    [SerializeField] protected float hitCoolDown = 0.25f;
    [SerializeField] protected HitFXConfig[] hitFXList;
    public Transform hitTransform;//播放受击特效的位置
    [SerializeField] protected Vector3 hitFXScale;
    protected virtual  void Awake()
    {
       
    }
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        canExcuteCombo = true;
        runningEventIndex = new RunningEventIndex();
        audioSource = GetComponent<AudioSource>();
        attackCheckSystem = GetComponent<AttackCheckGizmos>();
        canBeHit = true;
        //Debug.Log(currentComboList.TryGetComboListCount());
    }

    
    protected virtual void Update()
    {
        RunEvent();
    }
    private void RunEvent()
    {
        if (!currentComboList) return;
        //匹配动画名称 和 判断动画是否处于过渡状态
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(currentComboList.TryGetComboName(currentComboIndex)) || animator.IsInTransition(0))return;
        //攻击检测
        ComboInteractionConfig comboInteractionConfig = currentComboList.TryGetComboInteractionConfig(currentComboIndex, runningEventIndex.attackDetectionIndex);
        if(comboInteractionConfig != null)
        {
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime>comboInteractionConfig.startTime)
            {
                //获得攻击反馈配置信息
                AttackFeedbackConfig attackFeedbackConfig=currentComboList.TryGetAttackFeedbackConfig(currentComboIndex,runningEventIndex.attackDetectionIndex);
                //执行攻击检测
                attackCheckSystem.StartAttacking(comboInteractionConfig, attackFeedbackConfig);
            }
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime>comboInteractionConfig.endTime)
            {
                attackCheckSystem.EndAttacking();
                //执行 结束攻击
                runningEventIndex.attackDetectionIndex++;
                runningEventIndex.attackFeedbackIndex++;
            }
        }
        //生成特效
        FXConfig fxConfig = currentComboList.TryGetFXConfig(currentComboIndex, runningEventIndex.FXIndex);
        if (fxConfig != null)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > fxConfig.startTime)
            {
                //修改位置
                Vector3 fxPosition = transform.forward * fxConfig.position.z + transform.up * fxConfig.position.y + transform.right * fxConfig.position.x;
                FXManager.Instance.PlayOneFX(fxConfig, fxPosition + transform.position,
                    fxConfig.rotation + transform.eulerAngles, fxConfig.scale);
                runningEventIndex.FXIndex++;
            }
        }
        //生成音效
        ClipConfig clipConfig = currentComboList.TryGetClipConfig(currentComboIndex, runningEventIndex.clipIndex);
        if (clipConfig != null)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > clipConfig.startTime)
            {
                //播放音效
                if (clipConfig.audioClip)
                {
                    audioSource.PlayOneShot(clipConfig.audioClip, clipConfig.volume);
                    runningEventIndex.clipIndex++;
                }
            }
        }
    }
    //播放攻击动画
    protected void ExcuteCombo()
    {
       //
       if(!canExcuteCombo) return;
       //重置事件计数
       runningEventIndex.Reset();
        //
        currentComboIndex = nextComboIndex;
        //播放动画
        animator.CrossFadeInFixedTime(currentComboList.TryGetComboName(currentComboIndex), 0.1555f,0,0);
        //后摇
        UpdateComboIndex();
        canExcuteCombo = false;
        StartCoroutine(IE_ExecuteComboColdTime(currentComboList.TryGetCoolDownTime(currentComboIndex)));
        //更新攻击计数
        if (stopComboCoroutine != null)
        {
            StopCoroutine(stopComboCoroutine);
        }
        stopComboCoroutine = StartCoroutine(IE_StopCombo(currentComboList.TryGetCoolDownTime(currentComboIndex)));
    }

    private Coroutine stopComboCoroutine;
    // 冷却后摇计时
    IEnumerator IE_ExecuteComboColdTime(float coldTime)
    {
        while(coldTime > 0)
        {
            coldTime-= Time.deltaTime;  
            yield return null;  
        }
        canExcuteCombo = true;
    }

    // 控制连招超时重置
    IEnumerator IE_StopCombo(float coldTime)
    {
        float time = coldTime * multiplier;
        while (time > 0)
        {
            time -= Time.deltaTime;  
            yield return null;
        }
        nextComboIndex = 0;
    }

    //更新攻击计数
    protected void UpdateComboIndex()
    {
        nextComboIndex++;
       //Debug.Log(nextComboIndex.ToString());
   
        if(nextComboIndex>=currentComboList.TryGetComboListCount())
        {
            nextComboIndex = 0;  
        }
    }

    /// <summary>
    /// 受击函数
    /// </summary>
    [SerializeField] private float rotationSpeed;
    //受击函数
    public virtual void OnHit(ComboInteractionConfig interactionConfig, AttackFeedbackConfig attackFeedbackConfig, Transform attacker)
    {
        //看向攻击者
        //获取当前的旋转和目标旋转
        Quaternion fromRotation = transform.rotation;
        Quaternion toRotation = Quaternion.LookRotation(-attacker.position, Vector3.up);
        //平滑过渡到目标旋转
        transform.rotation = Quaternion.Lerp(fromRotation, toRotation, Time.deltaTime * rotationSpeed);
        if (!canBeHit)
        {
            return;
        }
        canBeHit = false;
        StartCoroutine(IE_HitCoolDown(attackFeedbackConfig, hitCoolDown));
        //生成受击特效
        string hitFXName = hitFXList[(int)interactionConfig.attackForce].TryGetHitFXName();
        FXManager.Instance.PlayOneHitFX(hitFXName, hitTransform.position, hitFXScale);

    }

    IEnumerator IE_HitCoolDown(AttackFeedbackConfig attackFeedbackConfig, float coolDownTime)
    {
        coolDownTime = coolDownTime + attackFeedbackConfig.stopFrameTime;
        Debug.Log(coolDownTime);
        while (coolDownTime > 0)
        {
            yield return null;
            coolDownTime -= Time.deltaTime;
        }
        canBeHit = true;
    }

    protected void SetAnimatorSpeed(float speed) => animator.speed = speed;

    protected void ResetAnimatorSpeed() => animator.speed = 1f;
}

public class RunningEventIndex
{
    public int attackDetectionIndex = 0;
    public int FXIndex = 0;
    public int clipIndex = 0;
    public int attackFeedbackIndex = 0;
    public void Reset()
    {
        attackDetectionIndex = 0;
        FXIndex = 0;
        clipIndex = 0;
        attackFeedbackIndex = 0;
    }
}
