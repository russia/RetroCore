using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroCore.Manager.MapManager
{
    public class Entity
    {
        public interface Entities : IDisposable
        {
            int id { get; set; }
            Cell cell { get; set; }
        }
    }
}
