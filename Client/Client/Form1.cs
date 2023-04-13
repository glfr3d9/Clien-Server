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
using System.IO;

namespace Client
{
    public partial class Form1 : Form
    {
        int k = 0, p = 0; //счетчики нажатий на кнопку
        int port;
        static SocketClient client;
        public string ReceiveMessageA, ReceiveMessageB, ReceiveMessageC, ReceiveMessageD;// переменная для сравнения, промежуточная переменная (1 доп задание)

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }
        private void connection1() //подключение к 1 серверу
        {
            string MyIp = ServerIP.Text; //создаем переменную для айпи
            client = new SocketClient(MyIp, this.port); //новый клиент
            if (client.Connect())
            {
                //если законнектились то ок
                lbx_message.Invoke(new MethodInvoker(delegate { lbx_message.Items.Add("Данные с 1 сервера:"); }));
                Status1();           

            }
        }

        private void connection2() //подключение к 2 серверу
        {
            string MyIp = ServerIP.Text; //создаем переменную для айпи
            client = new SocketClient(MyIp, this.port); //новый клиент
            if (client.Connect())
            {
                //если законнектились то ок
                lbx_message.Invoke(new MethodInvoker(delegate { lbx_message.Items.Add("Данные с 2 сервера:"); }));
                Status2();
            }
        }
        private void repconnection2()//переподключение к 2 серверу для таймера (1 доп задание)
        {
            string MyIp = ServerIP.Text; //создаем переменную для айпи
            client = new SocketClient(MyIp, this.port); //новый клиент
            if (client.Connect())
            {
                //если законнектились то ок 
                Status2();
            }
        }
        private void repconnection1()//переподключение к 1 серверу для таймера (1 доп задание)
        {
            string MyIp = ServerIP.Text; //создаем переменную для айпи
            client = new SocketClient(MyIp, this.port); //новый клиент
            if (client.Connect())
            {
                //если законнектились то ок 
                Status1();
            }
        }
        private void Status2() //вывод данных с 2 сервера
        {
            //получаем сообщения
            while (true)
            {
                client.Receive(); //получаем сообщение
                if (client.ReceiveMessage.Count > 1)
                    if (client.ReceiveMessage[client.ReceiveMessage.Count - 1] == "[end]") break;

            }
     
            for (int i = 0; i < client.ReceiveMessage.Count-1; i++)
            {
                ReceiveMessageA += client.ReceiveMessage[i];//запись сообщения в переменную
            }
            if (ReceiveMessageA != ReceiveMessageB)//сравнение с промежуточной переменной
            {
                lbx_message.Invoke(new MethodInvoker(delegate { lbx_message.Items.Add(ReceiveMessageA); }));
                ReceiveMessageB = ReceiveMessageA;//переопределение промежуточной переменной
            }
            ReceiveMessageA = "";//обнуление переменной для записи сообщения
            for (int i = 0; i < client.ReceiveMessage.Count; i++)
            {
                client.ReceiveMessage.RemoveAt(i);//очистка полученного сообщения
            }
        }
        private void Status1()//вывод данных с 1 сервера
        {
            //получаем сообщения
            while (true)
            {
                client.Receive(); //получаем сообщение
                if (client.ReceiveMessage.Count > 1)
                    if (client.ReceiveMessage[client.ReceiveMessage.Count - 1] == "[end]") break;

            }

            for (int i = 0; i < client.ReceiveMessage.Count - 1; i++)
            {
                ReceiveMessageC += client.ReceiveMessage[i];//запись сообщения в переменную
            }
            if (ReceiveMessageC != ReceiveMessageB)//сравнение с промежуточной переменной
            {
                lbx_message.Invoke(new MethodInvoker(delegate { lbx_message.Items.Add(ReceiveMessageC); }));
                ReceiveMessageD = ReceiveMessageC;//переопределение промежуточной переменной
            }
            ReceiveMessageC= "";//обнуление переменной для записи сообщения
            for (int i = 0; i < client.ReceiveMessage.Count; i++)
            {
                client.ReceiveMessage.RemoveAt(i);//очистка полученного сообщения
            }        
            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //реализация выхода из клиента
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
            System.Windows.Forms.DialogResult.Yes)
                Application.Exit();
        }


        private void btnClear_Click(object sender, EventArgs e)//очистка формы, полное обнуление переменных
        {
            lbx_message.Items.Clear();
            ReceiveMessageA = "";
            ReceiveMessageB = "";
            k = 0;
            p = 0;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (p % 2 != 0)//если кнопка подключения нажата 1 раз, каждый тик (=1000) запрашиваем инфу от сервера
            {
                Thread connect = new Thread(repconnection1);
                connect.Start();
                connect.IsBackground = true;
            }
            else//если нажали на кнопку подключения в 2 раз остановка связи с сервером
                timer2.Stop();
        }

        private void btnCreateConnection_Click(object sender, EventArgs e)
        {
            //получить данные с сервера один
            p++;//счетчик нажатия на кнопку
            ReceiveMessageC = "";
            ReceiveMessageD = "";
            port = 12000;
            if (p % 2 != 0)//если нажали в первый раз то подключение и запуск таймера
            {
                Thread connect = new Thread(connection1);
                connect.Start();
                connect.IsBackground = true;
                timer2.Start();
                btnCreateConnection.Text = "Отключиться от 1 сервера";
            }
            else //если нажали во второй раз
            {
                client.Disconnect();
                btnCreateConnection.Text = "Получить данные с сервера 1";
            }
            
         
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            //получить данные с сервера два
            k++;//счетчик нажатия на кнопку
            ReceiveMessageA = "";
            ReceiveMessageB = "";
            port = 13000;
            if (k % 2 != 0)//если нажали в первый раз топодключение и запуск таймера
            {
                Thread connect = new Thread(connection2);
                connect.Start();
                connect.IsBackground = true;
                //Timer1.Start();
                btnSendMessage.Text = "Отключиться от 2 сервера";
            }
            else //если нажали во второй раз
            {
                client.Disconnect();
                btnSendMessage.Text = "Получить данные с сервера 2";
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (k % 2 != 0)//если кнопка подключения нажата 1 раз, каждый тик (=1000) запрашиваем инфу от сервера
            {
                Thread connect = new Thread(repconnection2);
                connect.Start();
                connect.IsBackground = true;
            }
            else//если нажали на кнопку подключения в 2 раз остановка связи с сервером
                Timer1.Stop();
        }

        private void Form1_Load(object sender, EventArgs e)//определение таймера при загрузке формы
        {
            /*Timer1 = new System.Windows.Forms.Timer();
            Timer1.Interval = 10000;
            Timer1.Tick += new EventHandler(Timer1_Tick);*/
            timer2 = new System.Windows.Forms.Timer();
            timer2.Interval = 5000;
            timer2.Tick += new EventHandler(timer2_Tick);
        }
    }
}
