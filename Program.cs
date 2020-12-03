using RetroCore.Helpers;
using RetroCore.Network.Dispatcher;
using System;
using System.Threading;

namespace RetroCore
{
    internal class Program
    {
        //https://google.github.io/styleguide/csharp-style.html
        /*
        http://dofusretro.cdn.ankama.com/lang/versions_fr.txt
        http://dofusretro.cdn.ankama.com/lang/swf/NOMDUFICHIER_fr_VERSION.swf
         */

        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            StringHelper.WriteLine("Initializing..", ConsoleColor.Blue);
            PacketsReceiver.Initialize();
            StringHelper.WriteLine("Initialization done, starting..", ConsoleColor.White);
            Client bot1 = new Client("msaliraso1", "sfn198622");
            //mrchriistoo77:EliasetMathieu77
            while (true)
                Thread.Sleep(500);
        }
    }
}