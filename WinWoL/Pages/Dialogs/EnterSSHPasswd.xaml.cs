using Microsoft.UI.Xaml.Controls;
using WinWoL.Models;

namespace WinWoL.Pages.Dialogs
{
    public sealed partial class EnterSSHPasswd : ContentDialog
    {
        public SSHPasswdModel PasswdData { get; private set; }
        public EnterSSHPasswd(SSHPasswdModel sshPasswdModel)
        {
            this.InitializeComponent();
            PrimaryButtonClick += MyDialog_PrimaryButtonClick;
            SecondaryButtonClick += MyDialog_SecondaryButtonClick;
            PasswdData = sshPasswdModel;
        }
        private void MyDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // 在"确定"按钮点击事件中保存用户输入的内容
            PasswdData.SSHPasswd = SSHPasswordBox.Password;
        }

        private void MyDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // 在"取消"按钮点击事件中不做任何操作
        }
    }
}
