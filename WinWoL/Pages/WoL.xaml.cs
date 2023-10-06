using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using WinWoL.Datas;
using WinWoL.Models;
using System.Net;
using Newtonsoft.Json;
using WinWoL.Methods;
using System.Threading;
using Microsoft.UI.Dispatching;
using WinWoL.Pages.Dialogs;
using System.Security.Principal;
using Validation;
using Renci.SshNet;

namespace WinWoL.Pages
{
    public sealed partial class WoL : Page
    {
        WoLModel selectedWoLModel;
        private DispatcherQueue _dispatcherQueue;
        public WoL()
        {
            this.InitializeComponent();
            // 获取UI线程的DispatcherQueue
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            // 加载数据
            LoadData();

            PingRefConfig.IsEnabled = false;
        }

        private void LoadData()
        {
            // 实例化SQLiteHelper
            SQLiteHelper dbHelper = new SQLiteHelper();
            // 查询数据
            List<WoLModel> dataList = dbHelper.QueryData();

            // 将数据列表绑定到ListView
            dataListView.ItemsSource = dataList;
        }
        // 添加/修改配置按钮点击
        private async void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建一个初始的WoLModel对象
            WoLModel initialWoLData = new WoLModel();

            // 创建一个新的dialog对象
            AddWoL dialog = new AddWoL(initialWoLData);
            // 对此dialog对象进行配置
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.PrimaryButtonText = "添加";
            dialog.CloseButtonText = "关闭";
            // 默认按钮为PrimaryButton
            dialog.DefaultButton = ContentDialogButton.Primary;

            // 显示Dialog并等待其关闭
            ContentDialogResult result = await dialog.ShowAsync();

            // 如果按下了Primary
            if (result == ContentDialogResult.Primary)
            {
                // 实例化SQLiteHelper
                SQLiteHelper dbHelper = new SQLiteHelper();
                // 插入新数据
                dbHelper.InsertData(initialWoLData);
                // 重新加载数据
                LoadData();
            }
        }


