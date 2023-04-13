using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Data;

namespace Server2
{
    class SocketServer
    {
        //просмотр памяти
        [DllImport("kernel32.dll")]
        //выделение памяти
        public static extern IntPtr GlobalAlloc(int con, int size);
        [DllImport("kernel32.dll")]
        //освобождение памяти
        public static extern int GlobalFree(IntPtr start);
        [DllImport("kernel32.dll")]
        public static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);

        public string messageA = "";
        public struct MEMORYSTATUS
        {
            public UInt32 dwLength;               //Размер структуры, в байтах
            public UInt32 dwMemoryLoad;           //процент занятой памяти
            public UInt32 dwTotalPhys;            //общее кол-во физической(оперативной) памяти
            public UInt32 dwAvailPhys;            //свободное кол-во физической(оперативной) памяти
            public UInt32 dwTotalPageFile;        //предел памяти для системы или текущего процесса
            public UInt32 dwAvailPageFile;        //Максимальный объем памяти,который текущий процесс может передать в байтах.
            public UInt32 dwTotalVirtual;         //общее количество виртуальной памяти(файл подкачки)
            public UInt32 dwAvailVirtual;         //свободное количество виртуальной памяти(файл подкачки)
            public UInt32 dwAvailExtendedVirtual; //Зарезервировано. Постоянно 0.
        }

        public Socket server;
        public Socket client;
        private IPEndPoint ip;
        private List<Thread> thread_list;
        private int max_conn;
        public List<string> messageSend = new List<string>();
        public List<string> ClientList = new List<string>();
        public SocketServer(string ip, Int32 port)
        {
            this.max_conn = 2;
            this.thread_list = new List<Thread>();
            this.ip = new IPEndPoint(IPAddress.Any, port);
            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.server.Bind(this.ip);
            this.server.Listen(this.max_conn);
        }

        public void Start()
        {
            for (int i = 0; i < this.max_conn; i++)
            {
                Thread th = new Thread(Listening);
                th.Start();
                thread_list.Add(th);
            }
        }

        public void Dispose()
        {
            foreach (Thread th in thread_list)
            {
                th.Interrupt();
            }
            server.Close();
        }       
        
        private void Listening()
        {

            while (true)
            {
                try
                {
                    using (Socket client = this.server.Accept())
                    {
                        this.ClientList.Add(client.RemoteEndPoint.ToString());
                        //client.Blocking = true;
                        if (client.Connected)
                        {
                            List<string> message = new List<string>();
                            HashSet<IntPtr> handle_list = new HashSet<IntPtr>();
                            HashSet<uint> potok_list = new HashSet<uint>();
                            string win32_proc_query_string = "select * from CIM_Process";
                            string cim_query_string = "select * from CIM_ProcessExecutable";

                            //Вывод дискриптора и полного имени длл библиотеки

                            MEMORYSTATUS memStatus = new MEMORYSTATUS();
                            GlobalMemoryStatus(ref memStatus);
                            UInt32 mem = memStatus.dwTotalPhys - memStatus.dwAvailPhys;
                            UInt32 virt = memStatus.dwTotalVirtual - memStatus.dwAvailVirtual;

                            message.Add("Используемая физическая память " + mem.ToString() + " %; " + "Используемая вирутальная память " + virt.ToString() + " %");
                            message.Add("[end]");
                            if (messageA != message[0] + message[1])//если полученное сообщение не равно отправленному заполняем и отправляем (2 доп задание)
                            {
                                messageA = message[0] + message[1];
                                SendAll(client, message);//тут передаем;    
                            }
                            else
                            {
                                for (int i = 0; i < message.Count;)//если сообщение равно предидущему удаляем и не отправляем
                                {
                                    //тут вот по байтам он видимо передаем все сообщение
                                    message.RemoveAt(i);
                                }
                            }
                        }
                    }
                }

                catch (SocketException ex) { }

        }
    }
        public void SendAll(Socket handler, List<string> message)
        {
            for (int i = 0; i < message.Count;)
            {
                Send(message[i], handler);
                message.RemoveAt(i);
                Thread.Sleep(5);
            }
        }
        public void Send(string message, Socket handler)
        {
            byte[] tosend = Encoding.UTF8.GetBytes(message);
            try
            {
                handler.Send(tosend, 0, tosend.Length, SocketFlags.None);
            }
            catch
            {
            }
        }
    }
}
