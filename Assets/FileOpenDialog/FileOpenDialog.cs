
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class FileOpenDialog
{
    #region Config Field
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
    #endregion

    public FileOpenDialog()
    {
        structSize = Marshal.SizeOf(this);
        dlgOwner = GetForegroundWindow();
    }

    #region Win32API WRAP
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    static extern bool GetOpenFileName([In, Out]FileOpenDialog dialog);  //这个方法名称必须为GetOpenFileName
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    static extern bool GetSaveFileName([In, Out]FileOpenDialog dialog);  //这个方法名称必须为GetSaveFileName
    #endregion

    /// <summary>
    /// 获得选择的文件的完整路径
    /// </summary>
    /// <param name="ofn">文件选择窗配置</param>
    /// <returns>文件路径</returns>
    public static bool GetOpenFilePath(FileOpenDialog ofn)
    {
        return GetOpenFileName(ofn);
    }
    /// <summary>
    /// 获得用户保存文件的完整路径
    /// </summary>
    /// <param name="ofn">文件选择窗配置</param>
    /// <returns>文件路径</returns>
    public static bool GetSaveFilePath(FileOpenDialog ofn)
    {
        return GetSaveFileName(ofn);
    }
}

