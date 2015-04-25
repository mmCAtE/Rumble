using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Skyfire.Network;

namespace MsgUI
{
    public partial class MsgNotify : Form
    {
        private UDPListener udp;

        public MsgNotify()
        {
            InitializeComponent();
        }

        private void MsgNotify_Load(object sender, EventArgs e)
        {
            String serviceName = "LocalMessager";
            StartService(serviceName);

            udp = new UDPListener(3723);
            udp.DataReceivedEvent += OnServiceDataReceive;
            udp.Start();
        }

        private void OnServiceDataReceive(object sender, NetworkDataReceivedEventArgs args)
        {
            msgNotifyIcon.ShowBalloonTip(60000, "新消息", "您有新短消息，请注意查收", ToolTipIcon.Info);
        }

        //protected override void DefWndProc(ref Message m)
        //{
        //    switch(m.Msg)
        //    {
        //        case 0x0924:
        //        {
        //            msgNotifyIcon.ShowBalloonTip(60000, "新消息", "您有新短消息，请注意查收", ToolTipIcon.Info);
        //            break;
        //        }
        //        default:
        //        {
        //            base.DefWndProc(ref m);
        //            break;
        //        }
        //    }
        //}

        private void exitNotify_Click(object sender, EventArgs e)
        {
            if(udp !=null)
            {
                udp.Stop();
            }
            Close();
        }

        private void InstallService(String serviceName)
        {
            Process p = new Process();
            //p.StartInfo = RunAsAdministrator();
            p.StartInfo.FileName = "cmd.exe";
            // 是否使用操作系统shell启动
            p.StartInfo.UseShellExecute = false;
            // 是否接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardInput = true;
            // 是否由调用程序获取输出信息
            p.StartInfo.RedirectStandardOutput = false;
            // 是否重定向标准错误输出
            p.StartInfo.RedirectStandardError = false;
            // 是否显示程序窗体
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Verb = "runas";
            p.Start();

            // 向cmd输入命令
            p.StandardInput.WriteLine("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\InstallUtil.exe /u D:\\output\\Services\\Debug\\LocalMessager.exe");
            p.StandardInput.WriteLine("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\InstallUtil.exe D:\\output\\Services\\Debug\\LocalMessager.exe");
            p.StandardInput.WriteLine("exit");
            /*
             * 如果在要自行的命令前加&，说明这个命令无论前面的命令的执行情况如何(执行成功还是失败，设置是否执行完成)
             * 都会执行后面的命令。
             * &&表示前面必须有一个命令执行成功后才会执行后面的命令
             * ||表示前面必须有一个命令执行失败后才会执行后面的命令
             */
            //p.StandardInput.WriteLine("&exit");
            p.StandardInput.AutoFlush = true;

            // 等待程序执行完退出
            p.WaitForExit();
            p.Close();
        }

        private void StartService(String serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                if (service.Status == ServiceControllerStatus.Stopped || service.Status == ServiceControllerStatus.Paused)
                {
                    service.Start();
                }
            }
            catch (InvalidOperationException ex)
            {
                InstallService(serviceName);
                StartService(serviceName);
            }
        }

        private bool IsRunAsAdministrator()
        {
            //ProcessStartInfo psi = new ProcessStartInfo();
            // 获取当前登录用户的Windows标识
            WindowsIdentity indentity = WindowsIdentity.GetCurrent();
            // 创建Windows用户主题
            //Application.EnableVisualStyles();
            WindowsPrincipal principal = new WindowsPrincipal(indentity);
            // 判断当前用户是否是管理员
            //if(principal.IsInRole(WindowsBuiltInRole.Administrator))
            //{
            //}
            //else
            //{
            //    psi.Verb = "runas";
            //}

            //return psi;
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
