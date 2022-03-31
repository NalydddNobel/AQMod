using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common
{
    public sealed class AequusDefeats : ModSystem
    {
        public static bool downedCrabson;
        public static bool downedOmegaStarite;
        public static bool downedEventGaleStreams;

        public static bool HardmodeTier => Main.hardMode || downedOmegaStarite;

        public override void OnWorldLoad()
        {
            downedCrabson = false;
            downedOmegaStarite = false;
            downedEventGaleStreams = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["Crabson"] = downedCrabson;
            tag["OmegaStarite"] = downedOmegaStarite;
            tag["GaleStreams"] = downedEventGaleStreams;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedCrabson = tag.Get<bool>("Crabson");
            downedOmegaStarite = tag.Get<bool>("OmegaStarite");
            downedEventGaleStreams = tag.Get<bool>("GaleStreams");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(downedCrabson);
            writer.Write(downedOmegaStarite);
            writer.Write(downedEventGaleStreams);
        }

        public override void NetReceive(BinaryReader reader)
        {
            downedCrabson = reader.ReadBoolean();
            downedOmegaStarite = reader.ReadBoolean();
            downedEventGaleStreams = reader.ReadBoolean();
        }

        public static void MarkAsDefeated(ref bool defeated)
        {
            NPC.SetEventFlagCleared(ref defeated, -1);
        }
    }
}