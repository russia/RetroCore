using RetroCore.Manager.MapManager;
using RetroCore.Manager.MapManager.PathFinder;
using RetroCore.Network;
using System;
using System.Threading.Tasks;

namespace RetroCore
{
    public class Client
    {
        public string Username { get; protected set; }
        public string Password { get; protected set; }
        public string GameTicket { get; set; }

        public int CharacterId { get; set; }
        public SocketClient Network;

        #region States

        public bool isFighting { get; set; } = false;

        #endregion States

        #region Managers

        public Map MapManager;
        public PathFinder PathFinderManager;

        #endregion Managers

        public Client(string user, string pass)
        {
            this.Username = user;
            this.Password = pass;
            Network = new SocketClient(this);
            MapManager = new Map(this);
            PathFinderManager = new PathFinder(this);
        }

        #region Events

        public Task OnCharacterConnectionFinished() => Task.Run(async () =>
        {
            await Task.Delay(800); //todo find an other packet
            var path = PathFinderManager.GetPath(433);
            path.Reverse();
            foreach (var cell in path)
                Console.Write(cell.Id + " ->");
        });

        #endregion Events
    }
}