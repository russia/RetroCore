using RetroCore.Helpers;
using RetroCore.Helpers.MapsReader;
using RetroCore.Manager.MapManager.WorldPathFinder;
using RetroCore.Network.Dispatcher;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RetroCore
{
    internal class Program
    {
        /*
        https://google.github.io/styleguide/csharp-style.html
        https://cdn.discordapp.com/attachments/309032714982522881/752115574808510564/Diagramme_des_packets_Dofus.png
        https://funky-emu.net/uploads/monthly_2011_11/59fb3340d8092_MAPIDCELL.jpg.218d7f76b1bd94e4b499244d335c961f.jpg
         */
        public static bool Debug;

        private static void Main(string[] args)
        {
           
            Debug = Debugger.IsAttached;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            StringHelper.WriteLine("Initializing..", ConsoleColor.Blue);
            DataManager.Initialize();
            MapKeyCracker.Initialize();
            PacketsReceiver.Initialize();
            StringHelper.WriteLine("Initialization done, starting..", ConsoleColor.Gray);

            Client bot1 = null;
            var task = Task.Factory.StartNew(() => { bot1 = new Client("msaliraso1", "sfn198622"); });
            //var task2 = Task.Factory.StartNew(async () => { Client bot1 = new Client("mrchriistoo77", "EliasetMathieu77"); });
            //mrchriistoo77: EliasetMathieu77
            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}