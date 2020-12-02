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
        public SocketClient Network;
        public Client(string user, string pass)
        {
            this.Username = user;
            this.Password = pass;
            Network = new SocketClient(this);
           
        }
    }
}
