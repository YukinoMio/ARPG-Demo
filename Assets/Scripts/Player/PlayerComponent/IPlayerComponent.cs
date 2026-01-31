/// <summary>
/// 玩家组件接口 - 所有玩家组件都应实现这个接口
/// </summary>
public interface IPlayerComponent
{
    /// <summary>
    /// 初始化组件
    /// </summary>
    void Initialize(PlayerController playerController);

    /// <summary>
    /// 可选：组件更新
    /// </summary>
    void OnUpdate() { } // 默认空实现

    /// <summary>
    /// 可选：组件固定更新
    /// </summary>
    void OnFixedUpdate() { } // 默认空实现

    /// <summary>
    /// 可选：组件延迟更新
    /// </summary>
    void OnLateUpdate() { } // 默认空实现
}
