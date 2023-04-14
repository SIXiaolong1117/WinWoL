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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage;
using System.Text.RegularExpressions;

namespace WinWoL
{
    public sealed partial class AddConfigDialog : ContentDialog
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public AddConfigDialog()
        {
            this.InitializeComponent();

            string configInner = localSettings.Values["ConfigIDTemp"] as string;
            if (configInner != null)
            {
                string[] configInnerSplit = configInner.Split(',');
                configName.Text = configInnerSplit[0];
                macAddress.Text = configInnerSplit[1];
                ipAddress.Text = configInnerSplit[2];
                ipPort.Text = configInnerSplit[3];
                rdpIsOpen.IsOn = configInnerSplit[4] == "True";
                rdpIpAddress.Text = configInnerSplit[5];
                rdpIpPort.Text = configInnerSplit[6];
                Broadcast.IsChecked = configInnerSplit[7] == "True";
                SameIPAddr.IsChecked = configInnerSplit[8] == "True";
            }

            Test.Visibility = Visibility.Collapsed;
            redIsOpenCheck();
        }
        private void InnerChanged()
        {
            localSettings.Values["ConfigIDTemp"] = configName.Text + "," + macAddress.Text + ","
            + ipAddress.Text + "," + ipPort.Text + ","
            + rdpIsOpen.IsOn + "," + rdpIpAddress.Text + "," + rdpIpPort.Text + ","
            + Broadcast.IsChecked + "," + SameIPAddr.IsChecked;

            if (localSettings.Values["DeveloperImpartIsOpen"] as string == "True")
            {
                Test.Text = localSettings.Values["ConfigIDTemp"] as string;
                Test.Visibility = Visibility.Visible;
            }
            else
            {
                Test.Visibility = Visibility.Collapsed;
            }
        }
        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            // 获取TextBox的当前文本
            var text = ((TextBox)sender).Text;
            // 使用正则表达式排除英文逗号","
            // 因为该符号在代码逻辑中用作分割
            var regex = new Regex(",");
            // 如果文本与正则表达式匹配，即存在英文逗号，则撤消更改、弹出提示窗
            if (regex.IsMatch(text))
            {
                ((TextBox)sender).Undo();
                RegularErrorTips.IsOpen = true;
            }

            // 内容变更
            InnerChanged();
        }
        private void Broadcast_Checked(object sender, RoutedEventArgs e)
        {
            ipAddress.Text = "255.255.255.255";
            ipAddress.IsEnabled = false;
            SameIPAddr.IsEnabled = false;
            rdpIpAddress.IsEnabled = true;
            SameIPAddr.IsChecked = false;
            InnerChanged();
        }
        private void Broadcast_Unchecked(object sender, RoutedEventArgs e)
        {
            ipAddress.IsEnabled = true;
            SameIPAddr.IsEnabled = true;
            InnerChanged();
        }

        private void SameIPAddr_Checked(object sender, RoutedEventArgs e)
        {
            rdpIpAddress.Text = ipAddress.Text;
            rdpIpAddress.IsEnabled = false;
            InnerChanged();
        }
        private void SameIPAddr_Unchecked(object sender, RoutedEventArgs e)
        {
            rdpIpAddress.IsEnabled = true;
            InnerChanged();
        }
        private void rdpIsOpen_Toggled(object sender, RoutedEventArgs e)
        {
            redIsOpenCheck();
            InnerChanged();
        }
        private void redIsOpenCheck()
        {
            if (rdpIsOpen.IsOn == true)
            {
                RDPSettings.Visibility = Visibility.Visible;
            }
            else
            {
                RDPSettings.Visibility = Visibility.Collapsed;
            }
        }
    }
}