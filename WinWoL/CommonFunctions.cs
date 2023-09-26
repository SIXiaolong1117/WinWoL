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
using System.Diagnostics;

namespace WinWoL
{
    internal class CommonFunctions
    {
        // 导入配置
        public static async Task ImportConfig(string className, string configType, string configID)
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
            if (className == "WinWoL.WoL")
            {
                openPicker.FileTypeFilter.Add(".wolconfig");
            }
            else if (className == "WinWoL.SSH")
            {
                openPicker.FileTypeFilter.Add(".sshcmdconfig");
            }

            // 打开选择器供用户选择文件
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var path = file.Path;
                //localSettings.Values[configType + configID] = File.ReadAllText(path, Encoding.UTF8);
                // 将ConfigIDTemp所存储的字符串设置为当前配置所存储的字符串
                // 这样可以实现“修改”的操作
                localSettings.Values["ConfigIDTemp"] = File.ReadAllText(path, Encoding.UTF8);
            }
        }
        // 导出配置
        public static async Task<string> ExportConfig(string className, string configType, string configID,string configName)
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
            savePicker.SuggestedFileName = configName + "_BackUp_" + DateTime.Now.ToString();

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
        // 唤起RDP
        public static void RDPConnect(string arguments)
        {
            // 创建一个新的进程
            Process process = new Process();
            // 指定运行PowerShell
            process.StartInfo.FileName = "PowerShell.exe";
            // 参数为唤起mstsc的参数
            // 他保存在localSettings中，随主刷新函数刷新
            process.StartInfo.Arguments = arguments;
            //是否使用操作系统shell启动
            process.StartInfo.UseShellExecute = false;
            //是否在新窗口中启动该进程的值 (不显示程序窗口)
            process.StartInfo.CreateNoWindow = true;
            // 进程开始
            process.Start();
            // 等待执行结束
            process.WaitForExit();
            // 进程关闭
            process.Close();
        }
        // 获取域名对应的IP
        public static IPAddress domain2ip(string domain)
        {
            // 此函数本身可以处理部分非法IP（例如：266.266.266.266）
            // 这些非法IP会被算作域名来处理
            IPAddress ipAddress;
            if (IPAddress.TryParse(domain, out ipAddress))
            {
                // 是IP
                return IPAddress.Parse(domain);
            }
            else
            {
                // 是域名或其他输入
                return Dns.GetHostEntry(domain).AddressList[0];
            }
        }
        // 以UDP协议发送MagicPacket
        public static void sendMagicPacket(string macAddress, IPAddress ipAddress, int port)
        {
            // 将传入的Mac地址字符串分割为十六进制字符串数组
            // hexStrings = {"11", "22", "33", "44", "55", "66"}
            string s = macAddress;
            string[] hexStrings = s.Split(':');

            // 创建一个byte数组
            byte[] bytes = new byte[hexStrings.Length];
            // 遍历字符串数组，将每个字符串转换为byte值，并存储到byte数组中
            for (int i = 0; i < hexStrings.Length; i++)
            {
                // 使用16作为基数表示十六进制格式
                bytes[i] = Convert.ToByte(hexStrings[i], 16);
            }
            // 将MAC地址转换为字节数组：byte[] mac = new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 };
            byte[] mac = bytes;

            // 创建一个UDP Socket对象
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // 设置需要广播数据
            socket.EnableBroadcast = true;

            // 创建一个魔术包
            byte[] packet = new byte[17 * 6];
            // 填充前6个字节为0xFF
            for (int i = 0; i < 6; i++)
                packet[i] = 0xFF;
            // 填充后面16个重复的MAC地址字节
            for (int i = 1; i <= 16; i++)
                for (int j = 0; j < 6; j++)
                    packet[i * 6 + j] = mac[j];

            // 多次发送，避免丢包
            for (int i = 0; i < 10; i++)
            {
                // 发送数据
                socket.SendTo(packet, new IPEndPoint(ipAddress, port));
            }

            // 关闭Socket对象
            socket.Close();
        }
        // Ping测试函数
        public static string PingTest(string domain, int port)
        {
            // 获取IP地址
            // 在这里执行这个操作，可以处理一些非法IP的输入问题（例如：广播地址 255.255.255.255）
            // 非法IP会被返回“主机地址无法联通”，而不会让pingSender报错导致应用崩溃
            IPAddress ipAddress = CommonFunctions.domain2ip(domain);

            // Ping实例对象
            Ping pingSender = new Ping();
            // Ping选项
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "ping";
            byte[] buf = Encoding.ASCII.GetBytes(data);

            // 调用同步Send方法发送数据，结果存入reply对象;
            PingReply reply = pingSender.Send(ipAddress, 500, buf, options);
            // 判断replay，是否连通
            if (reply.Status == IPStatus.Success)
            {
                // 如果连通，尝试与指定端口通信
                var client = new TcpClient();
                if (!client.ConnectAsync(ipAddress, port).Wait(500))
                {
                    // 与指定端口通信失败
                    return "端口连接失败";
                }
                else
                {
                    // 与指定端口通信成功，计算RTT并返回
                    return reply.RoundtripTime.ToString() + " ms";
                }
            }
            else
            {
                // 无法联通
                return "无法联通";
            }
        }
        // SSH执行命令
        public static string SendSSHCommand(string sshCommand, string sshHost, string sshPort, string sshUser, string sshPasswdAndKey, string privateKeyIsOpen)
        {
            SshClient sshClient;

            if (privateKeyIsOpen == "True")
            {
                PrivateKeyFile privateKeyFile = new PrivateKeyFile(sshPasswdAndKey);
                ConnectionInfo connectionInfo = new ConnectionInfo(sshHost, sshUser, new PrivateKeyAuthenticationMethod(sshUser, new PrivateKeyFile[] { privateKeyFile }));
                sshClient = new SshClient(connectionInfo);
            }
            else
            {
                sshClient = new SshClient(sshHost, int.Parse(sshPort), sshUser, sshPasswdAndKey);
            }

            sshClient.Connect();

            if (sshClient.IsConnected)
            {
                SshCommand SSHCommand = sshClient.RunCommand(sshCommand);

                if (!string.IsNullOrEmpty(SSHCommand.Error))
                {
                    //SSHResponse.Subtitle = "Error: " + SSHCommand.Error;
                    //SSHResponse.IsOpen = true;
                    return "Error: " + SSHCommand.Error;
                }
                else
                {
                    //SSHResponse.Subtitle = SSHCommand.Result;
                    //SSHResponse.IsOpen = true;
                    return SSHCommand.Result;
                }
            }
            sshClient.Disconnect();
            return "0";
        }
    }
}