        private void ChangeConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataListView.SelectedItem != null)
            {
                // 获取WoLModel对象
                WoLModel selectedModel = (WoLModel)dataListView.SelectedItem;
                EditThisConfig(selectedModel);
            }
            else
            {
                NeedSelectedTips.IsOpen = true;
            }
        }
        // 导入配置按钮点击
        private async void ImportConfig_Click(object sender, RoutedEventArgs e)
        {
            ImportConfig.IsEnabled = false;
            // 实例化SQLiteHelper
            SQLiteHelper dbHelper = new SQLiteHelper();
            // 获取导入的数据
            WoLModel woLModel = await WoLMethod.ImportConfig();
            if (woLModel != null)
            {
                // 插入新数据
                dbHelper.InsertData(woLModel);
                // 重新加载数据
                LoadData();
            }
            ImportConfig.IsEnabled = true;
        }
        private void WoLPC(WoLModel woLModel)
        {
            InProgressing.IsActive = true;
            // 在子线程中执行任务
            Thread subThread = new Thread(new ThreadStart(() =>
            {
                string SuccessFlag = "0";
                // 尝试发送Magic Packet，成功打开已发送弹窗
                try
                {
                    IPAddress wolAddress = WoLMethod.DomainToIp(woLModel.WoLAddress, "IPv4");
                    WoLMethod.sendMagicPacket(woLModel.MacAddress, wolAddress, int.Parse(woLModel.WoLPort));
                    SuccessFlag = "1";
                }
                // 失败打开发送失败弹窗
                catch
                {
                    SuccessFlag = "0";
                }
                if (SuccessFlag == "1")
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        WoLResultTips.IsOpen = true;
                        WoLResultTips.Title = "Magic Packet 发送成功！";
                        WoLResultTips.Subtitle = "Magic Packet 已经通过 UDP 成功发送";
                    });
                }
                else
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        WoLResultTips.IsOpen = true;
                        WoLResultTips.Title = "Magic Packet 发送失败！";
                        WoLResultTips.Subtitle = "请检查您填写的配置内容";
                    });
                }
                _dispatcherQueue.TryEnqueue(() =>
                {
                    InProgressing.IsActive = false;
                });
            }));
            subThread.Start();
        }
        private async void EditThisConfig(WoLModel woLModel)
        {
            // 创建一个新的dialog对象
            AddWoL dialog = new AddWoL(woLModel);
            // 对此dialog对象进行配置
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.PrimaryButtonText = "修改";
            dialog.CloseButtonText = "关闭";
            // 默认按钮为PrimaryButton
            dialog.DefaultButton = ContentDialogButton.Primary;

            // 显示Dialog并等待其关闭
            ContentDialogResult result = await dialog.ShowAsync();

            // 如果按下了Primary
            if (result == ContentDialogResult.Primary)
            {
                // 实例化SQLiteHelper
                SQLiteHelper dbHelper = new SQLiteHelper();
                // 更新数据
                dbHelper.UpdateData(woLModel);
                // 重新加载数据
                LoadData();
            }
        }
        private async void ExportConfigFunction()
        {
            // 获取WoLModel对象
            WoLModel selectedModel = (WoLModel)dataListView.SelectedItem;
            string result = await WoLMethod.ExportConfig(selectedModel);
        }
        private void CopyThisConfig(WoLModel wolModel)
        {
            SQLiteHelper dbHelper = new SQLiteHelper();
            dbHelper.InsertData(wolModel);
            // 重新加载数据
            LoadData();
        }
        private void DelThisConfig(WoLModel wolModel)
        {
            // 实例化SQLiteHelper
            SQLiteHelper dbHelper = new SQLiteHelper();
            // 删除数据
            dbHelper.DeleteData(wolModel);
            // 重新加载数据
            LoadData();
        }
        private void ConfirmDelete_Click(object sender, RoutedEventArgs e)
        {
            // 关闭二次确认Flyout
            confirmationFlyout.Hide();
            DelThisConfig(selectedWoLModel);
        }
        private async void ConfirmReplace_Click(object sender, RoutedEventArgs e)
        {
            // 关闭二次确认Flyout
            confirmationReplaceFlyout.Hide();
            // 获取NSModel对象
            WoLModel selectedModel = (WoLModel)dataListView.SelectedItem;
            ImportConfig.IsEnabled = false;
            // 实例化SQLiteHelper
            SQLiteHelper dbHelper = new SQLiteHelper();

            // 获取导入的数据
            WoLModel newModel = await WoLMethod.ImportConfig();

            if (newModel != null)
            {
                // 获取当前配置的ID
                int id = selectedModel.Id;
                // 赋给导入的配置
                newModel.Id = id;
                // 插入新数据
                dbHelper.UpdateData(newModel);
                // 重新加载数据
                LoadData();
            }
            ImportConfig.IsEnabled = true;
        }
        private void CancelReplace_Click(object sender, RoutedEventArgs e)
        {
            // 关闭二次确认Flyout
            confirmationReplaceFlyout.Hide();
        }
        private void CancelDelete_Click(object sender, RoutedEventArgs e)
        {
            // 关闭二次确认Flyout
            confirmationFlyout.Hide();
        }
        private void PingRefConfig_Click(object sender, RoutedEventArgs e)
        {
            WoLModel selectedItem = (WoLModel)dataListView.SelectedItem;
            PingRef.Text = WoLMethod.PingTest(selectedItem.IPAddress);
        }
        private void OnListViewRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // 获取右键点击的ListViewItem
            FrameworkElement listViewItem = (sender as FrameworkElement);

            // 获取右键点击的数据对象（WoLModel）
            WoLModel selectedItem = listViewItem?.DataContext as WoLModel;

            if (selectedItem != null && InProgressing.IsActive == false)
            {
                // 将右键点击的项设置为选中项
                dataListView.SelectedItem = selectedItem;

                // 创建ContextMenu
                MenuFlyout menuFlyout = new MenuFlyout();

                if (selectedItem.WoLIsOpen == "True")
                {
                    MenuFlyoutItem wolPCMenuItem = new MenuFlyoutItem
                    {
                        Text = "网络唤醒"
                    };
                    wolPCMenuItem.Click += (sender, e) =>
                    {
                        WoLPC(selectedItem);
                    };
                    menuFlyout.Items.Add(wolPCMenuItem);
                }

                if (selectedItem.RDPIsOpen == "True")
                {
                    MenuFlyoutItem rdpPCMenuItem = new MenuFlyoutItem
                    {
                        Text = "远程桌面"
                    };
                    rdpPCMenuItem.Click += (sender, e) =>
                    {
                        string mstscCMD = $"mstsc /v:{selectedItem.IPAddress}:{selectedItem.RDPPort};";
                        WoLMethod.RDPConnect(mstscCMD);
                    };
                    menuFlyout.Items.Add(rdpPCMenuItem);
                }

                if (selectedItem.SSHIsOpen == "True")
                {
                    MenuFlyoutItem sshShutdownPCMenuItem = new MenuFlyoutItem
                    {
                        Text = "远程关机"
                    };
                    sshShutdownPCMenuItem.Click += (sender, e) =>
                    {
                        WoLMethod.SendSSHCommand(selectedItem.SSHCommand, selectedItem.IPAddress, selectedItem.SSHPort, selectedItem.SSHUser, selectedItem.SSHPasswd, selectedItem.SSHKeyPath, selectedItem.SSHKeyIsOpen);
                    };
                    menuFlyout.Items.Add(sshShutdownPCMenuItem);
                }

                // 添加分割线
                MenuFlyoutSeparator separator = new MenuFlyoutSeparator();
                menuFlyout.Items.Add(separator);

                MenuFlyoutItem exportMenuItem = new MenuFlyoutItem
                {
                    Text = "导出配置"
                };
                exportMenuItem.Click += (sender, e) =>
                {
                    ExportConfigFunction();
                };
                menuFlyout.Items.Add(exportMenuItem);

                MenuFlyoutItem replaceMenuItem = new MenuFlyoutItem
                {
                    Text = "覆盖配置"
                };
                replaceMenuItem.Click += (sender, e) =>
                {
                    // 弹出二次确认Flyout
                    confirmationReplaceFlyout.ShowAt(listViewItem);
                };
                menuFlyout.Items.Add(replaceMenuItem);

                // 添加分割线
                MenuFlyoutSeparator separator2 = new MenuFlyoutSeparator();
                menuFlyout.Items.Add(separator2);

                MenuFlyoutItem editMenuItem = new MenuFlyoutItem
                {
                    Text = "编辑配置"
                };
                editMenuItem.Click += (sender, e) =>
                {
                    EditThisConfig(selectedItem);
                };
                menuFlyout.Items.Add(editMenuItem);

                MenuFlyoutItem copyMenuItem = new MenuFlyoutItem
                {
                    Text = "复制配置"
                };
                copyMenuItem.Click += (sender, e) =>
                {
                    CopyThisConfig(selectedItem);
                };
                menuFlyout.Items.Add(copyMenuItem);

                MenuFlyoutItem deleteMenuItem = new MenuFlyoutItem
                {
                    Text = "删除配置"
                };
                deleteMenuItem.Click += (sender, e) =>
                {
                    // 保存选中的数据对象以便确认后执行
                    selectedWoLModel = selectedItem;
                    // 弹出二次确认Flyout
                    confirmationFlyout.ShowAt(listViewItem);
                };
                menuFlyout.Items.Add(deleteMenuItem);

                Thread.Sleep(10);

                // 在指定位置显示ContextMenu
                menuFlyout.ShowAt(listViewItem, e.GetPosition(listViewItem));
            }

        }
        private void OnListViewDoubleTapped(object sender, RoutedEventArgs e)
        {
            // 处理左键双击事件的代码
            // 获取右键点击的ListViewItem
            FrameworkElement listViewItem = (sender as FrameworkElement);
            if (listViewItem != null && InProgressing.IsActive == false)
            {
                // 获取点击的数据对象
                WoLModel selectedItem = listViewItem?.DataContext as WoLModel;
                WoLPC(selectedItem);
            }
        }
        private void dataListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataListView.SelectedItem != null)
            {
                PingRefConfig.IsEnabled = true;

                // 获取当前选择的项
                WoLModel selectedItem = (WoLModel)dataListView.SelectedItem;

                // 实例化SQLiteHelper
                SQLiteHelper dbHelper = new SQLiteHelper();
                // 查询数据
                List<WoLModel> dataList = dbHelper.GetDataListById(selectedItem.Id);

                // 将数据列表绑定到ListView
                dataListView2.ItemsSource = dataList;
            }
        }
    }
}
