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

namespace WinWoL.Pages
{
    public sealed partial class WoL : Page
    {
        // 定义一个ObservableCollection用于保存数据
        private ObservableCollection<Models.WoLModel> dataList = new ObservableCollection<Models.WoLModel>();
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public WoL()
        {
            this.InitializeComponent();
            // 加载数据
            LoadData();
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
            Dialogs.AddWoL dialog = new Dialogs.AddWoL(initialWoLData);
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
        private async void ChangeConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataListView.SelectedItem != null)
            {
                // 获取WoLModel对象
                WoLModel selectedModel = (WoLModel)dataListView.SelectedItem;

                // 创建一个新的dialog对象
                Dialogs.AddWoL dialog = new Dialogs.AddWoL(selectedModel);
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
                    dbHelper.UpdateData(selectedModel);
                    // 重新加载数据
                    LoadData();
                }
            }
            else
            {
                NeedSelectedTips.IsOpen = true;
            }
        }
        private void DelConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataListView.SelectedItem != null)
            {
                // 获取WoLModel对象
                WoLModel selectedModel = (WoLModel)dataListView.SelectedItem;
                // 实例化SQLiteHelper
                SQLiteHelper dbHelper = new SQLiteHelper();
                // 删除数据
                dbHelper.DeleteData(selectedModel);
                // 重新加载数据
                LoadData();
                // 隐藏提示Flyout
                if (this.DelConfig.Flyout is Flyout f)
                {
                    f.Hide();
                }
            }
            else
            {
                NeedSelectedTips.IsOpen = true;
            }
        }
        // 网络唤醒按钮点击
        private void WoLConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataListView.SelectedItem != null)
            {
                // 获取WoLModel对象
                WoLModel selectedModel = (WoLModel)dataListView.SelectedItem;
                // 实例化SQLiteHelper
                SQLiteHelper dbHelper = new SQLiteHelper();
                // 根据id获得数据
                WoLModel woLModel = dbHelper.GetDataById(selectedModel);
                IPAddress wolAddress = WoLMethod.DomainToIp(woLModel.WoLAddress,"IPv4");
                WoLMethod.sendMagicPacket(woLModel.MacAddress, wolAddress, int.Parse(woLModel.WoLPort));
            }
            else
            {
                NeedSelectedTips.IsOpen = true;
            }
        }
        // 远程桌面按钮点击
        private void RDPConfigButton_Click(object sender, RoutedEventArgs e)
        {
        }
        // 关闭电脑按钮点击
        private void ShutdownConfigButton_Click(object sender, RoutedEventArgs e)
        {
        }
        // 导入配置按钮点击
        private async void ImportConfig_Click(object sender, RoutedEventArgs e)
        {
            // 实例化SQLiteHelper
            SQLiteHelper dbHelper = new SQLiteHelper();
            // 获取导入的数据
            WoLModel woLModel = await WoLMethod.ImportConfig();
            // 插入新数据
            dbHelper.InsertData(woLModel);
            // 重新加载数据
            LoadData();
        }
        // 导出配置按钮点击
        private async void ExportConfig_Click(object sender, RoutedEventArgs e)
        {
            if (dataListView.SelectedItem != null)
            {
                // 获取WoLModel对象
                WoLModel selectedModel = (WoLModel)dataListView.SelectedItem;
                string result = await WoLMethod.ExportConfig(selectedModel);
            }
            else
            {
                NeedSelectedTips.IsOpen = true;
            }
        }
    }
}
