using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage;
using WinWoL.Models;

namespace WinWoL.Methods
{
    public class SSHMethod
    {
        // 导出配置
        public static async Task<string> ExportConfig(SSHModel sshModel)
        {
            // 创建一个FileSavePicker
            FileSavePicker savePicker = new FileSavePicker();
            // 获取当前窗口句柄 (HWND) 
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            // 使用窗口句柄 (HWND) 初始化FileSavePicker
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

            // 为FilePicker设置选项
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // 用户可以将文件另存为的文件类型下拉列表
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".sshconfigx" });
            // 如果用户没有选择文件类型，则默认为
            savePicker.DefaultFileExtension = ".sshconfigx";

            // 默认文件名
            savePicker.SuggestedFileName = sshModel.Name + "_BackUp_" + DateTime.Now.ToString();

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

                // 将数据序列化为 JSON 格式
                string jsonData = JsonConvert.SerializeObject(sshModel);

                // 写入文件
                await FileIO.WriteTextAsync(file, jsonData);

                // 让Windows知道我们已完成文件更改，以便其他应用程序可以更新文件的远程版本。
                // 完成更新可能需要Windows请求用户输入。
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    // 保存成功
                    return "保存成功";
                }
                else if (status == FileUpdateStatus.CompleteAndRenamed)
                {
                    // 重命名并保存成功
                    return "重命名并保存成功";
                }
                else
                {
                    // 文件无法保存！
                    return "无法保存！";
                }
            }
            return "错误！";
        }
        // 导入配置
        public static async Task<SSHModel> ImportConfig()
        {
            // 创建一个FileOpenPicker
            var openPicker = new FileOpenPicker();
            // 获取当前窗口句柄 (HWND) 
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            // 使用窗口句柄 (HWND) 初始化FileOpenPicker
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // 为FilePicker设置选项
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            // 建议打开位置 桌面
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            // 文件类型过滤器
            openPicker.FileTypeFilter.Add(".sshconfigx");

            // 打开选择器供用户选择文件
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // 读取 JSON 文件内容
                string jsonData = await FileIO.ReadTextAsync(file);
                // 反序列化JSON数据为WoLModel对象
                SSHModel importedData = JsonConvert.DeserializeObject<SSHModel>(jsonData);
                if (importedData != null)
                {
                    // 成功导入配置数据。 
                    return importedData;
                }
                else
                {
                    // JSON数据无法反序列化为配置数据。 
                    return null;
                }
            }
            else
            {
                // 未选择JSON文件。
                return null;
            }
        }
    }
}
