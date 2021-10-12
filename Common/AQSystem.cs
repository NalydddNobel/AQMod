using AQMod.Common.Skies;
using AQMod.Common.Utilities;
using AQMod.Content;
using AQMod.Content.WorldEvents;
using AQMod.Content.WorldEvents.Siege;
using AQMod.Tiles;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Common
{
    public class AQSystem : ModWorld
    {
        public static int NobleMushroomsCount { get; private set; }

        public override void Initialize()
        {
            OmegaStariteSceneManager.Initialize();
            AQMod.glimmerEvent.Init();
            CrabSeason.crabSeasonTimer = CrabSeason.CrabSeasonTimerMin;
            CrabSeason.CrabsonCachedID = -1;
            DemonSiege.Reset();
            AQMod.WorldEffects.Clear();
        }

        public override TagCompound Save()
        {
            if (AQMod.glimmerEvent.deactivationTimer > 0)
                AQMod.glimmerEvent.Deactivate();
            var tag = new TagCompound()
            {
                ["GlimmerEvent_active"] = AQMod.glimmerEvent.IsActive,
                ["GlimmerEvent_X"] = (int)AQMod.glimmerEvent.tileX,
                ["GlimmerEvent_Y"] = (int)AQMod.glimmerEvent.tileY,
                ["GlimmerEvent_GlimmerChance"] = AQMod.glimmerEvent.spawnChance,

                ["CrabSeason_crabSeasonTimer"] = CrabSeason.crabSeasonTimer,

                ["EnergyDrops"] = AQNPC.NoEnergyDrops,
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            CrabSeason.crabSeasonTimer = tag.GetIntOrDefault("CrabSeason_crabSeasonTimer", CrabSeason.CrabSeasonTimerMin);

            AQNPC.NoEnergyDrops = tag.GetBool("EnergyDrops");

            AQMod.glimmerEvent.tileX = (ushort)tag.GetInt("GlimmerEvent_X");
            AQMod.glimmerEvent.tileY = (ushort)tag.GetInt("GlimmerEvent_Y");
            AQMod.glimmerEvent.spawnChance = tag.GetInt("GlimmerEvent_GlimmerChance");

            if (!Main.dayTime)
                GlimmerEventSky.InitNight();
        }

        public override void PostUpdate()
        {
            AQNPC.UpdateBossRush();
            AQMod.glimmerEvent.UpdateWorld();
            CrabSeason.UpdateWorld();
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            NobleMushroomsCount = tileCounts[ModContent.TileType<NobleMushrooms>()];
        }

        public override void NetSend(BinaryWriter writer)
        {
            if (AQMod.glimmerEvent.IsActive)
            {
                writer.Write(AQMod.glimmerEvent.tileX);
                writer.Write(AQMod.glimmerEvent.tileY);
            }
            writer.Write(AQMod.glimmerEvent.spawnChance);
        }

        public override void NetReceive(BinaryReader reader)
        {
            AQMod.glimmerEvent.tileX = reader.ReadUInt16();
            AQMod.glimmerEvent.tileY = reader.ReadUInt16();
            AQMod.glimmerEvent.spawnChance = reader.ReadInt32();
        }
    }
}