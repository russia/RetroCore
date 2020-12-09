using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroCore.Manager.MapManager.WorldPathFinder.Helper
{
    public class Teleporters
    {
        private static int[] TeleportersCellIds = new int[] {
        44,73,102,131,160,189,218,247,276,305,334,363,392,421, //Left
        57,86,115,144,173,202,231,260,289,318,347,376,405,434, //Right
        15,16,17,18,19,20,21,22,23,24,25,26,27,28,             //Top
        450,451,452,453,454,455,456,457,458,459,460,461,462,463//Bottom
        };
        public static List<Cell> TryGetPath()
        {
            return null;
        }
        public static List<Cell> GetMapTeleporters(Map map) => map.Cells.Where(x => x.isTeleporter() && TeleportersCellIds.Contains(x.Id)).ToList();


    }
    public class PathInfos
    {
        Map _map;
        
        public PathInfos()
        {

        }
    }
}
