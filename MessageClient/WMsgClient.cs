using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Skyfire.Client.MessageClient
{
    public class WMsgClient : MsgClient
    {
        //protected UdpClient _client;

        public WMsgClient()
        {
            // 创建UDP客户端并连接服务
            _client = new UdpClient();
            _client.Connect("localhost", 3723);
        }

        //public void Close()
        //{
        //    // 关闭UDP客户端
        //    _client.Close();
        //}

        //public void SendMessage(String msg)
        //{
        //    byte[] buffer = Encoding.Default.GetBytes(msg);
        //    _client.Send(buffer, buffer.Length);
        //}
    }
}
