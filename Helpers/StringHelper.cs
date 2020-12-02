using System;
using System.Text;

namespace RetroCore.Helpers
{
    public class StringHelper
    {
        private static readonly char[] HashTable = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
                't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'};

        public static void WriteLine(string msg, ConsoleColor color = ConsoleColor.White)
        {
            Console.ResetColor();
            Console.ForegroundColor = color;
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss:fff] ") + msg);
        }

        public static string Encrypt(string password, string key)
        {
            StringBuilder str = new StringBuilder().Append("#1");
            for (int i = 0; i < password.Length; i++)
            {
                str.Append(HashTable[(password[i] / 16 + key[i]) % HashTable.Length]).Append(HashTable[(password[i] % 16 + key[i]) % HashTable.Length]);
            }
            return str.ToString();
        }
    }
}