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
                SSHPasswd.Text = configInnerSplit[5];
            }
        }
        private void InnerChanged()
        {
            localSettings.Values["SSHConfigIDTemp"] = 
              SSHConfigName.Text + "," + SSHCommand.Text + ","
            + SSHHost.Text + "," + SSHPort.Text + ","
            + SSHUser.Text + "," + SSHPasswd.Text;

            //if (localSettings.Values["DeveloperImpartIsOpen"] as string == "True")
            //{
            //    Test.Text = localSettings.Values["ConfigIDTemp"] as string;
            //    Test.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    Test.Visibility = Visibility.Collapsed;
            //}

        }
        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            // ÄÚÈÝ±ä¸ü
            InnerChanged();
        }
    }
}
