using RetroCore.Manager;
using RetroCore.Network;

namespace RetroCore
{
    public class Client
    {
        public string Username { get; protected set; }
        public string Password { get; protected set; }
        public string GameTicket { get; set; }

        public SocketClient Network;

        public MapManager MapManager;

        public Client(string user, string pass)
        {
            this.Username = user;
            this.Password = pass;
            Network = new SocketClient(this);

            //todo handle next part with event
            MapManager = new MapManager(this);
        }
    }
}