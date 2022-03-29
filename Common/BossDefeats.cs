using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common
{
    public sealed class BossDefeats : ModSystem
    {
        public static bool downedCrabson;

        public override void SaveWorldData(TagCompound tag)
        {
            tag["Crabson"] = downedCrabson;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedCrabson = tag.Get<bool>("Crabson");
        }

        public static void MarkAsDefeated(ref bool defeated)
        {
            NPC.SetEventFlagCleared(ref defeated, -1);
        }
    }
}