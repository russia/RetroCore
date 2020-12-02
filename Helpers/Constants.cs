using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RetroCore.Helpers
{
    public class Constants
    {
        public static IPAddress AuthAddress = IPAddress.Parse("172.65.206.193"); //  is there 2 differents auth serv ??
        public static int AuthPort = 5555; // there is also a 443 port
        public static string GameVersion = "1.33.6";

    }
}
