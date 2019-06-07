
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class OpenFileByWin32 : MonoBehaviour
{
    public string DialogTitle = "请选择文件";
    public int maxPathCharacter = 256;
    public int maxTitleCharacter = 64;
    public string fileExtension = "exe";

    public Text text;

    public FileOpenDialog CreatDialogConig()
    {
        return new FileOpenDialog()
        {
            filter = "exe files\0*.exe\0All Files\0*.*\0\0",
            maxFile = maxPathCharacter,
            file = new string(new char[maxPathCharacter]),
            maxFileTitle = maxTitleCharacter,
            fileTitle = new string(new char[maxTitleCharacter]),
            initialDir = UnityEngine.Application.dataPath, //默认路径
            title = DialogTitle,
            defExt = fileExtension,//显示文件的类型
                                   //注意一下项目不一定要全选 但是0x00000008项不要缺少
            flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008  //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
        };
    }

    public void OpenFile()
    {
        FileOpenDialog dialog = CreatDialogConig();
        dialog.title = "请选择打开的文件：";
        string msg = string.Empty;
        if (FileOpenDialog.GetOpenFilePath(dialog))
        {
            msg = "指定的文件路径为: " + dialog.file;
        }
        else
        {
            msg = "用户未作选择！";
        }
        text.text = msg;
    }
    public void SaveFile()
    {
        FileOpenDialog dialog = CreatDialogConig();
        dialog.title = "请选择保存的位置：";
        string msg = string.Empty;
        if (FileOpenDialog.GetSaveFilePath(dialog))
        {
            msg = "保存文件的路径为: " + dialog.file;
        }
        else
        {
            msg = "用户取消保存！";
        }
        text.text = msg;
    }
}
