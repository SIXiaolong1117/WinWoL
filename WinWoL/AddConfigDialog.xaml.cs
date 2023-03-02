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

namespace WinWoL
{
    public sealed partial class AddConfigDialog : ContentDialog
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public AddConfigDialog()
        {
            this.InitializeComponent();
        }
        public void checkInput()
        {
            if (configNum.Text == null)
                configNum.Text = "0";
            if (macAddress.Text == null)
                macAddress.Text = "11:22:33:44";
            if (ipAddress.Text == null)
                ipAddress.Text = "127.0.0.1";
            if (ipPort.Text == null)
                ipPort.Text = "9";
        }
        public void saveConfigID()
        {
            checkInput();
            localSettings.Values["ConfigID" + configNum.Text] = configNum.Text + "," + macAddress.Text + "," + ipAddress.Text + "," + ipPort.Text;
        }
        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            saveConfigID();
            Test.Text = localSettings.Values["ConfigID" + configNum.Text] as string;
        }
    }
}