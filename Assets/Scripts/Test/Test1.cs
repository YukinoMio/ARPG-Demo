using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test1 : MonoBehaviour
{

    public NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //自动寻路设置目标点
        //agent.SetDestionation();
        //停止寻路
        agent.isStopped = true;

    }
}
