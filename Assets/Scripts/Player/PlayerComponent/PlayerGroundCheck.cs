using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGroundCheck : PlayerComponentBase
{
    [Header("地面检测状态")]
    [SerializeField] private bool isGrounded;//是否在地面
    [SerializeField] private bool couldFall;//能否降落

    public bool IsGrounded => isGrounded;
    public bool CouldFall => couldFall;

    private CharacterController characterController;
    private float groundCheckOffset;
    private float fallHeight;//最小降落高度

    public override void Initialize(PlayerController playerController)
    {
        base.Initialize(playerController);
        characterController = player.CharacterController;
        groundCheckOffset = player.Config.groundCheckOffset;
        fallHeight= player.Config.fallHeight;   
    }

  
    void Update()
    {
        //Debug.Log("Ground Check");
        CheckGround();
    }


    private void CheckGround()
    {
        if (Physics.SphereCast(
            player.transform.position + (Vector3.up * groundCheckOffset),
            characterController.radius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckOffset - characterController.radius + 1.5f * characterController.skinWidth))
        {
            if (hit.collider.gameObject.CompareTag("Ground")/*hit.collider.gameObject.tag == "Ground"*/)
            {
                isGrounded = true;
                couldFall = false;
                return;
            }
        }

        isGrounded = false;
        couldFall = !Physics.Raycast(player.transform.position, Vector3.down, fallHeight);

    }

    //private void CheckGround()
    //{
    //    if (Physics.SphereCast(player.transform.position+(Vector3.up*groundCheckOffset),
    //        characterController.radius,Vector3.down,out RaycastHit hit,
    //        groundCheckOffset-characterController.radius+1.5f*characterController.skinWidth))
    //    {
    //        //检测到地面  且Tag是地面
    //        if (hit.collider.gameObject.CompareTag("Ground"))
    //        {
    //            couldFall = false;
    //            isGrounded = true;
    //                return;
    //        }
    //    }
    //    couldFall = !Physics.Raycast(player.transform.position, Vector3.down, fallHeight);
    //    isGrounded =false;
    //}
}
