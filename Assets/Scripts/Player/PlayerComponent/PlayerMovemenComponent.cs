using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovemenComponent : PlayerComponentBase
{
    [Header("移动状态")]
    [SerializeField] private Vector3 playerMovement = Vector3.zero;//移动方向
    [SerializeField] private Vector3 averageVelocity = Vector3.zero;

    public Vector3 PlayerMovement => playerMovement;
    public Vector3 GetPlayerMovement() => playerMovement;
    //平均速度变量
    public Vector3 AverageVelocity => averageVelocity;

    //玩家空中水平移动速度的缓存值
    private static readonly int CACHE_SIZE = 3;
    //缓存池
    private Vector3[] velCache = new Vector3[CACHE_SIZE];
    //缓存池中最老的向量的索引值
    private int currentCacheIndex = 0;

    private Transform cameraTransform;

    public override void Initialize(PlayerController playerController)
    {
        base.Initialize(playerController);
        cameraTransform = Camera.main.transform;
    }

    public override void OnUpdate()
    {
        CaculateInputDirection();
    }

    /// <summary>
    /// 计算移动方向
    /// </summary>
    private void CaculateInputDirection()
    {
        //TODO：控制玩家在攻击是不能旋转
        Vector3 cameraForwardProjection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        //根据玩家输入和 摄像机投影 计算移动方向
        playerMovement = cameraForwardProjection * player.InputHandler.MoveInput.y +
                        cameraTransform.right* player.InputHandler.MoveInput.x;
        //世界坐标转 本地坐标
        playerMovement = player.transform.InverseTransformVector(playerMovement);//移动方向跟随自身旋转朝向
    }

    /// <summary>
    /// 计算玩家离地前3帧的平均水平移动速度
    /// </summary>
    /// <param name="newVel">当前帧的速度</param>
    /// <returns>计算出的平均速度</returns>
    /// 常用于游戏开发中的各种平滑和去噪场景
    public Vector3 CalculateAverageVelocity(Vector3 newVel)
    {
        //缓存池设计为循环队列
        //新速度替换缓存池中最老的速度
        velCache[currentCacheIndex] = newVel;
        currentCacheIndex++;
        //取模防止数组越界
       currentCacheIndex %= CACHE_SIZE;
        //计算缓存池中速度的平均值
        Vector3 average = Vector3.zero;
        foreach( var speed in velCache )
        {
            average += speed;
        }
        return average / CACHE_SIZE;
    }
   
    public float GetCurrentSpeed()
    {
        if (player.InputHandler.IsCrouchPressed) return player.Config.crouchSpeed;
        if (player.InputHandler.IsRunPressed) return player.Config.runSpeed;
        return player.Config.walkSpeed;
    }
}
