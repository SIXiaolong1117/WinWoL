// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

// Copyright (C) 2023  SI Xiaolong (https://github.com/Direct5dom) (SIXiaolong_GitHub@outlook.com)

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Globalization;
using Windows.Storage;
using PInvoke;
using Windows.Services.Maps;
using Windows.Networking;
using System.Net.Mail;
using Validation;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;

namespace WinWoL
{
    public sealed partial class WoL : Page
    {
        // 引入localSettings
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        // Selection需要的List
        public List<string> ConfigSelector { get; set; } = new List<string>()
        {
            "0","1","2","3","4","5","6","7","8","9","10"
        };

        // 页面初始化
        public WoL()
        {
            this.InitializeComponent();

            if (localSettings.Values["configNum"] == null)
            {
                configNum.SelectedItem = ConfigSelector[0];
                localSettings.Values["configNum"] = ConfigSelector[0];
                refresh("0");
            }
            else
            {
                configNum.SelectedItem = localSettings.Values["configNum"];
                refresh(localSettings.Values["configNum"].ToString());
            }
        }

        // 功能实现
        // 主刷新函数
        private void refresh(string ConfigIDNum)
        {
            // 读取localSettings中存储的字符串
            string configInner = localSettings.Values["ConfigID" + ConfigIDNum] as string;

            // 初始化界面元素
            ConfigName.Text = "配置别名：";
            MacAddress.Text = "主机 Mac：";
            IpAddress.Text = "WoL 主机地址：";
            IpPort.Text = "WoL 端口：";
            RDPIpAddress.Text = "RDP 主机地址：";
            RDPIpPort.Text = "RDP 端口：";
            RDPPing.Text = "RDP 端口延迟：";

            // 隐藏覆盖 显示导入
            ImportConfig.Visibility = Visibility.Visible;
            ImportAndReplaceConfig.Visibility = Visibility.Collapsed;

            AddConfig.Content = "添加";
            DelConfig.IsEnabled = false;
            RefConfig.IsEnabled = false;
            WoLConfig.IsEnabled = false;
            RDPConfig.IsEnabled = false;
            ExportConfig.IsEnabled = false;

            // 如果字符串不为空
            if (configInner != null)
            {
                // 修改界面UI可用性和文字显示
                AddConfig.Content = "修改";
                DelConfig.IsEnabled = true;
                WoLConfig.IsEnabled = true;
                ExportConfig.IsEnabled = true;

                // 隐藏导入 显示覆盖
                ImportConfig.Visibility = Visibility.Collapsed;
                ImportAndReplaceConfig.Visibility = Visibility.Visible;

                // 分割字符串
                string[] configInnerSplit = configInner.Split(',');
                // 传入的字符串结构：
                // configName.Text + "," + macAddress.Text + ","
                // + ipAddress.Text + "," + ipPort.Text + ","
                // + rdpIsOpen.IsOn + "," + rdpIpAddress.Text + "," + rdpIpPort;
                string configName = configInnerSplit[0];
                string macAddress = configInnerSplit[1];
                string ipAddress = configInnerSplit[2];
                string ipPort = configInnerSplit[3];
                string rdpIsOpen = configInnerSplit[4];
                string rdpIpAddress = configInnerSplit[5];
                string rdpPort = configInnerSplit[6];

                // 更新RDP的命令行
                localSettings.Values["mstscCMD"] = "mstsc /v:" + rdpIpAddress + ":" + rdpPort + ";";

                // 如果不是广播地址，则显示IP或域名。
                // 如果是广播地址，则显示“向 LAN 网络广播”
                string ipAddressDisplay = ipAddress;
                if (ipAddressDisplay == "255.255.255.255")
                {
                    ipAddressDisplay = "向 LAN 网络广播";
                }

                // 如果开启RDP
                if (rdpIsOpen == "True")
                {
                    ConfigName.Text = "配置别名：" + configName;
                    MacAddress.Text = "主机 Mac：" + macAddress;
                    IpAddress.Text = "WoL 主机地址：" + ipAddressDisplay;
                    IpPort.Text = "WoL 端口：" + ipPort;
                    RDPIpAddress.Text = "RDP 主机地址：" + rdpIpAddress;
                    RDPIpPort.Text = "RDP 端口：" + rdpPort;
                    RDPPing.Text = "RDP 端口延迟：未测试";

                    RefConfig.IsEnabled = true;
                    RDPConfig.IsEnabled = true;
                }
                // 没有开启RDP
                else
                {
                    ConfigName.Text = "配置别名：" + configName;
                    MacAddress.Text = "主机 Mac：" + macAddress;
                    IpAddress.Text = "WoL 主机地址：" + ipAddressDisplay;
                    IpPort.Text = "WoL 端口：" + ipPort;
                    RDPIpAddress.Text = "RDP 主机地址：未设置";
                    RDPIpPort.Text = "RDP 端口：未设置";
                    RDPPing.Text = "RDP 端口延迟：未设置";

                    RefConfig.IsEnabled = false;
                    RDPConfig.IsEnabled = false;
                }
            }
        }
        // 以UDP协议发送MagicPacket
        public void sendMagicPacket(string macAddress, string domain, int port)
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

