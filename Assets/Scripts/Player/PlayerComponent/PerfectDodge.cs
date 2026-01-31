using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectDodge : MonoBehaviour
{
    [SerializeField] private PlayerCombatController playerCombatController;
    private bool canTrigerperfectDodge;
    // Start is called before the first frame update
    void Start()
    {
        canTrigerperfectDodge = true;
    }

    public void PerfectDodgeInterface()
    {
        if (canTrigerperfectDodge)
        {
            //执行完美闪避的逻辑
            canTrigerperfectDodge = false;
            playerCombatController.PerfectDodge();
            StartCoroutine(IE_CanPerfectDodgeTimeCount(playerCombatController.GetCanPerfectDodgeTime()));
        }
    }

    //完美闪避冷却
    IEnumerator IE_CanPerfectDodgeTimeCount(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);// 使用真实时间等待（不受Time.timeScale影响）
        canTrigerperfectDodge = true;
    }
}
