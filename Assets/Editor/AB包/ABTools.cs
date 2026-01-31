
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ABTools : EditorWindow
{
    private int nowSelIndex = 0;
    private string[] targetStrings = new string[] { "PC", "IOS", "ANDROID" };

    private string serverIP = "ftp://127.0.0.1";
    [MenuItem("AB包工具/打开工具窗口")]
   private static void OpenWindow()
    {
       ABTools windown= EditorWindow.GetWindowWithRect(typeof(ABTools),new Rect(0,0,350,220)) as ABTools;
        windown.Show();

    }
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 150, 15), "平台选择");
        //页签显示 是从数组中取出字符串内容来显示 所以需要改变当前选中的索引
        nowSelIndex=GUI.Toolbar(new Rect(10, 30, 250, 20),nowSelIndex,targetStrings);
        //资源服务器IP地址设置
        GUI.Label(new Rect(10, 60, 150, 15), "资源服务器地址");
        GUI.TextField(new Rect(10, 80, 150, 20), serverIP);
        //创建对比文件按钮
        if(GUI.Button(new Rect(10, 110, 100, 40), "创建对比文件"))
        {
            CreatABCompareFile();   
        }
        //保存默认资源到StreamingAssets按钮
        if(GUI.Button(new Rect(115, 110, 225, 40), "保存默认资源到StreamingAssets"))
        {
            MoveABToStreamingAssets();
        }
        //上传AB包和对比文件按钮
        if (GUI.Button(new Rect(10, 160, 330, 40), "上传AB包和对比文件"))
        {
            UplodaAllABFile();
        }
    }
   
    //创建AB包对比文件
    public  void CreatABCompareFile()
    {
        //获取文件夹信息
        //根据选择的平台 读取对应平台文件夹下的内容 来进行对比文件的生成
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/" + targetStrings[nowSelIndex]);
        //获取该目录下的所有文件信息
        FileInfo[] fileInfos = directory.GetFiles();
        //用于存储信息的字符串
        string abCompareInfo = "";
        foreach (FileInfo info in fileInfos)
        {
            //没有后缀的才是AB包 我们只想要AB包的信息
            if (info.Extension == "")
            {
                Debug.Log("文件名：" + info.Name);
                //拼接一个AB包的信息
                abCompareInfo += info.Name + " " + info.Length + " " + GetMD5(info.FullName);
                //用一个分隔符分开不同文件之间的信息
                abCompareInfo += '|';
            }
        }
        //因为循环完毕后，会在最后有一个|符号 所以去掉
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);
        Debug.Log(abCompareInfo);
        //存储拼接好的AB包资源信息
        File.WriteAllText(Application.dataPath + "/ArtRes/AB/"+ targetStrings[nowSelIndex]+"/ABCompareInfo.txt", abCompareInfo);
        //刷新编辑器
        AssetDatabase.Refresh();
        Debug.Log("AB包对比文件生成成功");
    }
    //获取文件MD5码
    public  string GetMD5(string filePath)
    {
        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            //声明一个MD5对象 用于生成MD5码
            MD5 md5 = new MD5CryptoServiceProvider();
            //利用API得到数据的MD5码 16个字节数组
            byte[] md5info = md5.ComputeHash(file);
            //关闭文件流
            file.Close();
            //把16个字节转换为16进制 拼接成字符串 为了减效md5码的长度
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5info.Length; i++)
            {
                sb.Append(md5info[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
    //将选中资源移动到StreamingAssets文件夹中
    private  void MoveABToStreamingAssets()
    {
        //通过编辑器Selection类中的放啊发  获取再Project窗口中选中的资源
        Object[] selectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (selectedAsset.Length == 0) return;
        //用于拼接本地默认AB包资源信息的字符串
        string abCompareInfo = "";
        foreach (Object asset in selectedAsset)
        {
            //通过Assetdatabase类 获取资源的路径
            string assetPath = AssetDatabase.GetAssetPath(asset);//Assets/ArtRes/AB/PC/lua.manifest
            Debug.Log(assetPath);
            //截取路径中的文件名  截取从后往前的第一个/
            string fileName = assetPath.Substring(assetPath.LastIndexOf('/'));//
            //判断是否有.符号 如果有 证明有后缀 不处理
            if (fileName.IndexOf('.') != -1)
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
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);
        //将本地默认资源的对比信息 存入文件
        File.WriteAllText(Application.streamingAssetsPath + "/ABCompareInfo.txt", abCompareInfo);
        //刷新窗口
        AssetDatabase.Refresh();
    }
    //上传AB包文件到服务器
    private  void UplodaAllABFile()
    {
        //获取文件夹信息
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/" + targetStrings[nowSelIndex] +"/");
        //获取该目录下的所有文件信息
        FileInfo[] fileInfos = directory.GetFiles();

        foreach (FileInfo info in fileInfos)
        {
            //没有后缀的才是AB包 我们只想要AB包的信息
            //还要获取 资源对比文件 后缀是txt（该文件夹中 只有对比文件是txt格式
            if (info.Extension == "" || info.Extension == ".txt")
            {
                //上传该文件
                FtpUploadFile(info.FullName, info.Name);
            }
        }
    }
    //异步上传文件
    private async  void FtpUploadFile(string filePath, string fileName)
    {
        await Task.Run(() =>
        {
            try
            {
                //1.创建一个ftp链接 用于上传
                FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP +"/AB/"+ targetStrings[nowSelIndex]+"/"+fileName)) as FtpWebRequest;
                //2.设置一个通信凭证 这样才能上传
                NetworkCredential n = new NetworkCredential("黄前久美子", "12345");
                req.Credentials = n;
                //3. 其他设置
                // 设置代理为null
                req.Proxy = null;
                //请求完毕后是否关闭控制连接
                req.KeepAlive = false;
                // 操作命令- 上传
                req.Method = WebRequestMethods.Ftp.UploadFile;
                // 指定传输的类型 2进制
                req.UseBinary = true;
                //4.上传文件
                //ftp的流对象
                Stream upLoadStream = req.GetRequestStream();
                // 读取文件信息 写入该流对象
                using (FileStream file = File.OpenRead(filePath))
                {
                    //一点一点地上传内容
                    byte[] bytes = new byte[2048];
                    //返回值代表 读取了多少个字节
                    int contentLength = file.Read(bytes, 0, bytes.Length);
                    //循环上传文件中地数据
                    while (contentLength != 0)
                    {
                        //写入到上传流中
                        upLoadStream.Write(bytes, 0, contentLength);
                        //写完再读
                        contentLength = file.Read(bytes, 0, bytes.Length);
                    }
                    //循环完毕后 证明上传结束
                    file.Close();
                    upLoadStream.Close();

                }
                Debug.Log(fileName + "上传成功");
            }
            catch (Exception ex)
            {
                Debug.Log(fileName+"上传失败" + ex.Message);
                throw;
            }
        });

    }
}
