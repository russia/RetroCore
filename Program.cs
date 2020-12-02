using RetroCore.Helpers;
using System;
using System.Threading;

namespace RetroCore
{
    class Program
    {
        //https://docs.microsoft.com/fr-fr/dotnet/csharp/programming-guide/inside-a-program/coding-conventions
        //https://google.github.io/styleguide/csharp-style.html
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            StringHelper.WriteLine("Initializing..",ConsoleColor.Blue);
            Client Bot1 = new Client("Calypso0oPL", "pokito1234"); 

            while (true)
                Thread.Sleep(500);
        }
    }
}
