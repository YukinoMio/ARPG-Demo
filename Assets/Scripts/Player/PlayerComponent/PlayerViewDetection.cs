using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using static PlayerStateManagerComponent;
public class PlayerViewDetection : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineTargetGroup cinemachineTargetGroup;

    [FormerlySerializedAs("enemies")]
    [Header("玩家索敌")]
    [SerializeField] private Collider[] enemyColliders;//玩家面前一定距离内的敌人数组
    [SerializeField] private bool isLockTarget = false;//锁定敌人目标

    [FormerlySerializedAs("viewDistance")]
    [FormerlySerializedAs("distance")]
    [Header("玩家视野检测")]
    [SerializeField] private float maxLockOnDistance = 30f;//最大锁定距离
    [SerializeField] private Vector3 offset;//检测盒偏移量
    [SerializeField] private Vector3 size;//检测盒尺寸
    [SerializeField] private Vector3 cubeCenter;//检测盒中心点
    [SerializeField] private Vector3 rotateEuler;//检测盒旋转角度
    [SerializeField] private LayerMask enemyLayer;//敌人层级
    [SerializeField] private LayerMask ignoreLayer;//忽略层级（用于视线检测）

    private int lockOnHash = Animator.StringToHash("LockOn");
    private int xInputHash=Animator.StringToHash("XInput");
    private int yInputHash = Animator.StringToHash("YInput");
    private int xSpeedHash = Animator.StringToHash("XSpeed");
    private int ySpeedHash = Animator.StringToHash("YSpeed");
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();    
    }
    private void LateUpdate()
    {
        FindEnemyInFront();
        SwitchAnimator();
        LockOnEnemy();
        if (nearestLockOnTarget)
        {
            Vector3 dir = nearestLockOnTarget.position - mainCamera.transform.position;
            dir.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            Vector3 eulerAngles = targetRotation.eulerAngles;
            eulerAngles.y = 0;
            mainCamera.transform.localEulerAngles = eulerAngles;
        }

    }
    [SerializeField] private Transform viewTransform;//视角参考点，可能设置在眼睛高度
    [SerializeField][Range(0, 180)] private float viewAngle = 50f;//视野角度
    //要改成EnemyCombatController
    [SerializeField] private List<EnemyLockOn> availableTargets = new List<EnemyLockOn>();//可用目标列表
    [SerializeField] private Transform nearestLockOnTarget;//最近锁定目标

    //查找前方敌人
    private void FindEnemyInFront()
    {
        //如果不在锁定状态 则不进行查找
        if(!isLockTarget)
        {
            return;
        }   
        availableTargets.Clear();
        //检测相机面前的盒形碰撞体内是否有ENemy
        Vector3 cameraPos=mainCamera.transform.position;
        Vector3 cameraForward =mainCamera.transform.forward;    
        cubeCenter=new Vector3(offset.x*cameraForward.x,offset.y*cameraForward.y,offset.z*cameraForward.z)+cameraPos;
        enemyColliders = Physics.OverlapBox(cubeCenter, size / 2, Quaternion.Euler(rotateEuler), enemyLayer);
        if(enemyColliders.Length > 0 )
        {
            //找到所有的enemies中离玩家最近的
            for(int i = 0; i < enemyColliders.Length; i++)
            {
                EnemyLockOn enemy = enemyColliders[i].GetComponent<EnemyLockOn>();
                if (enemy)
                {
                    Vector3 LockTargetDirection = new Vector3();
                    LockTargetDirection = enemy.transform.position - viewTransform.position;
                    float distanceFromTarget=Vector3.Distance(viewTransform.position,enemy.transform.position); 
                    float viewableAngle =Vector3.Angle(LockTargetDirection,mainCamera.transform.forward);   
                    if(viewableAngle>-viewAngle&&viewableAngle<viewAngle&& distanceFromTarget<=maxLockOnDistance)
                    {
                        availableTargets.Add(enemy);
                    }
                }
            }

            //寻找距离玩家最近的目标
            float shortestDistance=float.MaxValue;
            for(int i= 0; i < availableTargets.Count; i++)
            {
                float distanceFromTarget = Vector3.Distance(viewTransform.position, availableTargets[i].transform.position);
                if (distanceFromTarget <= maxLockOnDistance && distanceFromTarget < shortestDistance)
                {
                    shortestDistance=distanceFromTarget;
                    nearestLockOnTarget = availableTargets[i].lockOnTransform;
                }
            }
            if (nearestLockOnTarget)
            {
                //设置摄像机对象
                SetCameraTarget(nearestLockOnTarget);
            }
        }
    }
    [SerializeField] private float targetWeight;
    private void SetCameraTarget(Transform targetTransform)
    {
        animator.SetFloat(lockOnHash, 1f);
        //若当前没有
        if (cinemachineTargetGroup.m_Targets.Length == 1)//说明只有玩家一个
        {
            cinemachineTargetGroup.AddMember(targetTransform, targetWeight, 1);
            
        }
        //若此时已经有一个目标对象，则使用传入的targetTransform更换它
        else if (cinemachineTargetGroup.m_Targets.Length == 2)
        {
            CinemachineTargetGroup.Target newTarget= new CinemachineTargetGroup.Target
            {
                target = targetTransform, weight = targetWeight, radius = 1f
            };
            cinemachineTargetGroup.m_Targets[1] = newTarget;
        }
        else
        {
            Debug.LogError(string.Format("CinemachineTargetGroup的对象数量不正确，此时其中有{0}哥对象 ", cinemachineTargetGroup.m_Targets.Length));
        }
        Vector3 dir=targetTransform.position-mainCamera.transform.position;
        dir.Normalize();
        Quaternion targetRotation =Quaternion.LookRotation(dir);
        Vector3 eulerAngles=targetRotation.eulerAngles;//欧拉角和四元数转换可能消耗过多性能
        eulerAngles.y = 0;
        mainCamera.transform.localEulerAngles = eulerAngles;    
        cinemachineTargetGroup.DoUpdate();
    }

    /// <summary>
    /// 判断物体是否在相机中可见
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsVisableInCamera(Camera camera ,Transform target)
    {
        if (!camera || !target) return false;
        //将目标物体坐标转为屏幕坐标
        Vector3 screenPoint =camera.WorldToScreenPoint(target.position);
        //该物体坐标在屏幕外
        if (screenPoint.x < 0 || screenPoint.y < 0 || screenPoint.x > Screen.width || screenPoint.y > Screen.height||screenPoint.z<=0)
        {
            return false;
        }
        //从摄像机向慕白哦物体发射射线
        Ray ray =camera.ScreenPointToRay(screenPoint);
        //忽略检测Player和Player下子物体层
        if(Physics.Raycast(ray,out RaycastHit hit, maxLockOnDistance, ~(ignoreLayer)))
        {
            if(hit.collider.gameObject!=target.gameObject)
            {
                Debug.Log("视线被" + hit.collider.gameObject.name + "阻挡");

            }
            return hit.collider.gameObject == target.gameObject;
        }
        return false;
    }
    private Vector3 dir;
    private void SwitchAnimator()
    {
        dir = new Vector3(playerController.Movement.GetPlayerMovement().x, 0, playerController.Movement.GetPlayerMovement().z);
        if (playerController.StateManager.CurrentPosture == PlayerStateManagerComponent.PlayerPosture.Stand)
        {
            animator.SetFloat(xInputHash, playerController.InputHandler.GetMoveInput().x);
            animator.SetFloat(yInputHash, playerController.InputHandler.GetMoveInput().y);
            switch (playerController.StateManager.CurrentStandMotion)
            {
                case PlayerStateManagerComponent.StandMotion.Idle:
                    animator.SetFloat(xSpeedHash,0,0.1f,Time.deltaTime);
                    animator.SetFloat(ySpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case PlayerStateManagerComponent.StandMotion.Walk:
                    animator.SetFloat(xSpeedHash, dir.x * playerController.Config.GetWalkSpeed(), 0.1f, Time.deltaTime);
                    animator.SetFloat(ySpeedHash, dir.z * playerController.Config.GetWalkSpeed(), 0.1f, Time.deltaTime);
                    break;
                case PlayerStateManagerComponent.StandMotion.Run:
                    animator.SetFloat(xSpeedHash, dir.x * playerController.Config.GetRunSpeed(), 0.1f, Time.deltaTime);
                    animator.SetFloat(ySpeedHash, dir.z * playerController.Config.GetRunSpeed(), 0.1f, Time.deltaTime);
                    break;
            }
        }
    }

    //
    [SerializeField] private Transform target;
    [SerializeField] private float lockRotationSpeed;
    [SerializeField] private float offsetAngle;
    [SerializeField] private float stopFaceDis;
    private void LockOnEnemy()
    {
        //若不在锁定状态 ||找不到可以锁定的目标
        if(!isLockTarget||!nearestLockOnTarget)
        {
            animator.SetFloat(lockOnHash, 0f);
            ClearViewTarget();//清空之前查找到的目标
            ClearCameraTarget();//清空CameraGroup中除玩家之外的对象
            isLockTarget = false;//退出锁定状态（针对找不到可以锁定的目标）
            return;
        }
        //
        Vector3 toTarget=nearestLockOnTarget.position-transform.position;
        toTarget.y = 0;
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("EquipMotion")||
                animator.GetCurrentAnimatorStateInfo(0).IsTag("Equip") ||
            animator.GetCurrentAnimatorStateInfo(0).IsTag("KatanaAttack") ||
            animator.GetCurrentAnimatorStateInfo(0).IsTag("GSAttack") ||
            ((animator.GetCurrentAnimatorStateInfo(0).IsTag("Roll")) && Vector3.Distance(transform.position, nearestLockOnTarget.position) > stopFaceDis) ||
            animator.IsInTransition(0))
        {
            Quaternion baseRotation = Quaternion.LookRotation(toTarget);
            //创建左侧偏移
            Quaternion leftOffset=Quaternion.AngleAxis(offsetAngle,Vector3.up);
            //组合两个旋转
            Quaternion targetRotation = baseRotation * leftOffset;
            //旋转玩家root
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,lockRotationSpeed*Time.deltaTime);
        }


    }

    private void ClearViewTarget()
    {
        nearestLockOnTarget = null;
        availableTargets.Clear();
    }
    private void ClearCameraTarget()
    {
        CinemachineTargetGroup.Target[] newTargets=new CinemachineTargetGroup.Target[]{ cinemachineTargetGroup.m_Targets[0]};
        cinemachineTargetGroup.m_Targets=newTargets;
    }

    #region Gizoms
    private void OnDrawGizmos()
    {
        Vector3 cameraPos=mainCamera.transform.position;
        Vector3 cameraForward=mainCamera.transform.forward; 
        cubeCenter=new Vector3(offset.x*cameraForward.x,offset.y*cameraForward.y,offset.z*cameraForward.z)+cameraPos;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(cubeCenter, size);
    }

    private void DrawRay()
    {
        for(int i = 0; i < enemyColliders.Length; i++)
        {
            Transform target = enemyColliders[i].transform;
            Vector3 screenPoint= mainCamera.WorldToScreenPoint(target.position);
            Ray ray=mainCamera.ScreenPointToRay(screenPoint);
            Gizmos.DrawRay(ray.origin, ray.direction * maxLockOnDistance);

        }
    }
    #endregion

    #region 玩家输入相关
    //获取锁定敌人输入
    public void GetLockTargetInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isLockTarget = !isLockTarget;
        }
    }
    #endregion
}
