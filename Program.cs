using RetroCore.Helpers;
using RetroCore.Network.Dispatcher;
using System;
using System.Threading;

namespace RetroCore
{
    class Program
    {
        //https://google.github.io/styleguide/csharp-style.html
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            StringHelper.WriteLine("Initializing..",ConsoleColor.Blue);
            PacketsReceiver.Initialize();
            StringHelper.WriteLine("Initialization done, starting..", ConsoleColor.White);
            Client bot1 = new Client("benjamindelemer", "abercrombie1"); 

            while (true)
                Thread.Sleep(500);
        }
    }
}
