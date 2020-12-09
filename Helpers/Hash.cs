using RetroCore.Manager.MapManager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RetroCore.Helpers
{
    public class Hash
    {
        private static readonly char[] HashTable = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
                't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'};

        public static string GetIp(string packet)
        {
            StringBuilder ip = new StringBuilder();

            for (int i = 0; i < 8; i += 2)
            {
                int ascii1 = packet[i] - 48;
                int ascii2 = packet[i + 1] - 48;

                if (i != 0)
                    ip.Append('.');

                ip.Append(((ascii1 & 15) << 4) | (ascii2 & 15));
            }
            return ip.ToString();
        }

        public static string DecypherData(string data, string decryptKey)
        {
            try
            {
                string result = string.Empty;

                if (decryptKey == "") return data;

                decryptKey = PrepareKey(decryptKey);
                int checkSum = CheckSum(decryptKey) * 2;

                for (int i = 0, k = 0; i < data.Length; i += 2)
                    result += (char)(int.Parse(data.Substring(i, 2), NumberStyles.HexNumber) ^ decryptKey[(k++ + checkSum) % decryptKey.Length]);

                return Uri.UnescapeDataString(result);
            }
            catch
            {
                return "error";
            }
        }

        private static string PrepareKey(string key)
        {
            string keyResult = string.Empty;
            for (var i = 0; i < key.Length; i += 2)
                keyResult += Convert.ToChar(int.Parse(key.Substring(i, 2), NumberStyles.HexNumber));
            return Uri.UnescapeDataString(keyResult);
        }

        private static int CheckSum(string data) => data.Sum(t => t % 16) % 16;

        public static short GetCellIdByHash(string hash)
        {
            char char1 = hash[0], char2 = hash[1];
            short code1 = 0, code2 = 0, a = 0;

            while (a < HashTable.Length)
            {
                if (HashTable[a] == char1)
                    code1 = (short)(a * 64);

                if (HashTable[a] == char2)
                    code2 = a;

                a++;
            }
            return (short)(code1 + code2);
        }

        public static string GetHashedPath(List<Cell> Path)
        {
            Cell DestinationCell = Path.Last();

            if (Path.Count < 3)
                return DestinationCell.GetDirection(Path.First()) + Hash.GetCellChar(DestinationCell.Id);

            StringBuilder pathfinder = new StringBuilder();
            char lastDirection = Path[1].GetDirection(Path.First()), currentDirection;

            for (int i = 2; i < Path.Count; i++)
            {
                Cell actualCell = Path[i];
                Cell previousCell = Path[i - 1];
                currentDirection = actualCell.GetDirection(previousCell);

                if (lastDirection != currentDirection)
                {
                    pathfinder.Append(lastDirection);
                    pathfinder.Append(Hash.GetCellChar(previousCell.Id));

                    lastDirection = currentDirection;
                }
            }

            pathfinder.Append(lastDirection);
            pathfinder.Append(Hash.GetCellChar(DestinationCell.Id));
            return pathfinder.ToString();
        }

        public static string GetCellChar(short cellId) => HashTable[cellId / 64] + "" + HashTable[cellId % 64];

        public static int GetPort(char[] chars)
        {
            int port = 0;
            for (int i = 0; i < 2; i++)
                port += (int)(Math.Pow(64, 2 - i) * GetHash(chars[i]));
            port += GetHash(chars[2]);
            return port;
        }

        public static short GetHash(char ch)
        {
            for (short i = 0; i < HashTable.Length; i++)
                if (HashTable[i] == ch)
                    return i;

            throw new IndexOutOfRangeException(ch + " is not in the hash array.");
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