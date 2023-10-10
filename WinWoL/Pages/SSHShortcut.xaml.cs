using Microsoft.UI.Dispatching;
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
using System.Threading;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using WinWoL.Datas;
using WinWoL.Methods;
using WinWoL.Models;
using WinWoL.Pages.Dialogs;

namespace WinWoL.Pages
{
    public sealed partial class SSHShortcut : Page
    {
        // 启用本地设置数据
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        ResourceLoader resourceLoader = new ResourceLoader();
        private DispatcherQueue _dispatcherQueue;
        public SSHShortcut()
        {
            this.InitializeComponent();

            LoadData();
            LoadString();
        }
        private void LoadData()
        {
            // 实例化SQLiteHelper
            SQLiteHelper dbHelper = new SQLiteHelper();
            // 查询数据
            List<SSHModel> dataList = dbHelper.QuerySSHData();

            // 将数据列表绑定到ListView
            dataGridView.ItemsSource = dataList;
        }
        private void LoadString()
        {
            Header.Text = resourceLoader.GetString("SSHHeader");
        }
        // 添加/修改配置按钮点击
        private async void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建一个初始的WoLModel对象
            SSHModel initialWoLData = new SSHModel();

            // 创建一个新的dialog对象
            AddSSH dialog = new AddSSH(initialWoLData);
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
                dbHelper.InsertSSHData(initialWoLData);
                // 重新加载数据
                LoadData();
            }
        }
        private async void ConfirmReplace_Click(object sender, RoutedEventArgs e)
        {
            //// 关闭二次确认Flyout
            //confirmationReplaceFlyout.Hide();
            //// 获取NSModel对象
            //WoLModel selectedModel = (WoLModel)dataListView.SelectedItem;
            //ImportConfig.IsEnabled = false;
            //// 实例化SQLiteHelper
            //SQLiteHelper dbHelper = new SQLiteHelper();

            //// 获取导入的数据
            //WoLModel newModel = await WoLMethod.ImportConfig();

            //if (newModel != null)
            //{
            //    // 获取当前配置的ID
            //    int id = selectedModel.Id;
            //    // 赋给导入的配置
            //    newModel.Id = id;
            //    // 插入新数据
            //    dbHelper.UpdateData(newModel);
            //    // 重新加载数据
            //    LoadData();
            //}
            //ImportConfig.IsEnabled = true;
        }

        private void ConfirmDelete_Click(object sender, RoutedEventArgs e)
        {
            // 关闭二次确认Flyout
            confirmationFlyout.Hide();
            //DelThisConfig(selectedWoLModel);
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
        private void OnGridViewRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // 获取右键点击的Item
            FrameworkElement gridViewItem = (sender as FrameworkElement);

            // 获取右键点击的数据对象（WoLModel）
            WoLModel selectedItem = gridViewItem?.DataContext as WoLModel;

            if (selectedItem != null && InProgressing.IsActive == false)
            {
                // 将右键点击的项设置为选中项
                dataGridView.SelectedItem = selectedItem;

                // 创建ContextMenu
                MenuFlyout menuFlyout = new MenuFlyout();

                MenuFlyoutItem sshShutdownPCMenuItem = new MenuFlyoutItem
                {
                    Text = "执行 SSH 脚本"
                };
                sshShutdownPCMenuItem.Click += (sender, e) =>
                {
                };
                menuFlyout.Items.Add(sshShutdownPCMenuItem);

                // 添加分割线
                MenuFlyoutSeparator separator = new MenuFlyoutSeparator();
                menuFlyout.Items.Add(separator);

                MenuFlyoutItem exportMenuItem = new MenuFlyoutItem
                {
                    Text = "导出配置"
                };
                exportMenuItem.Click += (sender, e) =>
                {
                    //ExportConfigFunction();
                };
                menuFlyout.Items.Add(exportMenuItem);

                MenuFlyoutItem replaceMenuItem = new MenuFlyoutItem
                {
                    Text = "覆盖配置"
                };
                replaceMenuItem.Click += (sender, e) =>
                {
                    // 弹出二次确认Flyout
                    //confirmationReplaceFlyout.ShowAt(listViewItem);
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
                    //EditThisConfig(selectedItem);
                };
                menuFlyout.Items.Add(editMenuItem);

                MenuFlyoutItem copyMenuItem = new MenuFlyoutItem
                {
                    Text = "复制配置"
                };
                copyMenuItem.Click += (sender, e) =>
                {
                    //CopyThisConfig(selectedItem);
                };
                menuFlyout.Items.Add(copyMenuItem);

                MenuFlyoutItem deleteMenuItem = new MenuFlyoutItem
                {
                    Text = "删除配置"
                };
                deleteMenuItem.Click += (sender, e) =>
                {
                    //// 保存选中的数据对象以便确认后执行
                    //selectedWoLModel = selectedItem;
                    //// 弹出二次确认Flyout
                    //confirmationFlyout.ShowAt(listViewItem);
                };
                menuFlyout.Items.Add(deleteMenuItem);

                Thread.Sleep(10);

                // 在指定位置显示ContextMenu
                menuFlyout.ShowAt(gridViewItem, e.GetPosition(gridViewItem));
            }

        }
        private void OnGridViewDoubleTapped(object sender, RoutedEventArgs e)
        {
            // 处理左键双击事件的代码
            // 获取右键点击的Item
            //FrameworkElement listViewItem = (sender as FrameworkElement);
            //if (listViewItem != null && InProgressing.IsActive == false)
            //{
            //    // 获取点击的数据对象
            //    WoLModel selectedItem = listViewItem?.DataContext as WoLModel;
            //    WoLPC(selectedItem);
            //}
        }
    }
}
