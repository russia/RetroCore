using System.Reflection;

namespace RetroCore.Network.Dispatcher
{
    public class PacketData
    {
        public object Instance;
        public string Name;
        public MethodInfo Methode;

        public PacketData(object instance, string name, MethodInfo method)
        {
            this.Instance = instance;
            this.Name = name;
            this.Methode = method;
        }
    }
}