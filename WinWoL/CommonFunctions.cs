using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.IO;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Validation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage.Provider;
using Renci.SshNet;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.Mail;
using Microsoft.UI.Dispatching;
using System.Reflection;
using PInvoke;

namespace WinWoL
{
    internal class CommonFunctions
    {
        public static async Task ImportConfig(string who, string configType, string configID)
        {
            // 引入localSettings
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            // 创建一个FileOpenPicker
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            // 获取当前窗口句柄 (HWND) 
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            // 使用窗口句柄 (HWND) 初始化FileOpenPicker
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // 为FilePicker设置选项
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            // 建议打开位置 桌面
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            // 文件类型过滤器
            if (who == "WoL")
            {
                openPicker.FileTypeFilter.Add(".wolconfig");
            }
            else if (who == "SSH")
            {
                openPicker.FileTypeFilter.Add(".sshcmdconfig");
            }

            // 打开选择器供用户选择文件
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var path = file.Path;
                localSettings.Values[configType + configID] = File.ReadAllText(path, Encoding.UTF8);
            }
        }
        public static async Task<string> ExportConfig(string className, string configType, string configID)
        {
            // 引入localSettings
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            string configContent = localSettings.Values[configType + configID].ToString();
            // 创建一个FileSavePicker
            FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
            // 获取当前窗口句柄 (HWND) 
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            // 使用窗口句柄 (HWND) 初始化FileSavePicker
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

            // 为FilePicker设置选项
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            if (className == "WinWoL.WoL")
            {
                // 用户可以将文件另存为的文件类型下拉列表
                savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".wolconfig" });
                // 如果用户没有选择文件类型，则默认为
                savePicker.DefaultFileExtension = ".wolconfig";
            }
            else if (className == "WinWoL.SSH")
            {
                // 用户可以将文件另存为的文件类型下拉列表
                savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".sshcmdconfig" });
                // 如果用户没有选择文件类型，则默认为
                savePicker.DefaultFileExtension = ".sshcmdconfig";
            }
            // 默认文件名
            savePicker.SuggestedFileName = configType + configID + "_BackUp_" + DateTime.Now.ToString();

            // 打开Picker供用户选择文件
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                try
                {
                    // 阻止更新文件的远程版本，直到我们完成更改并调用 CompleteUpdatesAsync。
                    CachedFileManager.DeferUpdates(file);
                }
                catch
                {
                    // 当您保存至OneDrive等同步盘目录时，在Windows11上可能引起DeferUpdates错误，备份文件不一定写入正确。
                    return "保存行为完成，但当您保存至OneDrive等同步盘目录时，在Windows11上可能引起DeferUpdates错误，备份文件不一定写入正确。";
                }

                // 写入文件
                await FileIO.WriteTextAsync(file, configContent);

                // 让Windows知道我们已完成文件更改，以便其他应用程序可以更新文件的远程版本。
                // 完成更新可能需要Windows请求用户输入。
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    // 文件保存成功
                    return "文件保存成功";
                }
                else if (status == FileUpdateStatus.CompleteAndRenamed)
                {
                    // 文件重命名并保存成功
                    return "文件重命名并保存成功";
                }
                else
                {
                    // 文件无法保存！
                    return "无法保存！";
                }
            }
            return "错误！";
        }
    }
}