            // 获取IP地址
            IPAddress ip = domain2ip(domain);

            // 多次发送，避免丢包
            for (int i = 0; i < 10; i++)
            {
                // 发送数据
                socket.SendTo(packet, new IPEndPoint(ip, port));
            }

            // 关闭Socket对象
            socket.Close();
        }
        static IPAddress domain2ip(string domain)
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
        // Ping测试函数
        static string PingTest(string domain, int port)
        {
            // 获取IP地址
            // 在这里执行这个操作，可以处理一些非法IP的输入问题（例如：广播地址 255.255.255.255）
            // 非法IP会被返回“主机地址无法联通”，而不会让pingSender报错导致应用崩溃
            IPAddress ipAddress = domain2ip(domain);

            // Ping实例对象
            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
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
        // 根据配置文件，调用发送MagicPacket
        private void WoLPC(string ConfigIDNum)
        {
            // 读取localSettings中的字符串
            string configInner = localSettings.Values["ConfigID" + ConfigIDNum] as string;
            // 如果字符串非空
            if (configInner != null)
            {
                // 分割字符串
                string[] configInnerSplit = configInner.Split(',');
                // configName.Text + "," + macAddress.Text + "," + ipAddress.Text + ":" + ipPort.Text;
                string macAddress = configInnerSplit[1];
                string ipAddress = configInnerSplit[2];
                string ipPort = configInnerSplit[3];

                // 尝试发送Magic Packet，成功打开已发送弹窗
                try
                {
                    sendMagicPacket(macAddress, ipAddress, int.Parse(ipPort));
                    MagicPacketIsSendTips.IsOpen = true;
                }
                // 失败打开发送失败弹窗
                catch
                {
                    MagicPacketNotSendTips.IsOpen = true;
                }
            }
        }
        // 唤起mstsc函数
        private void RDPPCChildThread()
        {
            // 创建一个新的进程
            Process process = new Process();
            // 指定运行PowerShell
            process.StartInfo.FileName = "PowerShell.exe";
            // 参数为唤起mstsc的参数
            // 他保存在localSettings中，随主刷新函数刷新
            process.StartInfo.Arguments = localSettings.Values["mstscCMD"] as string;
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
        // 删除配置
        private void delConfig(string ConfigIDNum)
        {
            // 清空指定ConfigIDNum的localSettings
            localSettings.Values["ConfigID" + ConfigIDNum] = null;
        }
        // Ping RDP主机端口
        private void PingRDPRef(string ConfigIDNum)
        {
            // 从localSettings中读取字符串
            string configInner = localSettings.Values["ConfigID" + ConfigIDNum] as string;
            // 如果字符串非空
            if (configInner != null)
            {
                // 分割字符串
                string[] configInnerSplit = configInner.Split(',');
                // 传入的字符串结构：
                // configName.Text + "," + macAddress.Text + ","
                // + ipAddress.Text + "," + ipPort.Text + ","
                // + rdpIsOpen.IsOn + "," + rdpIpAddress.Text + "," + rdpIpPort;
                string rdpIpAddress = configInnerSplit[5];
                string rdpPort = configInnerSplit[6];

                // 检查RDP主机端口是否可以Ping通
                try
                {
                    RDPPing.Text = "RDP 端口延迟：" + PingTest(rdpIpAddress, int.Parse(rdpPort)).ToString();
                }
                catch
                {
                    RDPPing.Text = "RDP 端口延迟：无法联通";
                }
            }
        }

        // 事件
        // Selection改变
        private void configNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            refresh(configNum.SelectedItem.ToString());
            localSettings.Values["configNum"] = configNum.SelectedItem;

            //关闭所有弹出窗口
            MagicPacketIsSendTips.IsOpen = false;
            MagicPacketNotSendTips.IsOpen = false;
            SaveConfigTips.IsOpen = false;
        }
        // 添加/修改配置按钮点击
        private async void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            // 将ConfigIDTemp所存储的字符串设置为当前配置所存储的字符串
            // 这样可以实现“修改”的操作
            localSettings.Values["ConfigIDTemp"] = localSettings.Values["ConfigID" + ConfigIDNum];

            // 创建一个新的dialog对象
            AddConfigDialog configDialog = new AddConfigDialog();

            // 对此dialog对象进行配置
            configDialog.XamlRoot = this.XamlRoot;
            configDialog.Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            // 根据内容有无来决定PrimaryButton的内容
            if (AddConfig.Content.ToString() == "修改配置")
            {
                configDialog.PrimaryButtonText = "修改";
            }
            else
            {
                configDialog.PrimaryButtonText = "添加";
            }
            configDialog.CloseButtonText = "关闭";
            // 默认按钮为PrimaryButton
            configDialog.DefaultButton = ContentDialogButton.Primary;

            // 异步获取按下哪个按钮
            var result = await configDialog.ShowAsync();

            // 如果按下了Primary
            if (result == ContentDialogResult.Primary)
            {
                // 将ConfigIDTemp写入到当前配置ID下的localSettings
                localSettings.Values["ConfigID" + ConfigIDNum] = localSettings.Values["ConfigIDTemp"];
                // 刷新UI
                refresh(ConfigIDNum);
            }
        }
        // Ping测试按钮点击
        private void RefConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            // Ping测试
            PingRDPRef(ConfigIDNum);
        }
        // 删除配置按钮点击
        private void DelConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            // 删除配置
            delConfig(ConfigIDNum);
            // 刷新UI
            refresh(ConfigIDNum);
            // 隐藏提示Flyout
            if (this.DelConfig.Flyout is Flyout f)
            {
                f.Hide();
            }
        }
        // 网络唤醒按钮点击
        private void WoLConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            // WoL配置指定的设备
            WoLPC(ConfigIDNum);
        }
        // 远程桌面按钮点击
        private void RDPConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            // 刷新RDP主机端口的Ping状态
            PingRDPRef(ConfigIDNum);

            // 启动一个子线程，防止UI卡死
            ThreadStart childref = new ThreadStart(RDPPCChildThread);
            Thread childThread = new Thread(childref);
            childThread.Start();
        }

        private async void ImportConfig_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();

            // Create a file picker
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            // 检索当前WinUI3窗口的窗口句柄 (HWND)
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".wolconfig");

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                //PickAPhotoOutputTextBlock.Text = "Picked photo: " + file.Name;
                var path = file.Path;
                localSettings.Values["ConfigID" + ConfigIDNum] = File.ReadAllText(path, Encoding.UTF8);
                // 刷新UI
                refresh(ConfigIDNum);
            }
            else
            {
                // 操作取消
            }

        }

        private async void ExportConfig_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            string configContent = localSettings.Values["ConfigID" + ConfigIDNum].ToString();

            //// 将配置内容保存在剪贴板
            //var package = new DataPackage();
            //package.SetText(configContent);
            //Clipboard.SetContent(package);

            // 创建一个FilePicker
            FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();

            // 检索当前WinUI3窗口的窗口句柄 (HWND)
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);

            // 使用窗口句柄 (HWND) 初始化文件选取器
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

            // 为FilePicker设置选项
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // 用户可以将文件另存为的文件类型下拉列表
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".wolconfig" });
            // 默认文件名，如果用户没有键入或选择要替换的文件
            var enteredFileName = ((sender as Button).Parent as StackPanel)
            .FindName("FileNameTextBox") as TextBox;
            savePicker.SuggestedFileName = "WinUI_Wake_on_LAN_BackUp_" + DateTime.Now.ToString();

            // 打开Picker供用户选择文件
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // 阻止更新文件的远程版本，直到我们完成更改并调用 CompleteUpdatesAsync。
                CachedFileManager.DeferUpdates(file);

                // 写入文件
                var textBox = ((sender as Button).Parent as StackPanel)
                .FindName("FileContentTextBox") as TextBox;
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    using (var tw = new StreamWriter(stream))
                    {
                        tw.WriteLine(configContent);
                    }
                }

                // 让Windows知道我们已完成文件更改，以便其他应用程序可以更新文件的远程版本。
                // 完成更新可能需要 Windows 请求用户输入。
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    // 文件保存成功
                    SaveConfigTips.Title = "保存成功！";
                    SaveConfigTips.IsOpen = true;
                }
                else if (status == FileUpdateStatus.CompleteAndRenamed)
                {
                    // 文件重命名并保存成功
                    SaveConfigTips.Title = "重命名并保存成功！";
                    SaveConfigTips.IsOpen = true;
                }
                else
                {
                    // 文件无法保存！
                    SaveConfigTips.Title = "无法保存！";
                    SaveConfigTips.IsOpen = true;
                }
            }
            else
            {
                // 操作取消
            }

        }
    }
}
