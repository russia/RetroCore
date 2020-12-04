using RetroCore.Helpers;
using RetroCore.Helpers.MapsReader;
using RetroCore.Network.Dispatcher;
using RetroCore.Others;
using System;
using System.Threading;

namespace RetroCore
{
    internal class Program
    {
        /*
        http://dofusretro.cdn.ankama.com/lang/versions_fr.txt
        http://dofusretro.cdn.ankama.com/lang/swf/NOMDUFICHIER_fr_VERSION.swf
        https://google.github.io/styleguide/csharp-style.html
         */

        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            StringHelper.WriteLine("Initializing..", ConsoleColor.Blue);
            DataManager.Initialize();
         //   PacketsReceiver.Initialize();
         //   StringHelper.WriteLine("Initialization done, starting..", ConsoleColor.White);
         //   Client bot1 = new Client("msaliraso1", "sfn198622");
         //   //mrchriistoo77:EliasetMathieu77
         //   while (true)
         //       Thread.Sleep(500);
        }
      
    
    }
}