using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    [SerializeField] private Transform detectionCenter;// 检测起点（通常绑定到敌人眼部）
    [SerializeField] private float detectionRadius; // 最大检测距离
    [SerializeField][Range(0.5f, 1f)] private float detectionRadiusMultiplier;// 丢失目标的缓冲系数
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer; // 障碍物层级（如墙壁、树木）
    [SerializeField] private Collider[] targets = new Collider[1];
    [SerializeField, Header("攻击目标")] private Transform currentTarget = null;// 当前锁定的目标
    public Transform CurrentTarget=> currentTarget;// 外部只读访问
    [SerializeField, Range(0f, 360f)] private float detectAngle;// 视野锥形角度（如90°表示前方扇形区域）
    

    void Update()
    {
        View();
    }

    //视野
    private void View()
    {
        //每帧调用 用OverlapSpheraNonAlloc减少消耗
        int targetCount = Physics.OverlapSphereNonAlloc(detectionCenter.position, detectionRadius, targets, playerLayer);
        bool isInView = false;
        //若玩家在检测范围内
        if(targetCount > 0)
        {
            //射线检测障碍物
            if (IsInView(targets[0].transform))
            {
                //检测玩家是否在该对象面前一定角度的范围内
                if (Vector3.Dot(((targets[0].transform.position+new Vector3(0,1f,0))-(transform.position+new Vector3(0,1.2f,0))).normalized,
                    transform.forward) > Mathf.Cos(Mathf.Deg2Rad * detectAngle / 2))
                {
                    currentTarget = targets[0].transform;
                    isInView = true;
                }
            }
        }
        //如果在检测范围的一定距离内 则不会丢失目标
        // 玩家离开视野后，不会立即丢失目标，而是超出 检测半径×缓冲系数 后才解除锁定
        if (currentTarget)
        {
            //防抖动设计：避免玩家在边界反复进出视野导致AI抽搐
            //战术空间：给玩家提供"脱离战斗"的策略维度
            if (!isInView&& Vector3.Distance(transform.position,currentTarget.position)>detectionRadius*detectionRadiusMultiplier)
            {
                currentTarget = null;
                targets[0] = null;
            }
        }
    }

    /// <summary>
    /// 检测玩家对象在视野中是否可见
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsInView(Transform target)
    {
        for(int i = 5; i <= 10; i += 5)
        {
            float offset = i / 10f;
            //若检测到了障碍物（只检测障碍物层）
            //从头部向target从root开始以此向上每隔0.5f发射一条射线，若有一条射线命中（检测不到障碍物） 则说明看得到 返回true
            //TODO： 修改玩家在下蹲时的碰撞体大小
            if(Physics.Raycast((detectionCenter.position),
                ((target.position+target.up*offset)-detectionCenter.position).normalized,
                out RaycastHit hit,Vector3.Distance(detectionCenter.position,target.position+target.up*offset),obstacleLayer)==false )
            {
                return true;
            }

        }
        return false;
    }

    #region Gizmos绘图

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(detectionCenter.position, detectionRadius);

        if (targets[0] != null && currentTarget != null)
        {
            Gizmos.DrawRay(detectionCenter.position, ((targets[0].transform.root.position + targets[0].transform.root.up * 0f) - detectionCenter.position).normalized);
            Gizmos.DrawRay(detectionCenter.position, ((targets[0].transform.root.position + targets[0].transform.root.up * 0.5f) - detectionCenter.position).normalized);
            Gizmos.DrawRay(detectionCenter.position, ((targets[0].transform.root.position + targets[0].transform.root.up * 1f) - detectionCenter.position).normalized);
            Gizmos.DrawRay(detectionCenter.position, ((targets[0].transform.root.position + targets[0].transform.root.up * 1.5f) - detectionCenter.position).normalized);
        }
    }

    #endregion
}
