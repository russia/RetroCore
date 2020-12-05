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
        https://google.github.io/styleguide/csharp-style.html
         */

        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            StringHelper.WriteLine("Initializing..", ConsoleColor.Blue);
            DataManager.Initialize();
            string packet = "10294|0802221747|6a292e2c446752644236725b68355d733e3a446060303e7d476e7c63354e5e42505052657756526b32585753473b64782f6f2261545d47732a5e63253242772532427e4b636c663a6e33395031705c4461386348652d527f6059365d5154703a4c39416d342962707e653b214c2e7657573c4f5542223f5d285b66423a446c5d723c2f594a6d7820787826505b253242663d463150253235304b4e41312d7f784e7d23712c655a5230335f55382046202869795f6e71415463753c4f3840492c4e";
            DataManager.GetSwfContent(packet.Split('|')[0], packet.Split('|')[1], packet.Split('|')[2]);
            PacketsReceiver.Initialize();
            StringHelper.WriteLine("Initialization done, starting..", ConsoleColor.Magenta);
            Client bot1 = new Client("msaliraso1", "sfn198622");
            //mrchriistoo77: EliasetMathieu77
            while (true)
                Thread.Sleep(500);
        }

    }
}