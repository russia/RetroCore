using RetroCore.Helpers;
using RetroCore.Manager.MapManager;
using RetroCore.Manager.MapManager.PathFinder;
using RetroCore.Network;
using System;
using System.Linq;
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

        public async Task AwaitMovementEnds(Cell destination, byte gkk)
        {
            await Task.Delay(PathTiming.GetMovementTiming(MapManager.CurrentCell, MapManager.ActualPath, false));

            await this.Network.SendPacket("GKK" + gkk);
            MapManager.CurrentCell = destination;

            MapManager.ActualPath = null;
            OnCharacterConnectionFinished();
        }

        public Task OnCharacterConnectionFinished() => Task.Run(async () =>
        {
            await Task.Delay(800); //todo find an other packet
            var path = PathFinderManager.GetPath(MapManager.Cells.Where(x => x.is_Teleporter() == true).OrderBy(x => Guid.NewGuid()).First().Id);
       
            foreach (var cell in path)
            {
                Console.Write(cell.Id + " -> ");
            }

            string packetContent = "GA001" + Hash.getHashedPath(path);
            await this.Network.SendPacket(packetContent);
        });

        #endregion Events
    }
}