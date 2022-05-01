using Aequus.Common;
using System.IO;
using Terraria;
using Terraria.ID;
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

        public static bool HardmodeTier => Main.hardMode || downedOmegaStarite;

        public override void OnWorldLoad()
        {
            downedSpaceSquid = false;
            downedRedSprite = false;
            downedEventGaleStreams = false;
            downedCrabson = false;
            downedOmegaStarite = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["SpaceSquid"] = downedSpaceSquid;
            tag["RedSprite"] = downedRedSprite;
            tag["GaleStreams"] = downedEventGaleStreams;
            tag["Crabson"] = downedCrabson;
            tag["OmegaStarite"] = downedOmegaStarite;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedSpaceSquid = tag.Get<bool>("SpaceSquid");
            downedRedSprite = tag.Get<bool>("RedSprite");
            downedEventGaleStreams = tag.Get<bool>("GaleStreams");
            downedCrabson = tag.Get<bool>("Crabson");
            downedOmegaStarite = tag.Get<bool>("OmegaStarite");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(downedSpaceSquid);
            writer.Write(downedRedSprite);
            writer.Write(downedEventGaleStreams);
            writer.Write(downedCrabson);
            writer.Write(downedOmegaStarite);
        }

        public override void NetReceive(BinaryReader reader)
        {
            downedSpaceSquid = reader.ReadBoolean();
            downedRedSprite = reader.ReadBoolean();
            downedEventGaleStreams = reader.ReadBoolean();
            downedCrabson = reader.ReadBoolean();
            downedOmegaStarite = reader.ReadBoolean();
        }

        public static void MarkAsDefeated(ref bool defeated, int npcID)
        {
            NPC.SetEventFlagCleared(ref defeated, -npcID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A bitsbyte instance where 0 is copper, 1 is iron, 2 is silver, 3 is gold. When they are false, they are the alternate world ore.</returns>
        public static BitsByte OreTiers()
        {
            if (Main.drunkWorld)
            {
                return byte.MaxValue;
            }
            return new BitsByte(
                WorldGen.SavedOreTiers.Copper == TileID.Copper,
                WorldGen.SavedOreTiers.Iron == TileID.Iron,
                WorldGen.SavedOreTiers.Silver == TileID.Silver,
                WorldGen.SavedOreTiers.Gold == TileID.Gold);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the world has cobalt, false if the world has palladium, null if the world isn't in hardmode or has neither</returns>
        public static bool? HasCobalt()
        {
            if (!Main.hardMode || (WorldGen.SavedOreTiers.Cobalt != TileID.Cobalt && WorldGen.SavedOreTiers.Cobalt != TileID.Palladium))
            {
                return null;
            }
            return WorldGen.SavedOreTiers.Cobalt == TileID.Cobalt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the world has mythril, false if the world has orichalcum, null if the world isn't in hardmode or has neither</returns>
        public static bool? HasMythril()
        {
            if (!Main.hardMode || (WorldGen.SavedOreTiers.Mythril != TileID.Mythril && WorldGen.SavedOreTiers.Mythril != TileID.Orichalcum))
            {
                return null;
            }
            return WorldGen.SavedOreTiers.Mythril == TileID.Mythril;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the world has adamantite, false if the world has titanium, null if the world isn't in hardmode or has neither</returns>
        public static bool? HasAdamantite()
        {
            if (!Main.hardMode || (WorldGen.SavedOreTiers.Adamantite != TileID.Adamantite && WorldGen.SavedOreTiers.Adamantite != TileID.Titanium))
            {
                return null;
            }
            return WorldGen.SavedOreTiers.Adamantite == TileID.Adamantite;
        }
    }
}