using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Skyfire.Client.LogClient;
using Skyfire.Client.MessageClient;
using Skyfire.Network;

namespace LocalTest
{
    public partial class Form1 : Form
    {
        private LogClient logClient;
        private UDPListener udp;
        private MsgClient msgClient;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            logClient = new LogClient();
            logClient.WriteLog(textBox1.Text);
            //Thread.Sleep(1000);
            //logClient.WriteLog("测试日志");
            //logClient.WriteLog("haha");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (msgClient != null)
            {
                msgClient.Close();
            }
            if (logClient != null)
            {
                logClient.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            udp = new UDPListener(3721);
            udp.DataReceivedEvent += udp_DataReceivedEvent;
            udp.Start();
            richTextBox1.Text = richTextBox1.Text + "UDP启动成功\n";
        }

        void udp_DataReceivedEvent(object sender, NetworkDataReceivedEventArgs args)
        {
            String str = Encoding.Default.GetString(args.ReceivedData, 0, args.ReceivedLength);
            //richTextBox1.Text = richTextBox1.Text + str + "\n";
            Console.WriteLine(str);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            udp.Stop();
            richTextBox1.Text = richTextBox1.Text + "UDP停止成功\n";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            msgClient = new MsgClient();
            msgClient.SendMessage(textBox2.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            udp = new UDPListener(3722);
            udp.DataReceivedEvent += udp_DataReceivedEvent2;
            udp.Start();
        }
        private void udp_DataReceivedEvent2(Object sender, NetworkDataReceivedEventArgs args)
        {
            String str = Encoding.Default.GetString(args.ReceivedData, 0, args.ReceivedLength);
            //richTextBox1.Text = richTextBox1.Text + str + "\n";
            Console.WriteLine(str);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            udp.Stop();
            richTextBox1.Text = richTextBox1.Text + "UDP停止成功\n";
        }
    }
}
