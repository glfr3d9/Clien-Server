using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Server2
{
    public partial class Form1 : Form
    {
        SocketServer server;
        public Form1()
        {
            InitializeComponent();
            string MyIp = "";
            foreach (System.Net.IPAddress ip in Dns.GetHostByName(Dns.GetHostName()).AddressList)
            {
                MyIp = ip.ToString();
                break;
            }
            server = new SocketServer(MyIp, 13000);
            server.Start();
            Thread list = new Thread(List);
            list.IsBackground = true;
            list.Start();
        }
        private void Receive()
        {
            for (; ; )
            {
                try
                {
                    if (lbx_clientList.Items.Count != server.ClientList.Count)
                    {
                        lbx_clientList.Invoke(new MethodInvoker(delegate { lbx_clientList.Items.Clear(); }));
                        for (int i = 0; i < server.ClientList.Count; i++)
                            lbx_clientList.Invoke(new MethodInvoker(delegate { lbx_clientList.Items.Add(server.ClientList[i]); }));
                    }
                    Thread.Sleep(100000);
                }
                catch
                { }
            }
        }
        private void List()
        {
            for (; ; Thread.Sleep(200))
                if (server.ClientList.Count != lbx_clientList.Items.Count)
                {
                    lbx_clientList.Invoke(new MethodInvoker(delegate { lbx_clientList.Items.Clear(); }));
                    for (int i = 0; i < server.ClientList.Count; i++)
                        lbx_clientList.Invoke(new MethodInvoker(delegate { lbx_clientList.Items.Add(server.ClientList[i]); }));
                }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Dispose();
            this.Dispose();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            server.Dispose();
            this.Dispose();
            Application.Exit();
        }
    }
}
