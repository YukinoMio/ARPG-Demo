using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : UIPanelBase
{
    //进度条 
    public Image imgPro;
    //当前加载信息
    public Text txtInfo;
    //加载百分比
    public Text txtPer;
    private void Start()
    {
        //设置宽为0 高为50
        imgPro.rectTransform.sizeDelta = new Vector2(0, 50);
        txtInfo.text = "加载资源...";
    }
    //第一：需要去更新资源服务器上的AB包

    public void BeginUpdate()
    {
        //第一个
        ABUpdateMgr.Instance.CheckUpdate(ABUpdateOverDoSomething, (info) =>
        {
            txtInfo.text = info;
        }, (nowNum, maxNum) =>
        {
            imgPro.rectTransform.sizeDelta = new Vector2(nowNum / maxNum * 1600, 50);
        });
         
    }
    //第二： AB包更新完毕后 需要去处理ILRuntime初始化相关的逻辑
    public void ABUpdateOverDoSomething(bool isOver)
    {
        if(!isOver)
        {
            txtInfo.text = "AB包下载更新出错，请检查网络连接";
        }
        txtInfo.text = "资源加载结束";
        //ILRuntime的初始化相关
        ILRuntimeMgr.GetInstance().StartILRuntime(() =>
        {
            //ILRuntime相关内容加载结束 就可以执行游戏逻辑了
            txtInfo.text = "游戏初始化完毕";
            //热更相关逻辑执行
        }, (info) =>
        {
            txtInfo.text = info;
        });
    }
}
