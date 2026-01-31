using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float timer;
    private Action action;
    private bool timeIsDone;

    void Update()
    {
        OnUpdate();
        RecycleObject();
    }

    private void OnUpdate()
    {
        if(!this.gameObject.activeSelf)return ;
        if(timer>0&&!timeIsDone)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                action?.Invoke();
                timeIsDone = true;
            }
        }

    }

    /// <summary>
    /// 创建计时器
    /// </summary>
    /// <param name="timer">计时函数</param>
    /// <param name="callBackAction">回调函数</param>
    /// <param name="timeIsDone"></param>
    public void CreateTime(float timer,Action callBackAction,bool timeIsDone = false)
    {
        this.timer = timer;
        this.action = callBackAction;
        this.timeIsDone = timeIsDone;
    }

    public void RecycleObject()
    {
        if(timeIsDone)
        {
            action = null;
            //回收到对象池中
            CachePoolManager.Instance.PushObject(this.gameObject);
        }
    }
}
