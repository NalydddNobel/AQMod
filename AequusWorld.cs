using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus
{
    public sealed class AequusWorld : ModSystem
    {
        public static bool downedSpaceSquid;
        public static bool downedRedSprite;
        public static bool downedEventGaleStreams;
        public static bool downedCrabson;
        public static bool downedOmegaStarite;

        public static int killsSpaceSquid;
        public static int killsRedSprite;

        public static bool HardmodeTier => Main.hardMode || downedOmegaStarite;

        public override void OnWorldLoad()
        {
            downedSpaceSquid = false;
            downedRedSprite = false;
            downedEventGaleStreams = false;
            downedCrabson = false;
            downedOmegaStarite = false;

            killsRedSprite = 0;
            killsSpaceSquid = 0;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["SpaceSquid"] = downedSpaceSquid;
            tag["RedSprite"] = downedRedSprite;
            tag["GaleStreams"] = downedEventGaleStreams;
            tag["Crabson"] = downedCrabson;
            tag["OmegaStarite"] = downedOmegaStarite;

            tag["SpaceSquidKills"] = killsSpaceSquid;
            tag["RedSpriteKills"] = killsRedSprite;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedSpaceSquid = tag.Get<bool>("SpaceSquid");
            downedRedSprite = tag.Get<bool>("RedSprite");
            downedEventGaleStreams = tag.Get<bool>("GaleStreams");
            downedCrabson = tag.Get<bool>("Crabson");
            downedOmegaStarite = tag.Get<bool>("OmegaStarite");

            killsSpaceSquid = tag.Get<int>("SpaceSquidKills");
            killsRedSprite = tag.Get<int>("RedSpriteKills");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(downedSpaceSquid);
            writer.Write(downedRedSprite);
            writer.Write(downedEventGaleStreams);
            writer.Write(downedCrabson);
            writer.Write(downedOmegaStarite);

            writer.Write(killsSpaceSquid);
            writer.Write(killsRedSprite);
        }

        public override void NetReceive(BinaryReader reader)
        {
            downedSpaceSquid = reader.ReadBoolean();
            downedRedSprite = reader.ReadBoolean();
            downedEventGaleStreams = reader.ReadBoolean();
            downedCrabson = reader.ReadBoolean();
            downedOmegaStarite = reader.ReadBoolean();

            killsSpaceSquid = reader.ReadInt32();
            killsRedSprite = reader.ReadInt32();
        }

        public static void DefeatEvent(ref bool defeated)
        {
            NPC.SetEventFlagCleared(ref defeated, -1);
        }
    }
}