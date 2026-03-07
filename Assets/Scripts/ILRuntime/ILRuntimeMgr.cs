//using ILRuntime.Mono.Cecil.Pdb;
//using ILRuntime.Runtime;
//using ILRuntime.Runtime.Enviorment;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.CompilerServices;
//using System.Threading;
//using UnityEngine;
//using UnityEngine.Events;

public class ILRuntimeMgr : SingletonAutoMono<ILRuntimeMgr>
{

    //public AppDomain appDomain;

    ////dll文件和pdb文件的流对象
    //private MemoryStream dllStream;
    //private MemoryStream pdbStream;

    ////是否是调试模式
    //private bool isDebug = false;

    ////是否已经加载了对应的文件
    //private bool isStart = false;



    ///// <summary>
    ///// 启动ILRuntime 加载对应的dll和pdb文件
    ///// </summary>
    //public void StartILRuntime(UnityAction callBack, UnityAction<string> infoCallBack)
    //{
    //    if (!isStart)
    //    {
    //        isStart = true;
    //        //初始化
    //        appDomain = new AppDomain(ILRuntimeJITFlags.JITOnDemand);
    //        //加载对应的dll和pdb文件  从AB包中加载
    //        //通过AB包管理器 异步加载DLL文件信息
    //        infoCallBack("开始更新dll文件");
    //        ABMgr.GetInstance().LoadResAsync<TextAsset>("dll_res", "HotFix_Project.dll", (dll) =>
    //        {
    //            //异步加载完dll后 再去异步加载pdb文件 加载结束后 在使用他们来进行初始化
    //            infoCallBack("开始更新pdb文件");
    //            ABMgr.GetInstance().LoadResAsync<TextAsset>("dll_res", "HotFix_Project.pdb", (pdb) =>
    //            {

    //                //根据加载的文本信息 初始化 对应的两个流对象
    //                dllStream = new MemoryStream(dll.bytes);
    //                pdbStream = new MemoryStream(pdb.bytes);
    //                //利用初始化的流对象 进行ILRuntime的初始化
    //                appDomain.LoadAssembly(dllStream, pdbStream, new PdbReaderProvider());

    //                infoCallBack("开始初始化我们的 ILRuntime");
    //                //初始化相关操作
    //                InitILRuntime();

    //                if (isDebug)
    //                {
    //                    StartCoroutine(WaitDebugger(callBack));
    //                }
    //                else
    //                {
    //                    //加载结束 初始化结束 把逻辑交给外部继续处理
    //                    infoCallBack("初始化结束");
    //                    callBack?.Invoke();
    //                }

    //            });
    //        });
    //    }
    //}

    //private void InitILRuntime()
    //{
    //    //如果想使用Unity自带的性能调试窗口，调试ILRuntime相关内容 就需要加入该行代码
    //    //appDomain.UnityMainThreadID=Thread.CurrentThread.ManagedThreadId;   
    //}

    //IEnumerator WaitDebugger(UnityAction callback)
    //{
    //    while (!appDomain.DebugService.IsDebuggerAttached)
    //    {
    //        yield return null;
    //    }
    //    yield return new WaitForSeconds(1f);
    //    callback?.Invoke();
    //}

    ///// <summary>
    ///// 停止ILRuntime 卸载对应文件
    ///// </summary>
    //public void StopILRuntime()
    //{
    //    if (dllStream != null)
    //    {
    //        dllStream.Close();
    //    }
    //    if (pdbStream != null)
    //        pdbStream.Close();
    //    dllStream = null;
    //    pdbStream = null;
    //    appDomain = null;
    //}
}
