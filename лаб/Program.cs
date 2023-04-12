using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace лаб
{
    internal class Program
    {
        public static byte save = 0, delete = 0;
        public static Socket[] users = new Socket[5];
        public static byte[] dna = new byte[12];
        static void randomDna(ref byte[] dna)
        {
            Random rnd = new Random();
            for (int i = 0; i < dna.Length; i++)
            {
                dna[i] = (byte)rnd.Next(0, 4);
            }
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
        static void SendDna()
        {
            while (true)
            {
                randomDna(ref dna);
                Console.WriteLine(Decoder(dna));
                foreach (Socket u in users)
                {
                    if (u!=null)
                    {
                        u.Send(dna);
                    }
                }
                Thread.Sleep(20000);
                Console.WriteLine("Итоги Голосования");
                Console.WriteLine($"Сохранить {save}   Удалить {delete}");
                if (save > delete)
                {
                    Console.WriteLine("Решение большинства- Сохранить");
                }
                else if (save < delete)
                {
                    Console.WriteLine("Решение большинства- Удалить");
                }
                else if (save == delete)
                {
                    Console.WriteLine("Решение большинства- Неопределенно");
                }
                save = 0;
                delete = 0;
            }
        }
        static void ReciveVote(object index)
        {
            byte[] message = new byte[1];
            bool flag = true;
            Socket user = users[(int)index];
            while (flag)
            {
                user.Receive(message);
                if (message[0] == 0)
                {
                    delete += 1;
                }
                else if (message[0] == 1)
                {
                    save += 1;
                }
                else if (message[0] == 2)
                {
                    continue;
                }
                else if ((message[0] == 3))
                {
                    flag = false;
                    user.Shutdown(SocketShutdown.Both);
                    user.Close();
                    users[(int)index] = null;
                }
            }
        }
        static void NewVoter(object ser)
        {
            Socket server = (Socket)ser;
            while (true)
            {
                Socket new_user = server.Accept();
                bool flag = false;
                for (int i = 0; i < users.Length; i++)
                {
                    if (users[i]==null)
                    {
                        users[i] = new_user;
                        Thread NewVoterThread1 = new Thread(new ParameterizedThreadStart(ReciveVote));
                        NewVoterThread1.Start(i);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    byte[] error = new byte[12];
                    error[0] = 5;
                    new_user.Send(error);
                    Console.WriteLine("Больше нет мест");
                    new_user.Shutdown(SocketShutdown.Both);
                    new_user.Close();
                }
            }
        }
        static void Main(string[] args)
        {
            Thread SendDnaThread = new Thread(SendDna);
            Thread NewVoterThread = new Thread(NewVoter);
            string ip = "127.0.0.1";
            int port = 80;
            IPEndPoint end = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(end);
            listenSocket.Listen(10);
            Console.WriteLine("Старт");
            SendDnaThread.Start();
            NewVoterThread.Start(listenSocket);
        }

    }
}
