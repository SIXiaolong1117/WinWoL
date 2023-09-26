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
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Globalization;
using Windows.Storage;
using PInvoke;
using Windows.Services.Maps;
using Windows.Networking;
using System.Net.Mail;
using Validation;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Microsoft.UI.Dispatching;
using Windows.Storage.Streams;
using Renci.SshNet;

namespace WinWoL
{
    public sealed partial class WoL : Page
    {
        // 引入localSettings
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private DispatcherQueue _dispatcherQueue;

        // Selection需要的List
        public List<string> ConfigSelector { get; set; } = new List<string>()
        {
            "0","1","2","3","4","5","6","7","8","9","10"
        };

        // 页面初始化
        public WoL()
        {
            this.InitializeComponent();

            // 获取UI线程的DispatcherQueue
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            if (localSettings.Values["configNum"] == null)
            {
                configNum.SelectedItem = ConfigSelector[0];
                localSettings.Values["configNum"] = ConfigSelector[0];
                refresh("0");
            }
            else
            {
                configNum.SelectedItem = localSettings.Values["configNum"];
                refresh(localSettings.Values["configNum"].ToString());
            }
        }

        // 功能实现
        // 主刷新函数
        private void refresh(string ConfigIDNum)
        {
            // 读取localSettings中存储的字符串
            string configInner = localSettings.Values["ConfigID" + ConfigIDNum] as string;

            // 初始化界面元素
            ConfigName.Text = "配置别名：";
            MacAddress.Text = "主机 Mac：";
            IpAddress.Text = "主机地址：";
            IpPort.Text = "WoL 端口：";
            RDPIpPort.Text = "RDP 端口：";
            RDPPing.Text = "RDP 端口延迟：";

            // 隐藏覆盖 显示导入
            ImportConfig.Visibility = Visibility.Visible;
            ImportAndReplaceConfig.Visibility = Visibility.Collapsed;

            // 初始化所有按钮状态
            AddConfig.Content = "添加配置";
            DelConfig.IsEnabled = false;
            RefConfig.IsEnabled = false;
            WoLConfig.IsEnabled = false;
            RDPConfig.IsEnabled = false;
            ExportConfig.IsEnabled = false;
            HideConfig.IsEnabled = false;
            ShutdownConfig.IsEnabled = false;

            // 如果字符串不为空
            if (configInner != null)
            {
                // 修改界面UI可用性和文字显示
                AddConfig.Content = "修改配置";
                DelConfig.IsEnabled = true;
                WoLConfig.IsEnabled = true;
                ExportConfig.IsEnabled = true;
                HideConfig.IsEnabled = true;

                // 隐藏导入 显示覆盖
                ImportConfig.Visibility = Visibility.Collapsed;
                ImportAndReplaceConfig.Visibility = Visibility.Visible;

                // 分割字符串
                string[] configInnerSplit = new string[17];
                string[] configInnerSplitOld = configInner.Split(',');
                for (int i = 0; i < 17; i++)
                {
                    try
                    {
                        configInnerSplit[i] = configInnerSplitOld[i];
                    }
                    catch
                    {
                        if (i == 4 || i == 7 || i == 8 || i == 12 || i == 15)
                        {
                            configInnerSplit[i] = "False";
                        }
                        else
                        {
                            configInnerSplit[i] = "";
                        }
                    }
                }
                // 传入的字符串结构：
                //configName.Text = configInnerSplit[0];
                //macAddress.Text = configInnerSplit[1];
                //ipAddress.Text = configInnerSplit[2];
                //ipPort.Text = configInnerSplit[3];
                //rdpIsOpen.IsOn = configInnerSplit[4] == "True";
                //rdpIpAddress.Text = configInnerSplit[5];
                //rdpIpPort.Text = configInnerSplit[6];
                //Broadcast.IsChecked = configInnerSplit[7] == "True";
                //SSHCommand.Text = configInnerSplit[9];
                //SSHPort.Text = configInnerSplit[10];
                //SSHUser.Text = configInnerSplit[11];
                //PrivateKeyIsOpen.IsOn = configInnerSplit[12] == "True";
                //SSHPasswd.Password = configInnerSplit[13];
                //SSHKeyPath.Text = configInnerSplit[14];
                //shutdownIsOpen.IsOn = configInnerSplit[15] == "True";
                //SSHHost.Text = configInnerSplit[16];
                string configName = configInnerSplit[0];
                string macAddress = configInnerSplit[1];
                string ipAddress = configInnerSplit[5];
                string ipPort = configInnerSplit[3];
                string rdpIsOpen = configInnerSplit[4];
                string rdpIpAddress = configInnerSplit[5];
                string rdpPort = configInnerSplit[6];
                string shutdownIsOpen = configInnerSplit[15];
                string sshPort = configInnerSplit[10];

                // 更新RDP的命令行
                localSettings.Values["mstscCMD"] = "mstsc /v:" + rdpIpAddress + ":" + rdpPort + ";";

                // 如果不是广播地址，则显示IP或域名。
                // 如果是广播地址，则显示“向 LAN 网络广播”
                string ipAddressDisplay = ipAddress;
                if (ipAddressDisplay == "255.255.255.255")
                {
                    ipAddressDisplay = "向 LAN 网络广播";
                }

                // 如果开启隐藏地址
                string macAddressDisplay = macAddress;
                string ipPortDisplay = ipPort;
                string rdpPortDisplay = rdpPort;
                string sshPortDisplay = sshPort;
                if (localSettings.Values["HideConfig"] == null)
                {
                    localSettings.Values["HideConfig"] = "True";
                }
                if (localSettings.Values["HideConfig"].ToString() != "False")
                {
                    HideConfig.Content = "显示地址";
                    macAddressDisplay = "**:**:**:**:**:**";
                    ipAddressDisplay = "***.***.***.***";
                    ipPortDisplay = "***";
                    rdpPortDisplay = "***";
                    sshPortDisplay = "***";
                }

                ConfigName.Text = "配置别名：" + configName;
                MacAddress.Text = "主机 Mac：" + macAddressDisplay;
                IpAddress.Text = "主机地址：" + ipAddressDisplay;
                IpPort.Text = "WoL 端口：" + ipPortDisplay;

                // 如果开启RDP
                if (rdpIsOpen == "True")
                {
                    RDPIpPort.Text = "RDP 端口：" + rdpPortDisplay;
                    RDPPing.Text = "RDP 端口延迟：未测试";

                    RefConfig.IsEnabled = true;
                    RDPConfig.IsEnabled = true;
                }
                // 没有开启RDP
                else
                {
                    RDPIpPort.Text = "RDP 端口：未设置";
                    RDPPing.Text = "RDP 端口延迟：未设置";

                    RefConfig.IsEnabled = false;
                    RDPConfig.IsEnabled = false;
                }

                // 如果开启“关闭电脑”
                if (shutdownIsOpen == "True")
                {
                    SSHIpPort.Text = "SSH 端口：" + sshPortDisplay;
                    ShutdownConfig.IsEnabled = true;
                }
                // 没有开启“关闭电脑”
                else
                {
                    SSHIpPort.Text = "SSH 端口：未设置";
                    ShutdownConfig.IsEnabled = false;
                }
            }
        }

