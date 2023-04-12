using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Клиент
{
    public partial class Form1 : Form
    {
        public static string ip = "127.0.0.1";
        public static int port = 80;
        public static IPEndPoint end = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        public static Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static int time=0;
        public static bool voteStarted=false;
        public static string text1 = "--> --> -->";
        public static string text2 = "15";
        public static bool enb1 = false;
        public static bool enb2 = false;
        public static bool enb3 = true;
        public static bool vib2 = false;

        public Form1()
        {

            InitializeComponent();
        }
        public static string Decoder(byte[] dna)
            {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < dna.Length; i++)
            {
                switch (dna[i])
                {
                    case 0:
                        {
                            result.Append("A");
                            break;
                        }
                    case 1:
                        {
                            result.Append("U");
                            break;
                        }
                    case 2:
                        {
                            result.Append("C");
                            break;
                        }
                    case 3:
                        {
                            result.Append("G");
                            break;
                        }
                }
            }
            return result.ToString();
            }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] data = { 1 };
            ClientSocket.Send(data);
            time = 0;
            vib2 = false;
            text1 = "Ждите";
            enb1 = false;
            enb2 = false;
            voteStarted = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] data = { 0 };
            ClientSocket.Send(data);
            time = 0;
            vib2 = false;
            text1 = "Ждите";
            enb1 = false;
            enb2 = false;
            voteStarted = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            void Vote()
            {
                voteStarted = true;
                while (voteStarted)
                {
                    text2 = (15 - time).ToString();
                    time += 1;
                    if (time == 15)
                    {
                        time = 0;
                        byte[] data = { 2 };
                        ClientSocket.Send(data);
                        text1 = "Ждите";
                        enb1 = false;
                        enb2 = false;
                        voteStarted = false;
                        vib2 = false;
                    }
                    Thread.Sleep(1000);
                }
            }
            void Check()
            {
                while (true)
                {
                    byte[] dna = new byte[12];
                    ClientSocket.Receive(dna);
                    if (dna[0] == 5)
                    {
                        ClientSocket.Shutdown(SocketShutdown.Both);
                        ClientSocket.Close();
                        text1 = "Cлишком много участников";
                        break;
                    }
                    else
                    {
                        text1 = Decoder(dna);
                        text2 = "15";
                        vib2 = true;
                        Thread Voter = new Thread(Vote);
                        Voter.Start();
                        enb1 = true;
                        enb2 = true;
                        Thread.Sleep(1000);
                    }
                }
            }
            ClientSocket.Connect(end);
            enb3 = false;
            text1 = "Ждите";
            Thread Checker = new Thread(Check);
            Checker.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = text1;
            label2.Text = text2;
            label2.Visible = vib2;
            button1.Enabled = enb1;
            button2.Enabled = enb2;
            button3.Enabled = enb3;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
  
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ClientSocket.Connected)
            {
                byte[] data = { 3 };
                ClientSocket.Send(data);
                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Close();
            }
        }
    }
}
