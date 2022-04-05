using Terraria;
using Terraria.ID;

namespace Aequus
{
    partial class AequusHelpers
    {
        public static void SyncNPC(NPC npc)
        {
            NetMessage.SendData(MessageID.SyncNPC, Main.myPlayer, -1, null, npc.whoAmI);
        }
    }
}