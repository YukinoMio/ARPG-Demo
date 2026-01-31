using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件中心- 基于泛型的类型安全事件系统
/// 使用struct事件避免GC分配 提供高性能事件通信
/// </summary>
public class EventCenter :SingletonMonoBase<EventCenter>
{
    [Header("事件统计")]
    [SerializeField] private int totalEventFired;
    [SerializeField] private int activeSubscribers;
    //类型到委托的映射
    private Dictionary<Type,Delegate>eventHandlers=new Dictionary<Type, Delegate>();
    //订阅者管理：用于自动清理和调试
    private Dictionary<object,List<Type>> subscriberRegistrations=new Dictionary<object, List<Type>>();

    //性能监控：记录事件触发时间和调试
    private Queue<float> eventTimestamps=new Queue<float>();
    private const float STATS_WINDOWS = 10f;//10秒统计窗口

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("事件中心初始化完成");
    }

    /// <summary>
    /// 订阅事件 - 类型安全版本
    /// 使用泛型约束确保只有结构体可以作为事件数据，避免GC分配
    /// </summary>
    /// <typeparam name="T">事件数据类型，必须是结构体</typeparam>
    /// <param name="handler">事件处理函数</param>
    /// <example>
    /// EventCenter.Instance.Subscribe<PlayerHealthChanged>(OnHealthChanged);
    /// </example>
    public void Subscribe<T> (Action<T> handler) where T: struct
    {
        Type eventType=typeof(T);

        if (eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType] = Delegate.Combine(eventHandlers[eventType], handler); //合并委托
        }
        else
        {
            eventHandlers.Add(eventType, handler);
        }
    }

    /// <summary>
    /// 发布事件 - 线程安全的事件触发
    /// </summary>
    /// <typeparam name="T">事件数据类型</typeparam>
    /// <param name="eventData">事件数据</param>
    /// <example>
    /// EventCenter.Instance.Publish(new PlayerHealthChanged { CurrentHealth = 100 });
    /// </example>
    public void Publish<T> (T eventData) where T: struct//必须是值类型
    {
        Type eventType= typeof(T);
        totalEventFired++;
        //记录事件时间用于性能统计
        eventTimestamps.Enqueue(Time.time);
        //触发事件处理
        if (eventHandlers.ContainsKey(eventType) && eventHandlers[eventType]!=null)
        {
            try
            {
                (eventHandlers[eventType] as Action<T>)?.Invoke(eventData);//Delegate 没有接受 T 参数的 Invoke 方法
            }
            catch   (Exception e)
            {
                Debug.LogError($"事件处理错误 [{eventType.Name}]: {e.Message}\n{e.StackTrace}");
            }      
        }
    }
    /// <summary>
    /// 取消订阅特定时间
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    public void Unsubscribe<T> (Action<T> handler) where T: struct
    {
        Type eventType= typeof(T);
        if(eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType] = Delegate.Remove(eventHandlers[eventType],handler);
            //如果该事件类型没有监听者了 从字典移除
            if (eventHandlers[eventType] == null)
            {
                eventHandlers.Remove(eventType);
            }
        }
        //更新订阅者记录

    }


    /// <summary>
    /// 取消订阅者对象的所有事件 - 用于对象销毁时自动清理
    /// </summary>
    /// <param name="suscriber"></param>
    public void UnsubscribeAll(object subscriber)
    {
        if (subscriberRegistrations.ContainsKey(subscriber))
        {
            foreach (var eventType in subscriberRegistrations[subscriber])
            {
                if (eventHandlers.ContainsKey(eventType))
                {
                    // 获取所有委托并过滤出属于该订阅者的
                    var handlers = eventHandlers[eventType].GetInvocationList();
                    foreach (var handler in handlers)
                    {
                        if (handler.Target == subscriber)
                        {
                            eventHandlers[eventType] = Delegate.Remove(eventHandlers[eventType], handler);
                        }
                    }

                    // 清理空的事件类型
                    if (eventHandlers[eventType] == null)
                    {
                        eventHandlers.Remove(eventType);
                    }
                }
            }
            subscriberRegistrations.Remove(subscriber);
        }
    }


    /// <summary>
    /// 注册订阅者记录 - 内部管理使用
    /// </summary>
    private void RegisterSubscriber(object subscriber, Type eventType)
    {
        if (!subscriberRegistrations.ContainsKey(subscriber))
        {
            subscriberRegistrations[subscriber] = new List<Type>();
        }

        if (!subscriberRegistrations[subscriber].Contains(eventType))
        {
            subscriberRegistrations[subscriber].Add(eventType);
        }
    }

    /// <summary>
    /// 取消订阅者记录 - 内部管理使用
    /// </summary>
    private void UnregisterSubscriber(object subscriber, Type eventType)
    {
        if (subscriberRegistrations.ContainsKey(subscriber))
        {
            subscriberRegistrations[subscriber].Remove(eventType);

            // 如果订阅者没有监听任何事件了，移除记录
            if (subscriberRegistrations[subscriber].Count == 0)
            {
                subscriberRegistrations.Remove(subscriber);
            }
        }
    }

    
    private void CleanupExpiredTimestamps()
    {
        while(eventTimestamps.Count > 0&& Time.time-eventTimestamps.Peek()>STATS_WINDOWS)
        {
            eventTimestamps.Dequeue();
        }
    }


    /// <summary>
    /// 获取事件频率（事件/秒）- 性能监控
    /// </summary>
    public float GetEventsPerSecond()
    {
        if (eventTimestamps.Count < 2) return 0;
        return eventTimestamps.Count / STATS_WINDOWS;
    }

    public void ClearAllEvents()
    {
        eventHandlers.Clear();
        subscriberRegistrations.Clear();
        eventTimestamps.Clear();
        totalEventFired = 0;
        activeSubscribers = 0;
        Debug.Log("事件中心已清空所有事件订阅");
    }

    /// <summary>
    /// 打印调试信息 - 开发时使用
    /// </summary>
    public void PrintDebugInfo()
    {
        Debug.Log($"=== 事件中心状态 ===");
        Debug.Log($"总事件触发: {totalEventFired}");
        Debug.Log($"活跃订阅者: {activeSubscribers}");
        Debug.Log($"事件频率: {GetEventsPerSecond():F1} 事件/秒");
        Debug.Log($"已注册事件类型: {eventHandlers.Count}");

        foreach (var kvp in eventHandlers)
        {
            int handlerCount = kvp.Value?.GetInvocationList().Length ?? 0;
            Debug.Log($"  {kvp.Key.Name}: {handlerCount} 个监听器");
        }
    }
}
