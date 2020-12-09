using RetroCore.Helpers;
using RetroCore.Interfaces;
using RetroCore.Manager.MapManager;
using RetroCore.Manager.MapManager.PathFinder;
using RetroCore.Manager.MapManager.WorldPathFinder;
using RetroCore.Network;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetroCore
{
    public class Client : IClearable, IDisposable
    {
        public string Username { get; protected set; }
        public string Password { get; protected set; }
        public string GameTicket { get; set; }

        public int CharacterId { get; set; }
        public SocketClient Network;

        #region States

        public bool IsFighting { get; set; } = false;

        #endregion States

        #region Managers

        public Map MapManager;
        public PathFinder PathFinderManager;
        public WorldPathFinder WorldPathFinderManager;

        #endregion Managers

        public Client(string user, string pass)
        {
            this.Username = user;
            this.Password = pass;
            Network = new SocketClient(this);
            MapManager = new Map(this);
            PathFinderManager = new PathFinder(MapManager);
            WorldPathFinderManager = new WorldPathFinder(this);
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

        public Task OnCharacterConnectionFinished() => Task.Run(async () => //Used to debug
        {
            await Task.Delay(3500); //todo find an other packet
            //WorldPathFinderManager.GetPath(MapManager.XCoord, MapManager.YCoord);
            WorldPathFinderManager.GetPath(MapManager.XCoord, MapManager.YCoord);
            //var path = PathFinderManager.GetPath(MapManager.Cells.Where(x => x.IsTeleporter() == true).OrderBy(x => Guid.NewGuid()).First().Id);
            ////var path = PathFinderManager.GetPath(26);

            //foreach (var cell in path)
            //{
            //    Console.Write(cell.Id + " -> ");
            //}

            //string packetContent = "GA001" + Hash.getHashedPath(path);
            //await this.Network.SendPacket(packetContent);
        });

        #endregion Events

        public void Reconnect() => Network.Connection(Constants.AuthAddress, Constants.AuthPort); // add dispose in it ?

        public void Dispose()
        {
            Network.Dispose();
            Clear();
        }

        public void Clear()
        {
            Network = new SocketClient(this);
            MapManager = new Map(this);
            PathFinderManager = new PathFinder(MapManager);
            WorldPathFinderManager = new WorldPathFinder(this);
        }
    }
}