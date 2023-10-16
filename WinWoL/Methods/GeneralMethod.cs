using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinWoL.Methods
{
    public class GeneralMethod
    {
        // SSH执行命令
        public static string SendSSHCommand(string sshCommand, string sshHost, string sshPort, string sshUser, string sshPasswd, string sshKey, string privateKeyIsOpen)
        {
            SshClient sshClient;

            try
            {
                if (privateKeyIsOpen == "True")
                {
                    PrivateKeyFile privateKeyFile = new PrivateKeyFile(sshKey);
                    ConnectionInfo connectionInfo = new ConnectionInfo(sshHost, sshUser, new PrivateKeyAuthenticationMethod(sshUser, new PrivateKeyFile[] { privateKeyFile }));
                    sshClient = new SshClient(connectionInfo);
                }
                else
                {
                    sshClient = new SshClient(sshHost, int.Parse(sshPort), sshUser, sshPasswd);
                }
            }
            catch
            {
                return "SSH connect fail.";
            }

            try
            {
                sshClient.Connect();

                string res = null;

                if (sshClient.IsConnected)
                {
                    SshCommand SSHCommand = sshClient.RunCommand(sshCommand);

                    if (!string.IsNullOrEmpty(SSHCommand.Error))
                    {
                        res = "Error: " + SSHCommand.Error;
                    }
                    else
                    {
                        res = SSHCommand.Result;
                    }
                }
                sshClient.Disconnect();
                return res;
            }
            catch
            {
                return "SSH connect fail.";
            }
        }
    }
}
