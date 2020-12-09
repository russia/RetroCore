using Newtonsoft.Json;
using RetroCore.Helpers.MapsReader.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Web;

namespace RetroCore.Helpers.MapsReader
{
    public class MapKeyCracker // Awesome sources : https://github.com/hussein-aitlahcen/dofus-map-key
    {
        private const double ErrorClamp = 1.15;
        private const int CellPatternSize = 10;
        private const int MinKeyLength = 128;
        private const int MaxKeyLength = 256;
        private const int MinKeyXor = 32;
        private const int MaxKeyXor = 127;
        public static string HEX_CHARS = "0123456789ABCDEF";
        public static string StatisticsPath => Constants.OthersPath;
        private static Statistics statistics;

        public static Statistics LoadStatistics()
        {
            return JsonConvert.DeserializeAnonymousType<Statistics>(File.ReadAllText(StatisticsPath), new Statistics());
        }

        public static void Initialize()
        {
            statistics = LoadStatistics();
        }

        public static string MapCrackKey(string keyValue)
        {
            string EncodedData = HexToString(keyValue);
            int KeyLength = ComputeKeyLengthFriedman(statistics, EncodedData);
            ComputedKey Key = GuessKey(EncodedData, KeyLength, statistics);
            return FormatKeyExport(Key.Value);
        }

        public static string Checksum(string key)
        {
            int checksum = 0;
            for (int i = 0; i < key.Length; i++)
            {
                checksum += char.ConvertToUtf32(key, i) % 16;
            }
            return char.ToString(HEX_CHARS[checksum % 16]);
        }

        public static string HexToString(string data)
        {
            var output = new StringBuilder(data.Length / 2);
            for (var i = 0; i < data.Length; i += 2)
            {
                output.Append((char)int.Parse(data.Substring(i, 2), NumberStyles.HexNumber));
            }
            return output.ToString();
        }

        public static string PreEscape(string input)
        {
            var output = new StringBuilder();
            foreach (var c in input)
            {
                if (c < 32 || c > 127 || c == '%' || c == '+')
                {
                    output.Append(HttpUtility.UrlEncode(char.ToString(c)));
                }
                else
                {
                    output.Append(c);
                }
            }
            return output.ToString();
        }

        public static HashSet<int> ComputeMaxShifts(string s)
        {
            var shifts = new HashSet<int>();
            var matchPos = 0;
            var minLength = 3;
            for (int shift = 1; shift < s.Length; shift++)
            {
                int matchCount = 0;
                for (int i = 0; i < s.Length - shift; i++)
                {
                    if (s[i] == s[i + shift])
                    {
                        matchCount++;
                        if (matchCount >= minLength)
                        {
                            matchPos = i - matchCount + 1;
                            shifts.Add(shift);
                        }
                    }
                    else
                    {
                        matchCount = 0;
                    }
                }
            }
            return shifts;
        }

        public static string RightRotateShift(string key, int shift)
        {
            shift %= key.Length;
            return key.Substring(key.Length - shift) + key.Substring(0, key.Length - shift);
        }

        private static int GCD(int a, int b)
        {
            if (a >= MinKeyLength && a <= MaxKeyLength)
                return a;
            else if (b >= MinKeyLength && b <= MaxKeyLength)
                return b;
            else if (a == 0 || b == 0)
                return a | b;
            return GCD(Math.Min(a, b), Math.Max(a, b) % Math.Min(a, b));
        }

        public static double ComputeCoincidenceRate(string encodedData)
        {
            return ComputeCoincidenceRate(encodedData, 0, encodedData.Length);
        }

        public static double ComputeCoincidenceRate(string encodedData, int offset, int count)
        {
            return ComputeCoincidenceRate(GetFrequencies(encodedData, offset, count), count);
        }

        public static double ComputeCoincidenceRate(Dictionary<char, double> frequencies, int length)
        {
            return frequencies
                .Values
                .Select(value => value * length)
                .Aggregate(0.0, (acc, freq) => acc + ((freq * (freq - 1)) / (length * (length - 1))));
        }

        public static int ComputeKeyLengthFriedman(Statistics statistics, string encodedData)
        {
            var bestCoincidence = double.MinValue;
            var bestKeyLength = -1;
            for (var keyLength = MinKeyLength; keyLength <= MaxKeyLength; keyLength++)
            {
                var totalCoincidence = 0.0;
                var numberOfBlock = (int)Math.Ceiling(encodedData.Length / (double)keyLength);
                var offsetOverflow = encodedData.Length % keyLength;
                for (var blockOffset = 0; blockOffset < keyLength; blockOffset++)
                {
                    var numberOfBlockAffected = offsetOverflow > 0 ?
                          blockOffset > offsetOverflow ? numberOfBlock - 1 : numberOfBlock : numberOfBlock;
                    var cryptedBlock = new char[numberOfBlockAffected];
                    for (var blockNumber = 0; blockNumber < numberOfBlock; blockNumber++)
                    {
                        var absoluteOffset = blockNumber * keyLength + blockOffset;
                        if (absoluteOffset < encodedData.Length)
                        {
                            cryptedBlock[blockNumber] = encodedData[absoluteOffset];
                        }
                    }
                    totalCoincidence += ComputeCoincidenceRate(new string(cryptedBlock));
                }
                var coincidence = totalCoincidence / keyLength;
                if (coincidence >= bestCoincidence)
                {
                    bestCoincidence = coincidence;
                    bestKeyLength = keyLength;
                }
            }
            return bestKeyLength;
        }

