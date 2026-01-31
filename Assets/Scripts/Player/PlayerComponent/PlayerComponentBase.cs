using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponentBase : MonoBehaviour
{
    protected PlayerController player;
    public virtual void Initialize(PlayerController playerController)
    {
        this.player = playerController;
    }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateUpdate() { }  
}
