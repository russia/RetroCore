using System;

namespace RetroCore.Manager.MapManager
{
    public class IEntity
    {
        public interface IEntities : IDisposable
        {
            int Id { get; set; }
            Cell Cell { get; set; }
        }
    }
}