using System;

namespace RetroCore.Network.Dispatcher
{
    internal class PacketIdAttribute : Attribute
    {
        public string Value;

        public PacketIdAttribute(string v)
        {
            this.Value = v;
        }
    }
}