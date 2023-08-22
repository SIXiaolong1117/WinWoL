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
using System.Text.RegularExpressions;
using Validation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
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
                SSHKey.Text = configInnerSplit[7];
            }

            PrivateKeyIsOpenCheck();
        }
        private void InnerChanged()
        {
            localSettings.Values["SSHConfigIDTemp"] =
              SSHConfigName.Text + "," + SSHCommand.Text + ","
            + SSHHost.Text + "," + SSHPort.Text + ","
            + SSHUser.Text + "," + SSHPasswd.Password + ","
            + PrivateKeyIsOpen.IsOn + "," + SSHKey.Text;
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
                SSHKey.Text = "";
                SSHPasswd.Visibility = Visibility.Visible;
            }
        }
        private void PrivateKeyIsOpen_Toggled(object sender, RoutedEventArgs e)
        {
            PrivateKeyIsOpenCheck();
        }
    }
}
