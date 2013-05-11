using System;
using System.Collections.Generic;
using System.Text;

namespace AsynClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("以哪种方式异步通讯？");
            Console.WriteLine("A.Tcp");
            Console.WriteLine("B.Udp");
            ConsoleKey key;
            while (true)
            {
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.A)
                {
                    AsynTcpClient asynClient = new AsynTcpClient();
                    asynClient.AsynConnect();
                }
                else if (key == ConsoleKey.B)
                {
                    AsynUdpClient asynClient = new AsynUdpClient();
                    asynClient.InitClient();
                }
                else
                {
                    Console.WriteLine("输入有误,请重新输入：");
                    continue;
                }
                break;
            }
            Console.ReadKey();
        }
    }
}
