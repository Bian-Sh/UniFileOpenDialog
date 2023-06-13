using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using zFramework.IO;
using System.Linq;

public class OpenFileByWin32 : MonoBehaviour
{
    public Text text;

    public void OpenFile()
    {
        string title = "请选择打开的文件：";
        string msg = string.Empty;
            var paths = FileDialog.SelectFile(title, "应用程序|exe");
            if (paths?.Count>0)
            {
                msg = $"指定的文件路径为: \n{string.Join('\n', paths)}";
            }
            else
            {
                msg = "用户未作选择！";
            }
        text.text = msg;
    }

    public void SaveFile()
    {
        string title = "请选择保存的位置：";
        string msg = string.Empty;
        var file = Path.Combine(Application.streamingAssetsPath, "你要保存的文件名称.RAR");
        string path = FileDialog.SaveDialog(title,file,"压缩文件" );//假如你存rar文件。
        if (!string.IsNullOrEmpty(path))
        {
            msg = "保存文件的路径为: " + path;
        }
        else
        {
            msg = "用户取消保存！";
        }
        text.text = msg;
    }
}
