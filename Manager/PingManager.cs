using System;
using System.Collections.Generic;
using System.Linq;

namespace RetroCore.Manager
{
    public class PingManager
    {
        public List<int> Pings;
        public int Ticks;

        public PingManager()
        {
            Pings = new List<int>();
            Ticks = 0;
        }

        public int GetTotalPings() => Pings.Count();

        public int GetActualPing() => Environment.TickCount - Ticks;

        public int GetAveragePing()
        {
            int total_pings = 0, count = 0;
            while (count < Pings.Count)
            {
                total_pings += Pings[count];
                count++;
            }
            return total_pings / Pings.Count;
        }

        public void Update()
        {
            Pings.Add(GetActualPing());
            if (Pings.Count > 48)
                Pings.Clear();
        }
    }
}