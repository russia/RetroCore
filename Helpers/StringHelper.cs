using System;
using System.Linq;
using System.Text;

namespace RetroCore.Helpers
{
    public class StringHelper
    {
        
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private static Random random = new Random();

        public static void WriteLine(string msg, ConsoleColor color = ConsoleColor.White)
        {
            Console.ResetColor();
            Console.ForegroundColor = color;
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss:fff] ") + msg);
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStamp).UtcDateTime;
        }

        public static string GetRandomChar()
        {
            return chars[random.Next(0, chars.Count())].ToString();
        }

        public static string GetRandomNetworkKey()
        {
            var str1 = "";
            int rnd = random.Next(1, 128) + 128;
            int index = 0;
            while (index < rnd)
            {
                str1 = str1 + GetRandomChar();
                index = index + 1;
            }
            string result = Checksum(str1) + str1;
            return result + Checksum(result);
        }

        public static string Checksum(string content)
        {
            string hash;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                hash = BitConverter.ToString(
                  md5.ComputeHash(Encoding.UTF8.GetBytes(content))
                ).Replace("-", String.Empty);
            }
            return hash;
        }
    }
}