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
            int total_pings = 0, contador = 0;
            while (contador < Pings.Count)
            {
                total_pings += Pings[contador];
                contador++;
            }
            return total_pings / Pings.Count;
        }
    }
}