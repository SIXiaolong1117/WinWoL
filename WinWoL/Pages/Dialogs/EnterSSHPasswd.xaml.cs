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
            // ��"ȷ��"��ť����¼��б����û����������
            PasswdData.SSHPasswd = SSHPasswordBox.Password;
        }

        private void MyDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // ��"ȡ��"��ť����¼��в����κβ���
        }
    }
}