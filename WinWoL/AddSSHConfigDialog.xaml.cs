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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Validation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.Storage.Pickers;
using static System.Net.Mime.MediaTypeNames;

namespace WinWoL
{
    public sealed partial class AddSSHConfigDialog : ContentDialog
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public AddSSHConfigDialog()
        {
            this.InitializeComponent();

            string configInner = localSettings.Values["SSHConfigIDTemp"] as string;
            if (configInner != null)
            {
                string[] configInnerSplit = configInner.Split(',');

                SSHConfigName.Text = configInnerSplit[0];
                SSHCommand.Text = configInnerSplit[1];
                SSHHost.Text = configInnerSplit[2];
                SSHPort.Text = configInnerSplit[3];
                SSHUser.Text = configInnerSplit[4];
                SSHPasswd.Password = configInnerSplit[5];
                PrivateKeyIsOpen.IsOn = configInnerSplit[6] == "True";
                SSHKeyPath.Text = configInnerSplit[7];
            }

            PrivateKeyIsOpenCheck();
        }
        private void InnerChanged()
        {
            localSettings.Values["SSHConfigIDTemp"] =
              SSHConfigName.Text + "," + SSHCommand.Text + ","
            + SSHHost.Text + "," + SSHPort.Text + ","
            + SSHUser.Text + "," + SSHPasswd.Password + ","
            + PrivateKeyIsOpen.IsOn + "," + SSHKeyPath.Text;
        }
        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            // 内容变更
            InnerChanged();
        }
        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            // 内容变更
            InnerChanged();
        }
        private void PrivateKeyIsOpenCheck()
        {
            if (PrivateKeyIsOpen.IsOn == true)
            {
                SSHKey.Visibility = Visibility.Visible;
                SSHPasswd.Visibility = Visibility.Collapsed;
                SSHPasswd.Password = "";
            }
            else
            {
                SSHKey.Visibility = Visibility.Collapsed;
                SSHKeyPath.Text = "";
                SSHPasswd.Visibility = Visibility.Visible;
            }
        }
        private void PrivateKeyIsOpen_Toggled(object sender, RoutedEventArgs e)
        {
            PrivateKeyIsOpenCheck();
        }

        private async void SelectSSHKeyPath_Click(object sender, RoutedEventArgs e)
        {
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
            openPicker.FileTypeFilter.Add("*");

            // 打开选择器供用户选择文件
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                SSHKeyPath.Text = file.Path;
            }
        }
    }
}
