using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Runtime.InteropServices;
using System.Management;
using System.IO;
using System.Windows.Forms;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Server1
{
    class SocketServer
    {
        public string messageA = "";
        public Socket server;
        public Form myf;
        private IPEndPoint ip;
        private List<Thread> thread_list; 
        private int max_conn;
        public List<string> messageSend = new List<string>(); //для отправки сообщения
        public List<string> ClientList = new List<string>();
        public SocketServer(string ip, Int32 port, Form form)
        {
            this.max_conn = 2; //сколько клиентов могут подключаться к этому серверу
            this.thread_list = new List<Thread>();

            //процедура создания сервера чтобы он запустился и слушал подключения
            this.ip = new IPEndPoint(IPAddress.Any, port);
            this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.server.Bind(this.ip);
            this.server.Listen(this.max_conn);
            myf = form;
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

        //закрытие подключений
        public void Dispose()
        {
            foreach (Thread th in thread_list)
            {
                th.Interrupt();
            }
            server.Close();
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class OSVersionInfo
        {
            public int dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public String szCSDVersion;
            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetric smIndex);
        public enum SystemMetric : int
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
        }

        class Kernel
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool GetVersionEx([In, Out] OSVersionInfo info);
        }

        //слушаем клиента
        public void Listening()
        {
            while (true)
            {
                try
                {
                    using (Socket client = this.server.Accept())
                    {
                        this.ClientList.Add(client.RemoteEndPoint.ToString());
                        if (client.Connected)
                        {
                            List<string> message = new List<string>(); //строка, куда добавлять будем инфу для передачи
                                                        
                            int error = Marshal.GetLastWin32Error();
                            int CursorX = Cursor.Position.X;
                            int CursorY = Cursor.Position.Y;

                            int second = GetSystemMetrics(SystemMetric.SM_CXSCREEN);
                            int third = GetSystemMetrics(SystemMetric.SM_CYSCREEN);

                          
                            //ниже вот добавляем все что нужно для передачи
                            message.Add("Код последней ошибки: " + error + ";" + " " + " Положение курсора: " + CursorX + " " + CursorY);
                            message.Add("[end]"); 

                            if (messageA != message[0]+message[1])//если полученное сообщение не равно отправленному заполняем и отправляем (2 доп задание)
                            {
                                messageA = message[0]+message[1];
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
                catch
                {
                }
            }
        }
        //передача
        public void SendAll(Socket handler, List<string> message)
        {
            for (int i = 0; i < message.Count;)
            {
                //передача по байтам
                Send(message[i], handler);
                message.RemoveAt(i);
                Thread.Sleep(500);
            }
        }
        //сенд который делит на байты и передает
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
