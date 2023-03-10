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
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace WinWoL
{
    public sealed partial class Ping : Page
    {
        public Ping()
        {
            this.InitializeComponent();
        }
        public void pingTest(string pingHostPort)
        {
            // 将传入的字符串分裂成 IP/域名 和 端口
            string[] pingHostPortSplit = pingHostPort.Split(':');
            string pingHost = pingHostPortSplit[0];
            int port = int.Parse(pingHostPortSplit[1]);
            // Ping 实例对象
            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
            // Ping 选项
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "ping test data";
            byte[] buf = Encoding.ASCII.GetBytes(data);
            // 调用同步 Send 方法发送数据，结果存入 reply 对象;
            PingReply reply = pingSender.Send(pingHost, 120, buf, options);
            // 判断 replay，是否连通
            if (reply.Status == IPStatus.Success)
            {
                // 绘制模板
                List<Item> items = new List<Item>();
                items.Add(new Item(
                    "主机名：" + pingHost,
                    "主机IP：" + reply.Address.ToString(),
                    "往返时间RTT：" + reply.RoundtripTime.ToString() + " ms",
                    port + " 端口开放情况：" + checkPortEnable(pingHost, port).ToString()
                    ));
                MyGridView.ItemsSource = items;
            }
        }
        private bool checkPortEnable(string _ip, int _port)
        {
            //将IP和端口替换成为你要检测的
            string ipAddress = _ip;
            int portNum = _port;

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

            IPEndPoint point = new IPEndPoint(ip, portNum);

            bool _portEnable = false;
            try
            {
                using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    sock.Connect(point);
                    sock.Close();

                    _portEnable = true;
                }
            }
            catch
            {
                _portEnable = false;
            }
            return _portEnable;
        }

        private void pingTestButton_Click(object sender, RoutedEventArgs e)
        {
            pingTest(ipAddress.Text + ":" + ipPort.Text);
        }
    }



    public class Item
    {
        // 主机名
        public string HostName { get; set; }
        // 主机IP
        public string HostIP { get; set; }
        // 往返时间
        public string RTT { get; set; }
        // 端口是否开放
        public string PortIsOpen { get; set; }


        public Item(string hostName, string hostIP, string rTT, string portIsOpen)
        {
            HostName = hostName;
            HostIP = hostIP;
            RTT = rTT;
            PortIsOpen = portIsOpen;
        }
    }
}
