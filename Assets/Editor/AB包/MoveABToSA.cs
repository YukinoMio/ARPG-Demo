using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Drawing.Printing;
using System.IO;
using UnityEngine.Analytics;
public class MoveABToSA 
{
   // [MenuItem("AB包工具/移动选中资源到StreamingAssets中")]
  private static void MoveABToStreamingAssets()
    {
        //通过编辑器Selection类中的放啊发  获取再Project窗口中选中的资源
        Object[] selectedAsset=Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (selectedAsset.Length == 0) return;
        //用于拼接本地默认AB包资源信息的字符串
        string abCompareInfo = "";
        foreach(Object asset in selectedAsset)
        {
            //通过Assetdatabase类 获取资源的路径
            string assetPath=AssetDatabase.GetAssetPath(asset);//Assets/ArtRes/AB/PC/lua.manifest
            Debug.Log(assetPath);
            //截取路径中的文件名  截取从后往前的第一个/
            string fileName = assetPath.Substring(assetPath.LastIndexOf('/'));//
            //判断是否有.符号 如果有 证明有后缀 不处理
            if(fileName.IndexOf('.')!=-1)
            {
                continue;
            }
            Debug.Log(fileName);///lua.manifest
            //利用AssetDataBase中的api 将选中文件 复制到目标路径
            AssetDatabase.CopyAsset(assetPath, "Assets/StreamingAssets" + fileName);

            //获取拷贝到StreamingAssets文件夹中的文件的全部信息
            FileInfo fileInfo = new FileInfo(Application.streamingAssetsPath + fileName);
            //拼接AB包信息到字符串中
            abCompareInfo += fileInfo.Name + " " + fileInfo.Length + " " + CreateABCompare.GetMD5(fileInfo.FullName);
            //用一个符号隔开多个AB包信息
            abCompareInfo += "|";
        }
        //去掉最后一个符号| 方便拆分字符串
        abCompareInfo =abCompareInfo.Substring(0,abCompareInfo.Length - 1);
        //将本地默认资源的对比信息 存入文件
        File.WriteAllText(Application.streamingAssetsPath + "/ABCompareInfo.txt", abCompareInfo);
        //刷新窗口
        AssetDatabase.Refresh();
    }
}
