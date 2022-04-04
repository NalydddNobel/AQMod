﻿using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common
{
    public sealed class WorldFlags : ModSystem
    {
        public static bool downedSpaceSquid;
        public static bool downedRedSprite;
        public static bool downedEventGaleStreams;
        public static bool downedCrabson;
        public static bool downedOmegaStarite;

        public static bool HardmodeTier => Main.hardMode || downedOmegaStarite;

        public override void OnWorldLoad()
        {
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

        public static void MarkAsDefeated(ref bool defeated)
        {
            NPC.SetEventFlagCleared(ref defeated, -1);
        }
    }
}