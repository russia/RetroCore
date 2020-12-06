using System;

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