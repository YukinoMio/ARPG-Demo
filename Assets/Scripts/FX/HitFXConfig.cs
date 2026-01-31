using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


[Serializable]
public struct HitFXInfo
{
    public GameObject hitFX;
    public string hitFXName;
}
[CreateAssetMenu(fileName = "HitFXConfig", menuName = "ScriptableObject/FX/HitFXConfig")]
public class HitFXConfig : ScriptableObject
{
    [SerializeField] private HitFXInfo[] hitFXInfoList=new HitFXInfo[1];
    public string TryGetHitFXName()
    {
        return hitFXInfoList[Random.Range(0, hitFXInfoList.Length)].hitFXName;
    }
    public GameObject TryGetHitFXObj()
    {
        return hitFXInfoList[Random.Range(0, hitFXInfoList.Length)].hitFX;
    }
}
