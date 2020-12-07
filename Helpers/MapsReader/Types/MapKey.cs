using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroCore.Helpers.MapsReader.Types
{
    public class Map
    {
        public int Id;
        public int Width;
        public int Height;
        public string Key;
        public string DecodedData;
        public string EncodedData;
    }

    public class Statistics
    {
        public double IndexOfCoincidence;
        public Dictionary<char, double> GlobalFrequencies;
        public Dictionary<char, double>[] PositionFrequencies;
    }

    public class ComputedKey
    {
        public string Value;
        public Dictionary<int, List<(int, double)>> Alternatives;
    }
}
