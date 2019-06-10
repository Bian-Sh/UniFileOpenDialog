
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.IO;

public class OpenFileByWin32 : MonoBehaviour
{
    public Text text;


    public void OpenFile()
    {
        string title = "请选择打开的文件：";
        string msg = string.Empty;
        string path = FileDialogForWindows.FileDialog(title, "exe");
        if (!string.IsNullOrEmpty(path))
        {
            msg = "指定的文件路径为: " +path;
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
        string path = FileDialogForWindows.SaveDialog(title, Path.Combine( Application.streamingAssetsPath,"你要保存的文件名称.RAR"));//假如你存rar文件。
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
