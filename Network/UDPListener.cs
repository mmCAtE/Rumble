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
    /// <summary>
    /// UDP监听器
    /// @author mmCAtE
    /// @version 0.1
    /// @date 2015-03-06
    /// </summary>
    public class UDPListener
    {
        /// <summary>
        /// Socket
        /// @since 0.1
        /// </summary>
        private Socket _listener;
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

        /// <summary>
        /// 构造方法
        /// @since 0.1
        /// </summary>
        /// <param name="port">要监听的端口号</param>
        public UDPListener(int port)
        {
            _port = port;
        }

        /// <summary>
        /// 启动UDP监听器
        /// @since 0.1
        /// </summary>
        public void Start()
        {
            //得到本机IP，设置TCP端口号         
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, _port);
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //绑定网络地址
            _listener.Bind(ip);
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
            _listener.Close();
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
                // 等待客户端数据。此时会阻塞线程
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint Remote = (EndPoint)(sender);
                args.ReceivedLength = _listener.ReceiveFrom(args.ReceivedData, ref Remote);
                // 事件响应函数不为空时执行
                if (DataReceivedEvent != null)
                {
                    DataReceivedEvent(this, args);
                }
            }
        }
    }
}