        public static int ComputeHammingDistance(string message, int x, int y, int keyLength)
        {
            var distance = 0;
            for (var i = 0; i < keyLength; i++)
            {
                var cA = message[x + i];
                var cB = message[y + i];
                var xor = cA ^ cB;
                while (xor != 0)
                {
                    distance++;
                    xor &= xor - 1;
                }
            }
            return distance;
        }

        private static ComputedKey GuessKey(string message, int keyLength, Statistics statistics)
        {
            var alternatives = new Dictionary<int, List<(int, double)>>();
            var blockSize = keyLength;
            var numberOfBlock = (int)Math.Ceiling(message.Length / (double)blockSize);
            var offsetOverflow = message.Length % blockSize;
            var key = new StringBuilder();
            for (var blockOffset = 0; blockOffset < blockSize; blockOffset++)
            {
                alternatives.Add(blockOffset, new List<(int, double)>());
                var bestError = double.MaxValue;
                var bestXor = -1;
                // Only between thoses
                for (var xorKey = MinKeyXor; xorKey <= MaxKeyXor; xorKey++)
                {
                    var numberOfBlockAffected = offsetOverflow > 0 ?
                        blockOffset > offsetOverflow ? numberOfBlock - 1 : numberOfBlock : numberOfBlock;
                    var decryptedBlock = new char[numberOfBlockAffected];
                    for (var blockNumber = 0; blockNumber < numberOfBlock; blockNumber++)
                    {
                        var absoluteOffset = blockNumber * blockSize + blockOffset;
                        if (absoluteOffset < message.Length)
                        {
                            var currentData = message[absoluteOffset];
                            decryptedBlock[blockNumber] = (char)(currentData ^ xorKey);
                        }
                    }
                    var decrypted = HttpUtility.UrlDecode(new string(decryptedBlock));
                    var error = ComputeError(decrypted, blockOffset, blockSize, statistics);
                    if (error <= bestError)
                    {
                        if (error == bestError)
                        {
                            if (bestXor != -1)
                            {
                                alternatives[blockOffset].Add((xorKey, error));
                            }
                        }
                        else
                        {
                            var clampedValue = error * ErrorClamp;
                            alternatives[blockOffset].RemoveAll(x => x.Item2 > clampedValue);
                            alternatives[blockOffset].Add((xorKey, error));
                        }
                        bestError = error;
                        bestXor = xorKey;
                    }
                }
                key.Append((char)bestXor);
            }
            // Shift back the key with its checksum
            var computedKey = key.ToString();
            int shift = int.Parse(Checksum(computedKey), NumberStyles.HexNumber) * 2;
            var finalKey = RightRotateShift(computedKey, shift);
            return new ComputedKey
            {
                Value = finalKey,
                Alternatives = alternatives.Where(x => x.Value.Count > 1).ToDictionary(x => x.Key, x => x.Value)
            };
        }

        private static double ComputeError(string decrypted, int blockOffset, int blockSize, Statistics statistics)
        {
            var frequencies = GetFrequencies(decrypted);
            return GetPositionError(decrypted, blockOffset, blockSize, statistics);
        }

        public static double GetPositionError(string decrypted, int blockOffset, int blockSize, Statistics statistics)
        {
            var distance = 0.0;
            for (var i = 0; i < decrypted.Length; i++)
            {
                var absolutePosition = blockSize * i + blockOffset;
                var positionStatistics = statistics.PositionFrequencies[absolutePosition % CellPatternSize];
                var currentData = decrypted[i];
                if (positionStatistics.ContainsKey(currentData))
                {
                    distance += (1 - positionStatistics[currentData]);
                }
                else
                {
                    distance += 10;
                }
            }
            return distance;
        }

        // Squared Euclidean distance
        public static double GetDistance(Dictionary<char, double> u, Dictionary<char, double> v)
        {
            var distance = 0.0;
            foreach (var kv in u)
            {
                if (v.ContainsKey(kv.Key))
                {
                    distance += Math.Abs(kv.Value - v[kv.Key]);
                }
                else
                {
                    distance += kv.Value;
                }
            }
            foreach (var kv in v)
            {
                if (!u.ContainsKey(kv.Key))
                {
                    distance += kv.Value;
                }
            }
            return distance;
        }

        private static Dictionary<char, double> GetFrequencies(string input)
        {
            return GetFrequencies(input, 0, input.Length);
        }

        private static Dictionary<char, double> GetFrequencies(string input, int offset, int count)
        {
            var statistics = new Dictionary<char, double>();
            for (var i = offset; i < offset + count; i++)
            {
                var c = input[i];
                if (statistics.ContainsKey(c))
                    statistics[c]++;
                else
                    statistics[c] = 1;
            }
            return statistics.ToDictionary(kv => kv.Key, kv => (kv.Value / count));
        }

        public static string FormatKeyExport(string key)
        {
            return PreEscape(key).Select(c => String.Format("{0:X}", (int)c)).Aggregate("", (acc, c) => acc + c);
        }
    }
}