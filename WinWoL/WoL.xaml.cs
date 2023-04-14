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

namespace WinWoL
{
    public sealed partial class WoL : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public List<string> ConfigSelector { get; } = new List<string>()
        {
            "0","1","2","3","4","5","6","7","8","9","10"
        };

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
            List<ConfigItem> items = new List<ConfigItem>();
            string configInner = localSettings.Values["ConfigID" + ConfigIDNum] as string;
            if (configInner != null)
            {
                string[] configInnerSplit = configInner.Split(',');
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
                localSettings.Values["mstscCMD"] = "mstsc /v:" + rdpIpAddress + ":" + rdpPort + ";";

                // 如果开启RDP
                if (rdpIsOpen == "True")
                {
                    items.Add(new ConfigItem(
                        "配置别名：" + configName,
                        "主机 Mac：" + macAddress,
                        "主机 IP：" + ipAddress,
                        "使用端口：" + ipPort,
                        "RDP 主机 IP：" + rdpIpAddress,
                        "RDP 主机端口：" + rdpPort,
                        "RDP 端口延迟：" + PingTest(rdpIpAddress, int.Parse(rdpPort)).ToString()
                        ));
                }
                else
                {
                    items.Add(new ConfigItem(
                        "配置别名：" + configName,
                        "主机 Mac：" + macAddress,
                        "主机 IP：" + ipAddress,
                        "使用端口：" + ipPort,
                        "RDP 主机 IP：未设置",
                        "RDP 主机端口：未设置",
                        "RDP 端口延迟：未设置"
                        ));
                }

                AddConfig.Content = "修改配置";
                DelConfig.IsEnabled = true;
                WoLConfig.IsEnabled = true;
                RDPConfig.IsEnabled = true;
            }
            else
            {
                items.Add(new ConfigItem(
                    "配置别名：",
                    "主机 Mac：",
                    "主机 IP：",
                    "使用端口：",
                    "RDP 主机 IP：",
                    "RDP 主机端口：",
                    "RDP 端口延迟："
                    ));
                AddConfig.Content = "添加配置";
                DelConfig.IsEnabled = false;
                WoLConfig.IsEnabled = false;
                RDPConfig.IsEnabled = false;
            }
            MyGridView.ItemsSource = items;
        }
        // 以UDP协议发送MagicPacket
        public void sendMagicPacket(string macAddress, string domain, int port)
        {
            // 将string分割为十六进制字符串数组
            string s = macAddress;
            // hexStrings = {"11", "22", "33", "44", "55", "66"}
            string[] hexStrings = s.Split(':');
            // 创建一个byte数组
            byte[] bytes = new byte[hexStrings.Length];
            // 遍历字符串数组，将每个字符串转换为byte值，并存储到byte数组中
            for (int i = 0; i < hexStrings.Length; i++)
            {
                // 使用16作为基数表示十六进制格式
                bytes[i] = Convert.ToByte(hexStrings[i], 16);
            }
            // 创建一个UDP Socket对象
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // 设置需要广播数据
            socket.EnableBroadcast = true;
            // 将MAC地址转换为字节数组：byte[] mac = new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 };
            byte[] mac = bytes;
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
            IPAddress ip;
            if (IPAddress.TryParse(domain, out ip))
            {
                // 是IP
                ip = IPAddress.Parse(domain);
            }
            else
            {
                // 是域名
                ip = Dns.GetHostEntry(domain).AddressList[0];
            }
            // 发送数据
            socket.SendTo(packet, new IPEndPoint(ip, port));
            // 关闭Socket对象
            socket.Close();
        }
        // Ping测试函数
        static string PingTest(string ipAddress, int port)
        {
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
                // 获取IP地址
                IPAddress ip;
                if (IPAddress.TryParse(ipAddress, out ip))
                {
                    // 是IP
                    ip = IPAddress.Parse(ipAddress);
                }
                else
                {
                    // 是域名
                    ip = Dns.GetHostEntry(ipAddress).AddressList[0];
                }

                var client = new TcpClient();
                if (!client.ConnectAsync(ip, port).Wait(500))
                {
                    //连接失败
                    return "端口连接失败";
                }
                return reply.RoundtripTime.ToString() + " ms";
            }
            else
            {
                return "未联通";
            }

        }
        // 根据配置文件，调用发送MagicPacket
        private void WoLPC(string ConfigIDNum)
        {
            string configInner = localSettings.Values["ConfigID" + ConfigIDNum] as string;
            if (configInner != null)
            {
                string[] configInnerSplit = configInner.Split(',');
                // configName.Text + "," + macAddress.Text + "," + ipAddress.Text + ":" + ipPort.Text;
                string macAddress = configInnerSplit[1];
                string ipAddress = configInnerSplit[2];
                string ipPort = configInnerSplit[3];
                if ((macAddress != "") && (ipAddress != "") && (ipPort != ""))
                {
                    sendMagicPacket(macAddress, ipAddress, int.Parse(ipPort));
                    MagicPacketIsSendTips.IsOpen = true;
                }
                else
                {
                    MagicPacketNotSendTips.IsOpen = true;
                }

            }
        }
        // 唤起mstsc函数
        private void RDPPCChildThread()
        {
            Process process = new Process();
            process.StartInfo.FileName = "PowerShell.exe";
            process.StartInfo.Arguments = localSettings.Values["mstscCMD"] as string;
            //是否使用操作系统shell启动
            process.StartInfo.UseShellExecute = false;
            //是否在新窗口中启动该进程的值 (不显示程序窗口)
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
            process.Close();
        }
        // 删除配置
        private void delConfig(string ConfigIDNum)
        {
            localSettings.Values["ConfigID" + ConfigIDNum] = null;
        }

        // 事件
        // Selection改变
        private void configNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            refresh(configNum.SelectedItem.ToString());
            localSettings.Values["configNum"] = configNum.SelectedItem;
        }
        // 添加/修改配置按钮点击
        private async void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            localSettings.Values["ConfigIDTemp"] = localSettings.Values["ConfigID" + ConfigIDNum];

            AddConfigDialog configDialog = new AddConfigDialog();

            configDialog.XamlRoot = this.XamlRoot;
            configDialog.Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            if (AddConfig.Content.ToString() == "修改配置")
            {
                configDialog.PrimaryButtonText = "修改";
            }
            else
            {
                configDialog.PrimaryButtonText = "添加";
            }
            configDialog.CloseButtonText = "关闭";
            configDialog.DefaultButton = ContentDialogButton.Primary;

            var result = await configDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                localSettings.Values["ConfigID" + ConfigIDNum] = localSettings.Values["ConfigIDTemp"];
                refresh(ConfigIDNum);
            }
        }
        // 刷新配置按钮点击
        private void RefConfigButton_Click(object sender, RoutedEventArgs e)
        {
            refresh(configNum.SelectedItem.ToString());
        }
        // 删除配置按钮点击
        private void DelConfigButton_Click(object sender, RoutedEventArgs e)
        {
            delConfig(configNum.SelectedItem.ToString());
            refresh(configNum.SelectedItem.ToString());
            if (this.DelConfig.Flyout is Flyout f)
            {
                f.Hide();
            }
        }
        // 网络唤醒按钮点击
        private void WoLConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            WoLPC(ConfigIDNum);
        }
        // 远程桌面按钮点击
        private void RDPConfigButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadStart childref = new ThreadStart(RDPPCChildThread);
            Thread childThread = new Thread(childref);
            childThread.Start();
        }
    }
    // ConfigItem类
    public class ConfigItem
    {
        // 配置文件ID
        public string ConfigName { get; set; }
        // 主机MAC
        public string MacAddress { get; set; }
        // 主机IP
        public string IpAddress { get; set; }
        // 主机端口
        public string IpPort { get; set; }
        // RDP主机IP
        public string RDPIpAddress { get; set; }
        // RDP主机端口
        public string RDPIpPort { get; set; }
        // RDP主机端口Ping
        public string RDPPing { get; set; }

        public ConfigItem(string configName, string macAddress, string ipAddress, string ipPort, string rdpIpAddress, string rdpIpPort, string rdpPing)
        {
            ConfigName = configName;
            MacAddress = macAddress;
            IpAddress = ipAddress;
            IpPort = ipPort;
            RDPIpAddress = rdpIpAddress;
            RDPIpPort = rdpIpPort;
            RDPPing = rdpPing;
        }
    }
}
