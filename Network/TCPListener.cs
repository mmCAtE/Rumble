using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skyfire.Network
{
    public class TCPListener
    {
        /// <summary>
        /// Socket
        /// @since 0.1
        /// </summary>
        private TcpListener _listener;
        /// <summary>
        /// UDP监听线程
        /// @since 0.1
        /// </summary>
        private Thread _lstThread;
        /// <summary>
        /// 是否启动监听线程的标识
        /// @since 0.1
        /// </summary>
        private bool _isListening;
        /// <summary>
        /// UDP监听的端口号
        /// @since 0.1
        /// </summary>
        private int _port;

        /// <summary>
        /// 获取数据后的委托
        /// @since 0.1
        /// </summary>
        /// <param name="sender">UDP监听器本身</param>
        /// <param name="args">数据内容</param>
        public delegate void DataReceivedHandle(Object sender, NetworkDataReceivedEventArgs args);
        /// <summary>
        /// UDP连接中获取到数据的事件
        /// @since 0.1
        /// </summary>
        public event DataReceivedHandle DataReceivedEvent;

        public TCPListener(int port)
        {

            _port = port;
        }

        public void Start()
        {
            //得到本机IP，设置TCP端口号         
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, _port);
            _listener = new TcpListener(ip);
            // 设置线程运行标识
            _isListening = true;
            // 创建并启动线程
            _lstThread = new Thread(OnListenerThreadRun);
            _lstThread.Start();
        }

        /// <summary>
        /// 停止UDP监听器
        /// @since 0.1
        /// </summary>
        public void Stop()
        {
            // 设置线程运行标识
            _isListening = false;
            // 关闭Socket
            _listener.Stop();
        }

        /// <summary>
        /// 监听线程逻辑内容
        /// @since 0.1
        /// </summary>
        private void OnListenerThreadRun()
        {
             // 判断线程运行标识
            while (_isListening)
            {
                // 收到数据时的事件参数
                NetworkDataReceivedEventArgs args = new NetworkDataReceivedEventArgs();
                TcpClient client = _listener.AcceptTcpClient();

                int len = 0;
                while ((len = client.GetStream().Read(args.ReceivedData, 0, args.ReceivedData.Length)) != 0)
                {
                    args.ReceivedLength = len;
                }

                if(DataReceivedEvent != null)
                {
                    DataReceivedEvent(this, args);
                }
            }
        }
    }
}
