using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Skyfire.Client.MessageClient;
using Skyfire.Network;

namespace Skyfire.Service.Messager
{
    public partial class LocalMessager : ServiceBase
    {
        private UDPListener _listener;
        private WMsgClient _msgClient; 
        [DllImport("user32.dll")]
        public static extern void PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        private IntPtr _nh;

        public LocalMessager()
        {
            InitializeComponent();

            _nh = IntPtr.Zero;

            // 创建UDP监听器
            _listener = new UDPListener(3722);
            _listener.DataReceivedEvent += OnSocketReveiveData;
            _msgClient = new WMsgClient();
        }

        protected override void OnStart(string[] args)
        {
            // 启动UDP监听器
            _listener.Start();
        }

        protected override void OnStop()
        {
            // 停止UDP监听器
            _listener.Stop();
            _msgClient.Close();
        }

        private void OnSocketReveiveData(object sender, NetworkDataReceivedEventArgs args)
        {
            String data = Encoding.Default.GetString(args.ReceivedData);
            //if (data.StartsWith("{[IntPtr]}"))
            //{
            //    int index = data.IndexOf(":");
            //    String strHwnd = data.Substring(index + 1);
            //    _nh = new IntPtr(Convert.ToInt32(strHwnd));
            //}
            //else
            //{
            //    PostMessage(_nh, 0x0924, 0, 0);
            //}
            _msgClient.SendMessage("ShowTip");
        }
    }
}
