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
using System.Threading;
using Validation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Renci.SshNet;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.Mail;
using Microsoft.UI.Dispatching;

namespace WinWoL
{
    public sealed partial class SSHWoL : Page
    {
        // 引入localSettings
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private DispatcherQueue _dispatcherQueue;

        // Selection需要的List
        public List<string> ConfigSelector { get; set; } = new List<string>()
        {
            "0","1","2","3","4","5","6","7","8","9","10"
        };
        public SSHWoL()
        {
            this.InitializeComponent();

            // 获取UI线程的DispatcherQueue
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            if (localSettings.Values["SSHConfigNum"] == null)
            {
                configNum.SelectedItem = ConfigSelector[0];
                localSettings.Values["SSHConfigNum"] = ConfigSelector[0];
                refresh("0");
            }
            else
            {
                configNum.SelectedItem = localSettings.Values["SSHConfigNum"];
                refresh(localSettings.Values["SSHConfigNum"].ToString());
            }
        }
        // 功能实现
        // 主刷新函数
        private void refresh(string SSHConfigIDNum)
        {
            // 读取localSettings中存储的字符串
            string configInner = localSettings.Values["SSHConfigID" + SSHConfigIDNum] as string;

            // 初始化界面元素
            SSHConfigName.Text = "配置别名：";
            SSHCommand.Text = "SSH 命令：";
            SSHHost.Text = "SSH 主机：";
            SSHPort.Text = "SSH 端口：";
            SSHUser.Text = "SSH 用户：";
            SSHPing.Text = "SSH 端口延迟：未测试";

            // 隐藏覆盖 显示导入
            ImportConfig.Visibility = Visibility.Visible;
            ImportAndReplaceConfig.Visibility = Visibility.Collapsed;

            AddConfig.Content = "添加配置";
            DelConfig.IsEnabled = false;
            RefConfig.IsEnabled = false;
            ExportConfig.IsEnabled = false;

            // 如果字符串不为空
            if (configInner != null)
            {
                // 修改界面UI可用性和文字显示
                AddConfig.Content = "修改配置";
                DelConfig.IsEnabled = true;
                //WoLConfig.IsEnabled = true;
                ExportConfig.IsEnabled = true;

                // 隐藏导入 显示覆盖
                ImportConfig.Visibility = Visibility.Collapsed;
                ImportAndReplaceConfig.Visibility = Visibility.Visible;

                // 分割字符串
                string[] configInnerSplit = configInner.Split(',');
                // 传入的字符串结构：
                //  SSHConfigName.Text + "," + SSHCommand.Text + ","
                //+ SSHHost.Text + "," + SSHPort.Text + ","
                //+ SSHUser.Text + "," + SSHPasswd.Text + ","
                //+ PrivateKeyIsOpen.IsOn + "," + SSHKey.Text;
                string sshConfigName = configInnerSplit[0];
                string sshCommand = configInnerSplit[1];
                string sshHost = configInnerSplit[2];
                string sshPort = configInnerSplit[3];
                string sshUser = configInnerSplit[4];

                SSHConfigName.Text = "配置别名：" + sshConfigName;
                SSHCommand.Text = "SSH 命令：" + sshCommand;
                SSHHost.Text = "SSH 主机：" + sshHost;
                SSHPort.Text = "SSH 端口：" + sshPort;
                SSHUser.Text = "SSH 用户：" + sshUser;
                SSHPing.Text = "SSH 端口延迟：未测试";

                RefConfig.IsEnabled = true;
            }
        }
        // SSH执行命令
        private void SendSSHCommand(string sshCommand, string sshHost, string sshPort, string sshUser, string sshPasswdAndKey, string privateKeyIsOpen)
        {
            SshClient sshClient;

            if (privateKeyIsOpen == "True")
            {
                PrivateKeyFile privateKeyFile = new PrivateKeyFile(sshPasswdAndKey);
                ConnectionInfo connectionInfo = new ConnectionInfo(sshHost, sshUser, new PrivateKeyAuthenticationMethod(sshUser, new PrivateKeyFile[] { privateKeyFile }));
                sshClient = new SshClient(connectionInfo);
            }
            else
            {
                sshClient = new SshClient(sshHost, int.Parse(sshPort), sshUser, sshPasswdAndKey);
            }

            sshClient.Connect();

            if (sshClient.IsConnected)
            {
                SshCommand SSHCommand = sshClient.RunCommand(sshCommand);

                if (!string.IsNullOrEmpty(SSHCommand.Error))
                {
                    SSHResponse.Subtitle = "Error: " + SSHCommand.Error;
                    SSHResponse.IsOpen = true;
                }
                else
                {
                    SSHResponse.Subtitle = SSHCommand.Result;
                    SSHResponse.IsOpen = true;
                }
            }
            sshClient.Disconnect();
        }
        private void runSSH(string SSHConfigIDNum)
        {
            // 读取localSettings中的字符串
            string configInner = localSettings.Values["SSHConfigID" + SSHConfigIDNum] as string;

            if (configInner != null)
            {
                // 分割字符串
                string[] configInnerSplit = configInner.Split(',');
                // 传入的字符串结构：
                //  SSHConfigName.Text + "," + SSHCommand.Text + ","
                //+ SSHHost.Text + "," + SSHPort.Text + ","
                //+ SSHUser.Text + "," + SSHPasswd.Text + ","
                //+ PrivateKeyIsOpen.IsOn + "," + SSHKey.Text;
                string sshCommand = configInnerSplit[1];
                string sshHost = configInnerSplit[2];
                string sshPort = configInnerSplit[3];
                string sshUser = configInnerSplit[4];
                string sshPasswdAndKey;
                string privateKeyIsOpen = configInnerSplit[6];

                if (privateKeyIsOpen == "True")
                {
                    sshPasswdAndKey = configInnerSplit[7];
                }
                else
                {
                    sshPasswdAndKey = configInnerSplit[5];
                }

                // 获取IP地址
                sshHost = domain2ip(sshHost).ToString();

                try
                {
                    SendSSHCommand(sshCommand, sshHost, sshPort, sshUser, sshPasswdAndKey, privateKeyIsOpen);
                }
                catch
                {
                    SSHCommandNotSendTips.Subtitle = "请检查您填写的配置以及网络状况。\n注意私钥路径和格式（仅支持 OpenSSH 和 ssh.com 格式的 RSA 和 DSA 私钥）";
                    SSHCommandNotSendTips.IsOpen = true;
                }
            }
        }
        static IPAddress domain2ip(string domain)
        {
            // 此函数本身可以处理部分非法IP（例如：266.266.266.266）
            // 这些非法IP会被算作域名来处理
            IPAddress ipAddress;
            if (IPAddress.TryParse(domain, out ipAddress))
            {
                // 是IP
                return IPAddress.Parse(domain);
            }
            else
            {
                // 是域名或其他输入
                return Dns.GetHostEntry(domain).AddressList[0];
            }
        }
        // Ping测试函数
        static string PingTest(string domain, int port)
        {
            // 获取IP地址
            // 在这里执行这个操作，可以处理一些非法IP的输入问题（例如：广播地址 255.255.255.255）
            // 非法IP会被返回“主机地址无法联通”，而不会让pingSender报错导致应用崩溃
            IPAddress ipAddress = domain2ip(domain);

            // Ping实例对象
            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
            // Ping选项
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "ping";
            byte[] buf = Encoding.ASCII.GetBytes(data);

            // 调用同步Send方法发送数据，结果存入reply对象;
            PingReply reply = pingSender.Send(ipAddress, 500, buf, options);
            // 判断replay，是否连通
            if (reply.Status == IPStatus.Success)
            {
                // 如果连通，尝试与指定端口通信
                var client = new TcpClient();
                if (!client.ConnectAsync(ipAddress, port).Wait(500))
                {
                    // 与指定端口通信失败
                    return "端口连接失败";
                }
                else
                {
                    // 与指定端口通信成功，计算RTT并返回
                    return reply.RoundtripTime.ToString() + " ms";
                }
            }
            else
            {
                // 无法联通
                return "无法联通";
            }

        }
        // Ping SSH主机端口
        private void PingSSHRef(string SSHConfigIDNum)
        {
            // 暂时停用相关按钮
            RefConfig.IsEnabled = false;
            configNum.IsEnabled = false;
            // 在子线程中执行任务
            Thread subThread = new Thread(new ThreadStart(() =>
            {
                string pingRes = "";
                for (int i = 5; i > 0; i--)
                {
                    // 从localSettings中读取字符串
                    string configInner = localSettings.Values["SSHConfigID" + SSHConfigIDNum] as string;
                    // 如果字符串非空
                    if (configInner != null)
                    {
                        // 分割字符串
                        string[] configInnerSplit = configInner.Split(',');
                        // 传入的字符串结构：
                        // SSHConfigName.Text + "," + SSHCommand.Text + ","
                        // + SSHHost.Text + "," + SSHPort.Text + ","
                        // + SSHUser.Text + "," + SSHPasswd.Text;
                        string sshHost = configInnerSplit[2];
                        string sshPort = configInnerSplit[3];

                        // 检查RDP主机端口是否可以Ping通
                        try
                        {
                            pingRes = "SSH 端口延迟：" + PingTest(sshHost, int.Parse(sshPort)).ToString();
                        }
                        catch
                        {
                            pingRes = "SSH 端口延迟：无法联通";
                        }
                    }
                    // 要在UI线程上更新UI，使用DispatcherQueue
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        int flag = 0;
                        if (pingRes != "SSH 端口延迟：无法联通")
                        {
                            SSHPing.Text = pingRes;
                            flag++;
                        }
                        if (flag == 3)
                        {
                            SSHPing.Text = "SSH 端口延迟：无法联通";
                            flag = 0;
                        }
                        RefConfig.Content = "Ping (" + i + ")";
                    });
                    Thread.Sleep(1000);
                }
                // 要在UI线程上更新UI，使用DispatcherQueue
                _dispatcherQueue.TryEnqueue(() =>
                {
                    RefConfig.Content = "Ping";
                    RefConfig.IsEnabled = true;
                    configNum.IsEnabled = true;
                });
            }));

