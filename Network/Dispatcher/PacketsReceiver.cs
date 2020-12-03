using RetroCore.Helpers;
using RetroCore.Network.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RetroCore.Network.Dispatcher
{
    internal static class PacketsReceiver
    {
        private static bool IsInitialized = false;
        private static List<PacketData> Methods = new List<PacketData>();

        public static void Initialize()
        {
            if (!IsInitialized)
            {
                Assembly assembly = typeof(FrameTemplate).GetTypeInfo().Assembly;

                foreach (var type in assembly.GetTypes().SelectMany(x => x.GetMethods()).Where(m => m.GetCustomAttributes(typeof(PacketIdAttribute), false).Length > 0).ToArray())
                {
                    PacketIdAttribute attr = (PacketIdAttribute)type.GetCustomAttributes(typeof(PacketIdAttribute), true)[0];
                    Type stringType = Type.GetType(type.DeclaringType.FullName);

                    var instance = Activator.CreateInstance(stringType, null); // instance d'une methode

                    Methods.Add(new PacketData(instance, attr.Value, type));
                }
                StringHelper.WriteLine($"{Methods.Count()} packet(s) have been registered !", ConsoleColor.Cyan);

                IsInitialized = true;
            }
        }

        public static void Receive(Client client, string packet)
        {
            if (!IsInitialized)
                Initialize();
            if (client.Network.isDisposed)
                return;

            List<PacketData> methods = Methods.FindAll(m => packet.StartsWith(m.Name)); //we should NEVER have more than 1 packet
            if (!methods.Any())
            {
                //StringHelper.WriteLine($"X Received unknowed packet -> [{packet}]", ConsoleColor.Yellow);
                return;
            }
            if (methods.Count > 1)
            {
                StringHelper.WriteLine($"X This packet is registered {methods.Count()} times ! -> [{packet}]", ConsoleColor.Red);
                return;
            }
            try
            {
                methods.First().Methode.Invoke(methods.First().Instance, new object[] { client, packet });
            }
            catch (Exception ex)
            {
                if (client.Network.isDisposed)
                    return;
                client.Network.Dispose();
            }
        }

        public static string GetPacketName(string packet)
        {
            List<PacketData> methods = Methods.FindAll(m => packet.StartsWith(m.Name)); //we should NEVER have more than 1 packet
            if (!methods.Any())            
               return "";
            
            if (methods.Count > 1)
            {
                StringHelper.WriteLine($"X This packet is registered {methods.Count()} times ! -> [{packet}]", ConsoleColor.DarkRed);
                return "";
            }
            return methods.First().Name;
        }
        public static string GetPacketContent(string packet)
        {
            List<PacketData> methods = Methods.FindAll(m => packet.StartsWith(m.Name)); //we should NEVER have more than 1 packet
            if (!methods.Any())
                return "";

            if (methods.Count > 1)
            {
                StringHelper.WriteLine($"X This packet is registered {methods.Count()} times ! -> [{packet}]", ConsoleColor.DarkRed);
                return "";
            }
            return packet.Replace(methods.First().Name, "");
        }
    }
}