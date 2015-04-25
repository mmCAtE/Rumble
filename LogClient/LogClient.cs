using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skyfire.Client.LogClient
{
    /// <summary>
    /// 日志输出客户端组件。封装了和日志服务的通信动作
    /// @author mmCAtE
    /// @version 0.1
    /// @date 2015-03-06
    /// </summary>
    public class LogClient
    {
        /// <summary>
        /// 日志传输线程
        /// @since 0.1
        /// </summary>
        private Thread _writeThread;
        /// <summary>
        /// 线程开关变量集合
        /// @since 0.1
        /// </summary>
        private WaitHandle[] _watiHandle;
        /// <summary>
        /// 输出开关变量
        /// @since 0.1
        /// 
        /// </summary>
        private EventWaitHandle _writeEvent;
        /// <summary>
        /// UDP客户端
        /// @since 0.1
        /// </summary>
        private UdpClient _client;
        /// <summary>
        /// 线程运行标识
        /// @since 0.1
        /// </summary>
        private bool _isRunning;
        /// <summary>
        /// 日志内容队列
        /// @since 0.1
        /// </summary>
        private Queue _logMsg;

        /// <summary>
        /// 构造方法
        /// @since 0.1
        /// </summary>
        public LogClient()
        {
            // 创建数据队列
            _logMsg = Queue.Synchronized(new Queue());
            // 设置线程的运行标识
            _isRunning = true;
            // 创建线程开关变量
            _writeEvent = new AutoResetEvent(true);
            _watiHandle = new WaitHandle[1];
            _watiHandle[0] = _writeEvent;
            // 创建并启动现场
            _writeThread = new Thread(OnWriteLogData);
            _writeThread.Start();
            // 创建UDP客户端并连接服务
            _client = new UdpClient();
            _client.Connect("localhost", 3721);
        }

        /// <summary>
        /// 写日志
        /// @since 0.1
        /// </summary>
        /// <param name="content">日志内容文本</param>
        public void WriteLog(String content)
        {
            // 将日志内容放入数据队列
            lock (_logMsg.SyncRoot)
            {
                _logMsg.Enqueue(content);
                _writeEvent.Set();
            }
        }

        /// <summary>
        /// 关闭日志客户端
        /// @since 0.1
        /// </summary>
        public void Close()
        {
            // 设置线程运行标识
            _isRunning = false;
            // 终止线程。此处是通过使线程出错来终止
            _writeThread.Abort();
            // 关闭UDP客户端
            _client.Close();
        }

        /// <summary>
        /// 日志输出线程逻辑内容
        /// @since 0.1
        /// </summary>
        private void OnWriteLogData()
        {
            // 线程是否运行状态的标识
            while (_isRunning)
            {
                // 获取当前开关变量，并判断线程中逻辑操作的类型
                int index = WaitHandle.WaitAny(_watiHandle);
                // 输出日志内容
                if (_watiHandle[index] == _writeEvent)
                {
                    String msg = "";
                    // 从数据队列中取出数据
                    lock (_logMsg.SyncRoot)
                    {
                        // 遍历并发送数据队列中的所有数据
                        while (_logMsg.Count>0)
                        //(msg = _logMsg.Dequeue() != null ? _logMsg.Dequeue().ToString() : null) != null)
                        {
                            msg = msg + "\n" + _logMsg.Dequeue().ToString();
                            byte[] buffer = Encoding.Default.GetBytes(msg);
                            _client.Send(buffer, buffer.Length);
                        }
                        //byte[] buffer = Encoding.Default.GetBytes(msg);
                        //if (buffer.Length > 0)
                        //{
                        //    _client.Send(buffer, buffer.Length);
                        //}
                    }
                    // 关闭线程的开关变量
                    _writeEvent.Reset();
                }
            }
        }
    }
}