        // 根据配置文件，调用发送MagicPacket
        private void WoLPC(string ConfigIDNum)
        {
            // 读取localSettings中的字符串
            string configInner = localSettings.Values["ConfigID" + ConfigIDNum] as string;
            // 如果字符串非空
            if (configInner != null)
            {
                // 分割字符串
                string[] configInnerSplit = configInner.Split(',');
                // configName.Text + "," + macAddress.Text + "," + ipAddress.Text + "," + ipPort.Text;
                string macAddress = configInnerSplit[1];
                string ipAddress = configInnerSplit[2];
                string ipPort = configInnerSplit[3];

                // 尝试发送Magic Packet，成功打开已发送弹窗
                try
                {
                    // 获取IP地址
                    IPAddress ip = CommonFunctions.domain2ip(ipAddress);
                    // 暂时停用相关按钮
                    WoLConfig.IsEnabled = false;
                    // 在子线程中执行任务
                    Thread subThread = new Thread(new ThreadStart(() =>
                    {
                        CommonFunctions.sendMagicPacket(macAddress, ip, int.Parse(ipPort));

                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            MagicPacketIsSendTips.IsOpen = true;
                            // 开启相关按钮
                            WoLConfig.IsEnabled = true;
                        });
                    }));
                    subThread.Start();

                }
                // 失败打开发送失败弹窗
                catch
                {
                    MagicPacketNotSendTips.IsOpen = true;
                }
            }
        }

        // Ping RDP主机端口
        private void PingRDPRef(string ConfigIDNum)
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
                    string configInner = localSettings.Values["ConfigID" + ConfigIDNum] as string;
                    // 如果字符串非空
                    if (configInner != null)
                    {
                        // 分割字符串
                        string[] configInnerSplit = configInner.Split(',');
                        // 传入的字符串结构：
                        // configName.Text + "," + macAddress.Text + ","
                        // + ipAddress.Text + "," + ipPort.Text + ","
                        // + rdpIsOpen.IsOn + "," + rdpIpAddress.Text + "," + rdpIpPort;
                        string rdpIpAddress = configInnerSplit[5];
                        string rdpPort = configInnerSplit[6];
                        // 检查RDP主机端口是否可以Ping通
                        try
                        {
                            pingRes = "RDP 端口延迟：" + CommonFunctions.PingTest(rdpIpAddress, int.Parse(rdpPort)).ToString();
                        }
                        catch
                        {
                            pingRes = "RDP 端口延迟：无法联通";
                        }
                    }
                    // 要在UI线程上更新UI，使用DispatcherQueue
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        int flag = 0;
                        if (pingRes != "RDP 端口延迟：无法联通")
                        {
                            RDPPing.Text = pingRes;
                            flag++;
                        }
                        if (flag == 3)
                        {
                            RDPPing.Text = "RDP 端口延迟：无法联通";
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
        // 唤起mstsc函数
        private void RDPPCChildThread()
        {
            // 暂时停用相关按钮
            RDPConfig.IsEnabled = false;
            // 在子线程中执行任务
            Thread subThread = new Thread(new ThreadStart(() =>
            {
                string arguments = localSettings.Values["mstscCMD"] as string;

                CommonFunctions.RDPConnect(arguments);

                // 要在UI线程上更新UI，使用DispatcherQueue
                _dispatcherQueue.TryEnqueue(() =>
                {
                    RDPTips.IsOpen = true;
                    // 开启相关按钮
                    RDPConfig.IsEnabled = true;
                });
            }));
            subThread.Start();
        }

        // 事件
        // Selection改变
        private void configNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            refresh(configNum.SelectedItem.ToString());
            localSettings.Values["configNum"] = configNum.SelectedItem;

            //关闭所有弹出窗口
            MagicPacketIsSendTips.IsOpen = false;
            MagicPacketNotSendTips.IsOpen = false;
            RDPTips.IsOpen = false;
            SaveConfigTips.IsOpen = false;
        }
        // 添加/修改配置按钮点击
        private async void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            // 将ConfigIDTemp所存储的字符串设置为当前配置所存储的字符串
            // 这样可以实现“修改”的操作
            localSettings.Values["ConfigIDTemp"] = localSettings.Values["ConfigID" + ConfigIDNum];

            // 创建一个新的dialog对象
            AddConfigDialog configDialog = new AddConfigDialog();

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
                localSettings.Values["ConfigID" + ConfigIDNum] = localSettings.Values["ConfigIDTemp"];
                // 刷新UI
                refresh(ConfigIDNum);
            }
        }
        // Ping测试按钮点击
        private void RefConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            // Ping测试
            PingRDPRef(ConfigIDNum);
        }
        // 删除配置按钮点击
        private void DelConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            // 删除配置
            // 清空指定ConfigIDNum的localSettings
            localSettings.Values["ConfigID" + ConfigIDNum] = null;
            // 刷新UI
            refresh(ConfigIDNum);
            // 隐藏提示Flyout
            if (this.DelConfig.Flyout is Flyout f)
            {
                f.Hide();
            }
        }
        // 网络唤醒按钮点击
        private void WoLConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            // WoL配置指定的设备
            WoLPC(ConfigIDNum);
        }
        // 远程桌面按钮点击
        private void RDPConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            // 调用mstsc
            RDPPCChildThread();
        }
        // 导入配置按钮点击
        private async void ImportConfig_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();

            await CommonFunctions.ImportConfig("WinWoL.WoL", "ConfigID", ConfigIDNum);

            // 刷新UI
            refresh(ConfigIDNum);
        }
        // 导出配置按钮点击
        private async void ExportConfig_Click(object sender, RoutedEventArgs e)
        {
            // 从localSettings中读取字符串
            string ConfigIDNum = configNum.SelectedItem.ToString();
            string configInner = localSettings.Values["ConfigID" + ConfigIDNum] as string;
            string[] configInnerSplit = configInner.Split(',');
            string configName = configInnerSplit[0];
            // 如果字符串非空
            if (configInner != null)
            {
                string result = await CommonFunctions.ExportConfig("WinWoL.WoL", "ConfigID", ConfigIDNum, configName);
                SaveConfigTips.Title = result;
                SaveConfigTips.IsOpen = true;
            }
        }
        private void HideConfig_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            if (localSettings.Values["HideConfig"].ToString() == "True")
            {
                localSettings.Values["HideConfig"] = "False";
                HideConfig.Content = "隐藏地址";
            }
            else
            {
                localSettings.Values["HideConfig"] = "True";
                HideConfig.Content = "显示地址";
            }
            // 刷新UI
            refresh(ConfigIDNum);
        }
        private void runSSH(string ConfigIDNum)
        {
            // 读取localSettings中的字符串
            string configInner = localSettings.Values["ConfigID" + ConfigIDNum] as string;

            if (configInner != null)
            {
                // 分割字符串
                string[] configInnerSplit = configInner.Split(',');
                // 传入的字符串结构：
                //configName.Text = configInnerSplit[0];
                //macAddress.Text = configInnerSplit[1];
                //ipAddress.Text = configInnerSplit[2];
                //ipPort.Text = configInnerSplit[3];
                //rdpIsOpen.IsOn = configInnerSplit[4] == "True";
                //rdpIpAddress.Text = configInnerSplit[5];
                //rdpIpPort.Text = configInnerSplit[6];
                //Broadcast.IsChecked = configInnerSplit[7] == "True";
                //SSHCommand.Text = configInnerSplit[9];
                //SSHPort.Text = configInnerSplit[10];
                //SSHUser.Text = configInnerSplit[11];
                //PrivateKeyIsOpen.IsOn = configInnerSplit[12] == "True";
                //SSHPasswd.Password = configInnerSplit[13];
                //SSHKeyPath.Text = configInnerSplit[14];
                //shutdownIsOpen.IsOn = configInnerSplit[15] == "True";
                //SSHHost.Text = configInnerSplit[16];
                string sshCommand = configInnerSplit[9];
                string sshHost = configInnerSplit[16];
                string sshPort = configInnerSplit[10];
                string sshUser = configInnerSplit[11];
                string sshPasswdAndKey;
                string privateKeyIsOpen = configInnerSplit[12];

                if (privateKeyIsOpen == "True")
                {
                    sshPasswdAndKey = configInnerSplit[14];
                }
                else
                {
                    sshPasswdAndKey = configInnerSplit[13];
                }

                // 获取IP地址
                sshHost = CommonFunctions.domain2ip(sshHost).ToString();

                try
                {
                    SSHResponse.Subtitle = CommonFunctions.SendSSHCommand(sshCommand, sshHost, sshPort, sshUser, sshPasswdAndKey, privateKeyIsOpen);
                    SSHResponse.IsOpen = true;
                }
                catch
                {
                    SSHCommandNotSendTips.Subtitle = "请检查您填写的配置以及网络状况。\n注意私钥路径和格式（仅支持 OpenSSH 和 ssh.com 格式的 RSA 和 DSA 私钥）";
                    SSHCommandNotSendTips.IsOpen = true;
                }
            }
        }
        private void ShutdownConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string ConfigIDNum = configNum.SelectedItem.ToString();
            runSSH(ConfigIDNum);
        }
    }
}
