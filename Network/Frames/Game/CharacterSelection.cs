using RetroCore.Network.Dispatcher;
using System.Threading.Tasks;

namespace RetroCore.Network.Frames.Game
{
    public class CharacterSelection : FrameTemplate
    {
        [PacketId("HG")]
        public Task HelloGame(Client Client, string packet) => Task.Run(async () =>
        {
            //await Task.Delay(250);
            await Client.Network.SendPacket("AT" + Client.GameTicket);
        });

        [PacketId("ATK0")]
        public Task ServerSelectionResult(Client Client, string packet) => Task.Run(async () =>
        {
            await Client.Network.SendPacket("Ak0");
            await Client.Network.SendPacket("AV");
        });

        [PacketId("AV0")]
        public Task GetLanguage(Client Client, string packet) => Task.Run(async () =>
        {
            await Client.Network.SendPacket("Ages");
            await Client.Network.SendPacket("AL");
            await Client.Network.SendPacket("Af");
        });

        [PacketId("ALK")]
        public Task GetCharactersList(Client Client, string packet) => Task.Run(async () =>
        {
            //await Task.Delay(1000);
            await Client.Network.SendPacket("AS" + 240042919); //240042919 temp
            await Client.Network.SendPacket("Af"); 
        });

        [PacketId("ASK")]
        public Task GetSelectedCharacter(Client Client, string packet) => Task.Run(async () =>
        {
            await Client.Network.SendPacket("GC1");

        });
    }
}