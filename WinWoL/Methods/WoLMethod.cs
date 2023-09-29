using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinWoL.Methods
{
    class WoLMethod
    {
        // 获取域名对应的IP
        public static IPAddress domain2ip(string domain)
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
        // 以UDP协议发送MagicPacket
        public static void sendMagicPacket(string macAddress, IPAddress ipAddress, int port)
        {
            // 将传入的Mac地址字符串分割为十六进制字符串数组
            // hexStrings = {"11", "22", "33", "44", "55", "66"}
            string s = macAddress;
            string[] hexStrings = s.Split(':');

            // 创建一个byte数组
            byte[] bytes = new byte[hexStrings.Length];
            // 遍历字符串数组，将每个字符串转换为byte值，并存储到byte数组中
            for (int i = 0; i < hexStrings.Length; i++)
            {
                // 使用16作为基数表示十六进制格式
                bytes[i] = Convert.ToByte(hexStrings[i], 16);
            }
            // 将MAC地址转换为字节数组：byte[] mac = new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 };
            byte[] mac = bytes;

            // 创建一个UDP Socket对象
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // 设置需要广播数据
            socket.EnableBroadcast = true;

            // 创建一个魔术包
            byte[] packet = new byte[17 * 6];
            // 填充前6个字节为0xFF
            for (int i = 0; i < 6; i++)
                packet[i] = 0xFF;
            // 填充后面16个重复的MAC地址字节
            for (int i = 1; i <= 16; i++)
                for (int j = 0; j < 6; j++)
                    packet[i * 6 + j] = mac[j];

            // 多次发送，避免丢包
            for (int i = 0; i < 10; i++)
            {
                // 发送数据
                socket.SendTo(packet, new IPEndPoint(ipAddress, port));
            }

            // 关闭Socket对象
            socket.Close();
        }
    }
}
