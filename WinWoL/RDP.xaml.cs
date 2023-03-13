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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Validation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;

namespace WinWoL
{
    public sealed partial class RDP : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public List<string> ConfigSelector { get; } = new List<string>()
        {
            "0","1","2","3","4","5","6","7","8","9","10"
        };
        public RDP()
        {
            this.InitializeComponent();

            if (localSettings.Values["rdpNum"] == null)
            {
                configNum.SelectedItem = ConfigSelector[0];
                localSettings.Values["rdpNum"] = ConfigSelector[0];
                refresh("0");
            }
            else
            {
                configNum.SelectedItem = localSettings.Values["rdpNum"];
                refresh(localSettings.Values["rdpNum"].ToString());
            }
        }
        private void configNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            refresh(configNum.SelectedItem.ToString());
            localSettings.Values["rdpNum"] = configNum.SelectedItem;
        }
        public void RDPPCChildThread()
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
        private void refresh(string ConfigIDNum)
        {
            List<RDPItem> items = new List<RDPItem>();
            string configInner = localSettings.Values["rdpConfig" + ConfigIDNum] as string;
            if (configInner != null)
            {
                string[] configInnerSplit = configInner.Split(',');
                // configName.Text + "," + ipAddress.Text + "," + ipPort.Text;
                string configName = configInnerSplit[0];
                string ipAddress = configInnerSplit[1];
                string ipPort = configInnerSplit[2];

                items.Add(new RDPItem(
                    "配置别名：" + configName,
                    "主机 IP：" + ipAddress,
                    "使用端口：" + ipPort
                    ));

                AddConfig.Content = "修改配置";
                DelConfig.IsEnabled = true;
                RDPConfig.IsEnabled = true;
            }
            else
            {
                items.Add(new RDPItem(
                    "配置别名：",
                    "主机 IP：",
                    "使用端口："
                    ));
                AddConfig.Content = "添加配置";
                DelConfig.IsEnabled = false;
                RDPConfig.IsEnabled = false;
            }
            MyGridView.ItemsSource = items;
        }
        private async void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string rdpNum = configNum.SelectedItem.ToString();
            localSettings.Values["rdpConfigTemp"] = localSettings.Values["rdpConfig" + rdpNum];

            AddRDPDialog configDialog = new AddRDPDialog();

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
                localSettings.Values["rdpConfig" + rdpNum] = localSettings.Values["rdpConfigTemp"];
                refresh(rdpNum);
            }
        }
        private void delConfig(string RDPNum)
        {
            localSettings.Values["rdpConfig" + RDPNum] = null;
        }
        private void DelConfigButton_Click(object sender, RoutedEventArgs e)
        {
            delConfig(configNum.SelectedItem.ToString());
            refresh(configNum.SelectedItem.ToString());
            if (this.DelConfig.Flyout is Flyout f)
            {
                f.Hide();
            }
        }
        private void RDPConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string configInner = localSettings.Values["rdpConfig" + configNum.SelectedItem.ToString()] as string;
            string[] configInnerSplit = configInner.Split(',');
            string ipAddress = configInnerSplit[1];
            string ipPort = configInnerSplit[2];
            localSettings.Values["mstscCMD"] = "mstsc /v:" + ipAddress + ":" + ipPort + ";";
            ThreadStart childref = new ThreadStart(RDPPCChildThread);
            Thread childThread = new Thread(childref);
            childThread.Start();
        }
    }
    public class RDPItem
    {
        // 主机名
        public string HostName { get; set; }
        // 主机IP
        public string HostIP { get; set; }
        // 往返时间
        public string HostPort { get; set; }


        public RDPItem(string hostName, string hostIP, string hostPort)
        {
            HostName = hostName;
            HostIP = hostIP;
            HostPort = hostPort;
        }
    }
}
