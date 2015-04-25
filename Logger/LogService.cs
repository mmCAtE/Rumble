using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Skyfire.Network;

namespace Skyfire.Service.Logger
{
    /// <summary>
    /// 本地日志服务。
    /// 将日志服务安装为一个Windows服务，以服务的形式进行日志输出。
    /// 在日志服务中使用一个UDP监听来获取客户方传递过来的日志内容。在UDP监听线程中将获取到的日志数据保存
    /// 到消息队列中(当前使用了双消息队列来缓冲数据)。当消息队列中的数据符合了输入条件后，会启动两外一个
    /// 线程对当前符合条件的消息队列中的数据输出到日志文件中，输出完成后会重置这个消息队列的状态，并使输
    /// 出线程挂起。
    /// @author mmCAtE
    /// @version 0.1
    /// @date 2015-03-06
    /// </summary>
    public partial class LocalLogger : ServiceBase
    {
        /// <summary>
        /// 操作成功标识
        /// </summary>
        private const byte ALL_OPERATE_DONE = 0x00;

        /// <summary>
        /// 线程是否运行的标识
        /// @since 0.1
        /// </summary>
        private bool _isListener;
        // 日志服务中提供两个消息队列，交替进行日志内容的存入和取出，防止因为存入和取出两个动作
        // 发生冲突导致死锁
        /// <summary>
        /// 第一个日志消息队列
        /// @since 0.1
        /// </summary>
        private Queue _logMessage1;
        /// <summary>
        /// 第二个日志消息队列
        /// @since 0.1
        /// </summary>
        private Queue _logMessage2;
        /// <summary>
        /// 当前正在使用哪个消息队列的标识。对于存入和取出，这个标示所代表的含义是相反的。
        /// 例如，当_whickQueue=true时，在存入动作表示使用第一个队列，而在取出动作中则表示使用第二个队列
        /// @since 0.1
        /// </summary>
        private bool _whickQueue;
        /// <summary>
        /// 线程的开关量集合
        /// @since 0.1
        /// </summary>
        private WaitHandle[] _watiHandle;
        /// <summary>
        /// 日志文件输出线程的开关量
        /// @since 0.1
        /// </summary>
        private EventWaitHandle _writeEvent;
        /// <summary>
        /// 日志输出线程
        /// @since 0.1
        /// </summary>
        private Thread _writeThread;
        /// <summary>
        /// 日志文件输出流
        /// @since 0.1
        /// </summary>
        private StreamWriter _logWriter;
        /// <summary>
        /// UDP监听器。通过UPD监听端口来获取要输出的日志内容。
        /// @since 0.1
        /// </summary>
        private UDPListener _listener;

        /// <summary>
        /// 构造方法
        /// @since 0.1
        /// </summary>
        public LocalLogger()
        {
            InitializeComponent();

            // 创建开关量集合
            _watiHandle = new WaitHandle[1];
            _writeEvent = new AutoResetEvent(true);
            _watiHandle[0] = _writeEvent;
            // 日志文件输出路径
            String logDir = String.Format("{0}\\{1}\\{2}", "C:\\Users\\HaiHui\\Documents",
                "InstantMessage.Client", "Log");
            String logPath = String.Format("{0}\\{1}_{2}.log", logDir, Assembly.GetExecutingAssembly().GetName().Name,
                DateTime.Now.ToString("yy_MM_dd"));
            _logWriter = File.AppendText(logPath);
            Console.SetOut(_logWriter);
            // 创建UDP监听器
            _listener = new UDPListener(3721);
            _listener.DataReceivedEvent += OnSocketReveiveData;
            // 创建消息队列
            _whickQueue = true;
            _logMessage1 = Queue.Synchronized(new Queue());
            _logMessage2 = Queue.Synchronized(new Queue());
        }

        /// <summary>
        /// 服务启动事件重载
        /// @since 0.1
        /// </summary>
        /// <param name="args">事件参数</param>
        protected override void OnStart(string[] args)
        {
            // 线程运行状态
            _isListener = true;
            // 创建并启动线程
            _writeThread = new Thread(OnWriteRecivedData);
            _writeThread.Name = "DataWriter";
            _writeThread.Start();
            // 启动UDP监听器
            _listener.Start();
        }

        /// <summary>
        /// 服务停止事件重载
        /// @since 0.1
        /// </summary>
        protected override void OnStop()
        {
            // 停止UDP监听器
            _listener.Stop();
            // 停止日志文件输出流
            if (_logWriter != null)
            {
                _logWriter.Close();
            }
            Console.Out.Close();
        }

        /// <summary>
        /// UDP监听器接收到数据时的事件响应
        /// @since 0.1
        /// </summary>
        /// <param name="sender">UDP监听器</param>
        /// <param name="args">接收到的数据内容</param>
        private void OnSocketReveiveData(object sender, NetworkDataReceivedEventArgs args)
        {
            // 将字节数据转换为字符串
            String data = Encoding.Default.GetString(args.ReceivedData, 0, args.ReceivedLength);
            // 判断使用哪个数据队列(此时相当于是数据获取阶段)
            if (_whickQueue)
            {
                // 向第一个数据队列中放入日志内容
                lock (_logMessage1.SyncRoot)
                {
                    _logMessage1.Enqueue(data);
                }
            }
            else
            {
                // 向第二个数据队列中放入日志内容
                lock (_logMessage2.SyncRoot)
                {
                    _logMessage2.Enqueue(data);
                }
            }
            // 设置日志输出线程开始运行
            _writeEvent.Set();
        }

        /// <summary>
        /// 日志输出线程逻辑内容
        /// @since 0.1
        /// </summary>
        private void OnWriteRecivedData()
        {
            // 线程是否运行状态的标识
            while (_isListener)
            {
                // 获取当前开关变量，并判断线程中逻辑操作的类型
                int index = WaitHandle.WaitAny(_watiHandle);
                // 输出日志内容
                if (_watiHandle[index] == _writeEvent)
                {
                    String msg = "";
                    // 反向消息队列判断标识
                    _whickQueue = !_whickQueue;
                    // // 判断使用哪个数据队列(此时相当于是数据取出阶段)
                    if (_whickQueue)
                    {
                        // 从第二个数据队列中取出数据
                        lock (_logMessage2.SyncRoot)
                        {
                            //while ((msg = _logMessage2.Dequeue().ToString()) != null)
                            // 数据队列中有数据则取出来写入日志文件
                            while (_logMessage2.Count > 0)
                            {
                                msg = _logMessage2.Dequeue().ToString();
                                Console.Out.WriteLine(msg);
                            }
                        }
                    }
                    else
                    {
                        // 从第一个数据队列中取出数据
                        lock (_logMessage1.SyncRoot)
                        {
                            //while ((msg = _logMessage1.Dequeue().ToString()) != null)
                            // 数据队列中有数据则取出来写入日志文件
                            while (_logMessage1.Count > 0)
                            {
                                msg = _logMessage1.Dequeue().ToString();
                                Console.Out.WriteLine(msg);
                            }
                        }
                    }
                    // 刷新输出流
                    Console.Out.Flush();
                    // 关闭线程的开关变量
                    _writeEvent.Reset();
                }
            }
        }
    }
}
