using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroCore.Manager
{
    public class MapManager
    {
        public Client _client;

        public MapManager(Client client)
        {
            this._client = client;

        }

        #region updates

        public void UpdateMap(string packet)
        {
            string[] splitted = packet.Split('|');
            short id = short.Parse(splitted[0]);
        }

        #endregion

    }
}
