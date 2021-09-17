using System;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Game Server";

            Server.Start(2, 26950);

            while (true)
            {

            }
        }
    }
}
