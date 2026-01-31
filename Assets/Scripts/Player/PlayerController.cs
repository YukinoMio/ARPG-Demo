using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("配置")]
    [SerializeField] private PlayerConfig config;

    [Header("组件引用")]
    [SerializeField] private PlayerInputComponent inputHandler;
    [SerializeField] private PlayerMovemenComponent movement;
    [SerializeField] private PlayerAnimatorComponent animator;
    [SerializeField] private PlayerPhysicsComponent physics;
    [SerializeField] private PlayerGroundCheck groundCheck;
    [SerializeField] private PlayerStateManagerComponent stateManager;
    [SerializeField] private PlayerSoundController soundController;
    [SerializeField] private AdvancedAttributeSystem advancedAttributeSystem;
    [SerializeField] private WeaponSystem weaponSystem;
    //[SerializeField] private SwapWeapon swapWeaponSystem;
    //[SerializeField] private PlayerCombatController combat;

    [Header("业务处理器")]
    private LevelUpProcessor levelUpProcessor;
    
    public PlayerConfig Config => config;//只读
    public PlayerInputComponent InputHandler => inputHandler;
    public PlayerMovemenComponent Movement => movement;
    public PlayerAnimatorComponent Animator => animator;
    public PlayerPhysicsComponent Physics => physics;
    public PlayerGroundCheck GroundCheck => groundCheck;
    public PlayerStateManagerComponent StateManager => stateManager;
    public PlayerSoundController SoundController => soundController;
    public AdvancedAttributeSystem AdvancedAttributeSystem => advancedAttributeSystem;

    //public SwapWeapon SwapWeapon => swapWeaponSystem;
    //public PlayerCombatController CombatController => combat;
    public WeaponSystem WeaponSystem => weaponSystem;
    public LevelUpProcessor LevelUpProcessor => levelUpProcessor;

    private CharacterController characterController;
    public CharacterController CharacterController => characterController;
    private List<PlayerComponentBase> allComponents = new List<PlayerComponentBase>();

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (advancedAttributeSystem == null)
            advancedAttributeSystem = GetOrAddComponent<AdvancedAttributeSystem>();
        //if(combat == null)
            //combat=GetComponent<PlayerCombatController>();  
        InitializComponents();
        InitializeProcessors();
    }

    private void InitializComponents()
    {
        // 建议明确初始化顺序
        // 1. 基础组件（输入、物理）
        if (inputHandler == null)
        {
            Debug.LogError("PlayerInputComponent 需要手动挂载！请在Inspector中拖拽赋值。");
           //inputHandler=GetOrAddComponent<PlayerInputComponent>();
        }
        if (physics == null) physics = GetOrAddComponent<PlayerPhysicsComponent>();
        if (groundCheck == null) groundCheck = GetOrAddComponent<PlayerGroundCheck>();

        // 2. 状态管理
        if (stateManager == null) stateManager = GetOrAddComponent<PlayerStateManagerComponent>();

        // 3. 其他组件
        if (movement == null) movement = GetOrAddComponent<PlayerMovemenComponent>();  
        if (animator == null) animator = GetOrAddComponent<PlayerAnimatorComponent>();
        //if (soundController == null) soundController = GetOrAddComponent<PlayerSoundController>();
        //if(advancedAttributeSystem==null) advancedAttributeSystem= GetComponent<AdvancedAttributeSystem>();
        if (weaponSystem == null)
            weaponSystem = GetOrAddComponent<WeaponSystem>();
        //if (swapWeaponSystem == null)
            //swapWeaponSystem = GetOrAddComponent<SwapWeapon>();
        var initOrder = new List<PlayerComponentBase>
    {
        inputHandler, physics, groundCheck,
        stateManager, movement, animator, /*soundController,*/advancedAttributeSystem,weaponSystem,//swapWeaponSystem
    };

        foreach (var component in initOrder)
        {
            if (component != null&&component!=inputHandler)
                component.Initialize(this);
        }

        allComponents.AddRange(initOrder.Where(c => c != null));
    }

    private void InitializeProcessors()
    {
        //确保数值系统已经初始化
        if (advancedAttributeSystem != null)
        {
            levelUpProcessor=new LevelUpProcessor(this,advancedAttributeSystem);    

        }
        else
        {
            Debug.LogError("AdvancedAttributeSystem未初始化，无法创建LevelUpProcessor");
        }

    }
    private T GetOrAddComponent<T>() where T : PlayerComponentBase
    {
        T component = GetComponent<T>();
        if (component == null)
            component = gameObject.AddComponent<T>();
        return component;
    }
    void Start()
    {

    }
    void Update()
    {
        foreach (var component in allComponents)
        {
            if (component != null)
            {
                component.OnUpdate();
            }
        }
    }
    private void LateUpdate()
    {
        foreach (var component in allComponents)
        {
            if (component != null)
                component.OnLateUpdate();
        }
    }
    private void OnAnimatorMove()
    {
        ApplyFinalMovement();
    }
    private void ApplyFinalMovement()
    {
        if (animator == null || animator.Animator == null) return;

        Vector3 finalMovement = Vector3.zero;

        if (stateManager.CurrentPosture != PlayerStateManagerComponent.PlayerPosture.Jumping)
        {
            Vector3 animatorDelta = animator.Animator.deltaPosition;
            animatorDelta.y = physics.VerticalVelocity * Time.deltaTime;
            finalMovement = animatorDelta;

            movement.CalculateAverageVelocity(animator.Animator.velocity);
        }
        else
        {
            Vector3 jumpMovement = movement.AverageVelocity;
            jumpMovement.y = physics.VerticalVelocity;
            finalMovement = jumpMovement * Time.deltaTime;
        }

        characterController.Move(finalMovement);
    }

    private void OnDestroy()
    {
        levelUpProcessor?.Dispose();
    }
}
