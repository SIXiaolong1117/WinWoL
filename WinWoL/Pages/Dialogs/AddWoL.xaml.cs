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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using WinWoL.Datas;
using WinWoL.Models;

namespace WinWoL.Pages.Dialogs
{
    public sealed partial class AddWoL : ContentDialog
    {
        public WoLModel WoLData { get; private set; }
        public AddWoL(WoLModel wolModel)
        {
            this.InitializeComponent();
            PrimaryButtonClick += MyDialog_PrimaryButtonClick;
            SecondaryButtonClick += MyDialog_SecondaryButtonClick;

            // 初始化Dialog中的字段，使用传入的WoLModel对象的属性
            WoLData = wolModel;
            ConfigNameTextBox.Text = wolModel.Name;
            IpAddressTextBox.Text = wolModel.IPAddress;
            BroadcastCheckBox.IsChecked = wolModel.BroadcastIsOpen == "True";
            WoLIsOpenToggleSwitch.IsOn = wolModel.WoLIsOpen == "True";
            MacAddressTextBox.Text = wolModel.MacAddress;
            WoLPortTextBox.Text = wolModel.WoLPort;
            RDPIsOpenToggleSwitch.IsOn = wolModel.RDPIsOpen == "True";
            RDPIPPortTextBox.Text = wolModel.RDPPort;
            SSHShutdownIsOpenToggleSwitch.IsOn = wolModel.SSHIsOpen == "True";
            SSHCommandTextBox.Text = wolModel.SSHCommand;
            SSHPortTextBox.Text = wolModel.SSHPort;
            SSHUserTextBox.Text = wolModel.SSHUser;
            PrivateKeyIsOpenToggleSwitch.IsOn = wolModel.SSHKeyIsOpen == "True";
            SSHPasswordBox.Password = wolModel.SSHPasswd;
            SSHKeyPathTextBox.Text = wolModel.SSHKeyPath;

            refresh();
        }
        private void MyDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // 在"确定"按钮点击事件中保存用户输入的内容
            WoLData.Name = ConfigNameTextBox.Text;
            WoLData.IPAddress = IpAddressTextBox.Text;
            WoLData.BroadcastIsOpen = BroadcastCheckBox.IsChecked == true ? "True" : "False";
            WoLData.WoLIsOpen = WoLIsOpenToggleSwitch.IsOn ? "True" : "False";
            WoLData.WoLAddress = IpAddressTextBox.Text;
            WoLData.MacAddress = MacAddressTextBox.Text;
            WoLData.WoLPort = WoLPortTextBox.Text;
            WoLData.RDPIsOpen = RDPIsOpenToggleSwitch.IsOn ? "True" : "False";
            WoLData.RDPPort = RDPIPPortTextBox.Text;
            WoLData.SSHIsOpen = SSHShutdownIsOpenToggleSwitch.IsOn ? "True" : "False";
            WoLData.SSHCommand = SSHCommandTextBox.Text;
            WoLData.SSHPort = SSHPortTextBox.Text;
            WoLData.SSHUser = SSHUserTextBox.Text;
            WoLData.SSHPasswd = SSHPasswordBox.Password;
            WoLData.SSHKeyPath = SSHKeyPathTextBox.Text;
            WoLData.SSHKeyIsOpen = PrivateKeyIsOpenToggleSwitch.IsOn ? "True" : "False";
        }

        private void MyDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // 在"取消"按钮点击事件中不做任何操作
        }
        private void refresh()
        {
            // 是否启用功能
            WoLIsOpen();
            RDPIsOpen();
            ShutdownIsOpen();
            PrivateKeyIsOpen();
        }
        private void WoLIsOpen()
        {
            if (WoLIsOpenToggleSwitch.IsOn == true)
            {
                WoLConfig.Visibility = Visibility.Visible;
            }
            else
            {
                WoLConfig.Visibility = Visibility.Collapsed;
            }
        }
        private void RDPIsOpen()
        {
            if (RDPIsOpenToggleSwitch.IsOn == true)
            {
                RDPConfig.Visibility = Visibility.Visible;
            }
            else
            {
                RDPConfig.Visibility = Visibility.Collapsed;
            }
        }
        private void ShutdownIsOpen()
        {
            if (SSHShutdownIsOpenToggleSwitch.IsOn == true)
            {
                shutdownConfig.Visibility = Visibility.Visible;
            }
            else
            {
                shutdownConfig.Visibility = Visibility.Collapsed;
            }
        }
        private void PrivateKeyIsOpen()
        {
            if (PrivateKeyIsOpenToggleSwitch.IsOn == true)
            {
                SSHKey.Visibility = Visibility.Visible;
                SSHPasswordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                SSHKey.Visibility = Visibility.Collapsed;
                SSHPasswordBox.Visibility = Visibility.Visible;
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            refresh();
        }
        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            refresh();
        }
        private void Broadcast_Checked(object sender, RoutedEventArgs e)
        {
            WoLData.WoLAddress = "255.255.255.255";
            refresh();
        }
        private void Broadcast_Unchecked(object sender, RoutedEventArgs e)
        {
            WoLData.WoLAddress = IpAddressTextBox.Text;
            refresh();
        }
        private void rdpIsOpen_Toggled(object sender, RoutedEventArgs e)
        {
            refresh();
        }
        private void wolIsOpen_Toggled(object sender, RoutedEventArgs e)
        {
            refresh();
        }
        private void SSHShutdownIsOpen_Toggled(object sender, RoutedEventArgs e)
        {
            refresh();
        }
        private void privateKeyIsOpen_Toggled(object sender, RoutedEventArgs e)
        {
            refresh();
        }
        private async void SelectSSHKeyPath_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
