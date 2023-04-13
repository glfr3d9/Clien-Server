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
using System.Diagnostics;

namespace Server1
{
    public partial class Form1 : Form
    {
        SocketServer server;

        public Form1()
        {
            InitializeComponent();
            //запуск сервера
            string MyIp = Dns.GetHostName();
            server = new SocketServer(MyIp, 12000, this);
            server.Start();
            Thread list = new Thread(List); //если вдруг подключатся несколько клиентов
            list.IsBackground = true;
            list.Start();
            this.Show();
            
        }

        private void List()
        {
            for (; ; Thread.Sleep(200))
                if (server.ClientList.Count != lbx_clientList.Items.Count)
                {
                    server.messageA = "";
                    lbx_clientList.Invoke(new MethodInvoker(delegate { lbx_clientList.Items.Clear(); }));
                    for (int i = 0; i < server.ClientList.Count; i++)
                    {  
                        lbx_clientList.Invoke(new MethodInvoker(delegate { lbx_clientList.Items.Add(server.ClientList[i]); }));
                    }
                }
        }
        //если закрыть форму на крестик
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
                server.Dispose();
                Dispose();
                Application.Exit();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            
        }


        //если закрыть форму по кнопке
        private void button1_Click(object sender, EventArgs e)
        {
            server.Dispose();
            Dispose();
            Application.Exit();
        }

      
    }
}
