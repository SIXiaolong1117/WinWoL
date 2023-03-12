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
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using Validation;
using Windows.Services.Maps;
using Windows.Storage;
using System.Net.Mail;

namespace WinWoL
{
    public sealed partial class AddPingDialog : ContentDialog
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public AddPingDialog()
        {
            this.InitializeComponent();

            string configInner = localSettings.Values["pingConfigTemp"] as string;
            if (configInner != null)
            {
                string[] configInnerSplit = configInner.Split(',');
                configName.Text = configInnerSplit[0];
                ipAddress.Text = configInnerSplit[1];
                ipPort.Text = configInnerSplit[2];
            }
        }
        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            localSettings.Values["pingConfigTemp"] = configName.Text + "," + ipAddress.Text + "," + ipPort.Text;
            Test.Text = localSettings.Values["pingConfigTemp"] as string;
        }
    }
}