            subThread.Start();
        }

        // 事件
        // Selection改变
        private void configNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            refresh(configNum.SelectedItem.ToString());
            localSettings.Values["SSHConfigNum"] = configNum.SelectedItem;
        }
        // 添加/修改配置按钮点击
        private async void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string SSHConfigIDNum = configNum.SelectedItem.ToString();
            // 将ConfigIDTemp所存储的字符串设置为当前配置所存储的字符串
            // 这样可以实现“修改”的操作
            localSettings.Values["SSHConfigIDTemp"] = localSettings.Values["SSHConfigID" + SSHConfigIDNum];

            // 创建一个新的dialog对象
            AddSSHConfigDialog configDialog = new AddSSHConfigDialog();

            // 对此dialog对象进行配置
            configDialog.XamlRoot = this.XamlRoot;
            configDialog.Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            // 根据内容有无来决定PrimaryButton的内容
            if (AddConfig.Content.ToString() == "修改配置")
            {
                configDialog.PrimaryButtonText = "修改";
            }
            else
            {
                configDialog.PrimaryButtonText = "添加";
            }
            configDialog.CloseButtonText = "关闭";
            // 默认按钮为PrimaryButton
            configDialog.DefaultButton = ContentDialogButton.Primary;

            // 异步获取按下哪个按钮
            var result = await configDialog.ShowAsync();

            // 如果按下了Primary
            if (result == ContentDialogResult.Primary)
            {
                // 将ConfigIDTemp写入到当前配置ID下的localSettings
                localSettings.Values["SSHConfigID" + SSHConfigIDNum] = localSettings.Values["SSHConfigIDTemp"];
                // 刷新UI
                refresh(SSHConfigIDNum);
            }
        }
        // Ping测试按钮点击
        private void RefConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string SSHConfigIDNum = configNum.SelectedItem.ToString();
            // Ping测试
            PingSSHRef(SSHConfigIDNum);
        }
        // 删除配置按钮点击
        private void DelConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string SSHConfigIDNum = configNum.SelectedItem.ToString();
            // 删除配置
            // 清空指定ConfigIDNum的localSettings
            localSettings.Values["SSHConfigID" + SSHConfigIDNum] = null;
            // 刷新UI
            refresh(SSHConfigIDNum);
            // 隐藏提示Flyout
            if (this.DelConfig.Flyout is Flyout f)
            {
                f.Hide();
            }
        }
        // 导入配置按钮点击
        private async void ImportConfig_Click(object sender, RoutedEventArgs e)
        {
            string SSHConfigIDNum = configNum.SelectedItem.ToString();

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
            openPicker.FileTypeFilter.Add(".sshcmdconfig");

            // 打开选择器供用户选择文件
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var path = file.Path;
                localSettings.Values["SSHConfigID" + SSHConfigIDNum] = File.ReadAllText(path, Encoding.UTF8);
                // 刷新UI
                refresh(SSHConfigIDNum);
            }
        }
        // 导出配置按钮点击
        private async void ExportConfig_Click(object sender, RoutedEventArgs e)
        {
            // 从localSettings中读取字符串
            string SSHConfigIDNum = configNum.SelectedItem.ToString();
            string configInner = localSettings.Values["SSHConfigID" + SSHConfigIDNum] as string;
            // 如果字符串非空
            if (configInner != null)
            {
                // 分割字符串
                string[] configInnerSplit = configInner.Split(',');
                // 传入的字符串结构：
                // SSHConfigName.Text + "," + SSHCommand.Text + ","
                // + SSHHost.Text + "," + SSHPort.Text + ","
                // + SSHUser.Text + "," + SSHPasswd.Text;
                string sshConfigName = configInnerSplit[0];

                string configContent = localSettings.Values["SSHConfigID" + SSHConfigIDNum].ToString();

                // 创建一个FileSavePicker
                FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
                // 获取当前窗口句柄 (HWND) 
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
                // 使用窗口句柄 (HWND) 初始化FileSavePicker
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

                // 为FilePicker设置选项
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // 用户可以将文件另存为的文件类型下拉列表
                savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".sshcmdconfig" });
                // 默认文件名
                savePicker.SuggestedFileName = sshConfigName + "_BackUp_" + DateTime.Now.ToString();

                // 打开Picker供用户选择文件
                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    // 阻止更新文件的远程版本，直到我们完成更改并调用 CompleteUpdatesAsync。
                    CachedFileManager.DeferUpdates(file);

                    // 写入文件
                    using (var stream = await file.OpenStreamForWriteAsync())
                    {
                        using (var tw = new StreamWriter(stream))
                        {
                            tw.WriteLine(configContent);
                        }
                    }

                    // 让Windows知道我们已完成文件更改，以便其他应用程序可以更新文件的远程版本。
                    // 完成更新可能需要Windows请求用户输入。
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == FileUpdateStatus.Complete)
                    {
                        // 文件保存成功
                        SaveConfigTips.Title = "保存成功！";
                        SaveConfigTips.IsOpen = true;
                    }
                    else if (status == FileUpdateStatus.CompleteAndRenamed)
                    {
                        // 文件重命名并保存成功
                        SaveConfigTips.Title = "重命名并保存成功！";
                        SaveConfigTips.IsOpen = true;
                    }
                    else
                    {
                        // 文件无法保存！
                        SaveConfigTips.Title = "无法保存！";
                        SaveConfigTips.IsOpen = true;
                    }
                }
            }
        }
        // 执行脚本按钮点击
        private void RunSSHButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            runSSH(ConfigIDNum);
        }
    }
}
