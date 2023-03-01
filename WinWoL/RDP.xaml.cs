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
using Windows.Storage;

namespace WinWoL
{
    public sealed partial class RDP : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public RDP()
        {
            this.InitializeComponent();
        }
        private void RDPPCButton_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["mstscCMD"] = "mstsc /v:" + ipAddress.Text + ":" + ipPort.Text + ";";
            ThreadStart childref = new ThreadStart(RDPPCChildThread);
            Thread childThread = new Thread(childref);
            childThread.Start();
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
    }
}
