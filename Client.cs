using RetroCore.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroCore
{
    public class Client
    {
        public string Username { get; protected set; }
        public string Password { get; protected set; }

        public Client(string user, string pass)
        {
            this.Username = user;
            this.Password = pass;
            SocketClient socketClient = new SocketClient(this);
           
        }
    }
}
