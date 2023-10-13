using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text.RegularExpressions;
using Windows.Storage.Pickers;
using WinWoL.Models;

namespace WinWoL.Pages.Dialogs
{
    public sealed partial class AddSSH : ContentDialog
    {
        public SSHModel SSHData { get; private set; }
        public AddSSH(SSHModel sshModel)
        {
            this.InitializeComponent();
            PrimaryButtonClick += MyDialog_PrimaryButtonClick;
            SecondaryButtonClick += MyDialog_SecondaryButtonClick;

            // 初始化Dialog中的字段，使用传入的WoLModel对象的属性
            SSHData = sshModel;
            ConfigNameTextBox.Text = sshModel.Name;
            IpAddressTextBox.Text = sshModel.IPAddress;
            SSHCommandTextBox.Text = sshModel.SSHCommand;
            SSHPortTextBox.Text = sshModel.SSHPort;
            SSHUserTextBox.Text = sshModel.SSHUser;
            PrivateKeyIsOpenToggleSwitch.IsOn = sshModel.SSHKeyIsOpen == "True";
            SSHKeyPathTextBox.Text = sshModel.SSHKeyPath;

            refresh();
        }
        private void MyDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // 在"确定"按钮点击事件中保存用户输入的内容
            SSHData.Name = string.IsNullOrEmpty(ConfigNameTextBox.Text) ? "<未命名配置>" : ConfigNameTextBox.Text;
            SSHData.IPAddress = IpAddressTextBox.Text;
            SSHData.SSHCommand = SSHCommandTextBox.Text;
            SSHData.SSHPort = SSHPortTextBox.Text;
            SSHData.SSHUser = SSHUserTextBox.Text;
            SSHData.SSHKeyPath = SSHKeyPathTextBox.Text;
            SSHData.SSHKeyIsOpen = PrivateKeyIsOpenToggleSwitch.IsOn ? "True" : "False";
        }

        private void MyDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // 在"取消"按钮点击事件中不做任何操作
        }
        private void refresh()
        {
            // 是否启用功能
            PrivateKeyIsOpen();
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
        private void privateKeyIsOpen_Toggled(object sender, RoutedEventArgs e)
        {
            refresh();
        }
        private async void SelectSSHKeyPath_Click(object sender, RoutedEventArgs e)
        {
            // 创建一个FileOpenPicker
            var openPicker = new FileOpenPicker();
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
            string filePath = null;
            if (file != null)
            {
                filePath = file.Path;
            }
            else
            {
                filePath = null;
            }
            SSHKeyPathTextBox.Text = filePath;
        }
        private void IPAddressTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            IPAddressTextClean(textBox);
        }
        private void IPAddressTextPaste(object sender, TextControlPasteEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            IPAddressTextClean(textBox);
        }
        private void IPAddressTextClean(TextBox textBox)
        {
            string input = textBox.Text;

            // 使用正则表达式来匹配合法的格式
            string pattern = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$|^((?!-)[A-Za-z0-9-]{1,63}(?<!-)\.)+[A-Za-z]{2,6}$";
            if (Regex.IsMatch(input, pattern))
            {
                // 输入合法，保持文本不变
                textBox.Text = input;
            }
            else
            {
                // 输入非法，移除不匹配的字符
                textBox.Text = Regex.Replace(input, @"[^A-Za-z0-9:.]", "");
                // 光标移动至末尾
                textBox.SelectionStart = textBox.Text.Length;
            }
        }
        private void PortTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            PortTextClean(textBox);
        }
        private void PortTextPaste(object sender, TextControlPasteEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            PortTextClean(textBox);
        }
        private void PortTextClean(TextBox textBox)
        {
            string input = textBox.Text;

            // 使用正则表达式来匹配合法的格式
            string pattern = "^[0-9]*$";
            if (Regex.IsMatch(input, pattern))
            {
                // 输入合法，保持文本不变
                textBox.Text = input;
            }
            else
            {
                // 输入非法，移除不匹配的字符
                textBox.Text = Regex.Replace(input, "[^0-9]", "");
                // 光标移动至末尾
                textBox.SelectionStart = textBox.Text.Length;
            }
        }
    }
}
