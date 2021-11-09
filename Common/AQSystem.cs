using AQMod.Common.Skies;
using AQMod.Common.Utilities;
using AQMod.Content.WorldEvents.AzureCurrents;
using AQMod.Content.WorldEvents.CrabSeason;
using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.Tiles;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Common
{
    public class AQSystem : ModWorld
    {
        public static int NobleMushroomsCount { get; private set; }

        public override void Initialize()
        {
            OmegaStariteScene.Initialize();
            AQMod.CosmicEvent = new GlimmerEvent();
            AQMod.CosmicEvent.Init();
            AQMod.AtmosphericEvent = new AzureCurrents();
            CrabSeason.crabSeasonTimer = CrabSeason.CrabSeasonTimerMin;
            CrabSeason.CrabsonCachedID = -1;
            DemonSiege.Reset();
            if (Main.netMode != NetmodeID.Server)
                AQMod.WorldEffects.Clear();
        }

        public override TagCompound Save()
        {
            if (AQMod.CosmicEvent.deactivationTimer > 0)
                AQMod.CosmicEvent.Deactivate();
            var tag = new TagCompound()
            {
                ["GlimmerEvent_active"] = AQMod.CosmicEvent.IsActive,
                ["GlimmerEvent_X"] = (int)AQMod.CosmicEvent.tileX,
                ["GlimmerEvent_Y"] = (int)AQMod.CosmicEvent.tileY,
                ["GlimmerEvent_GlimmerChance"] = AQMod.CosmicEvent.spawnChance,

                ["CrabSeason_crabSeasonTimer"] = CrabSeason.crabSeasonTimer,

                ["EnergyDrops"] = AQNPC.NoEnergyDrops,
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            CrabSeason.crabSeasonTimer = tag.GetIntOrDefault("CrabSeason_crabSeasonTimer", CrabSeason.CrabSeasonTimerMin);

            AQNPC.NoEnergyDrops = tag.GetBool("EnergyDrops");

            AQMod.CosmicEvent.tileX = (ushort)tag.GetInt("GlimmerEvent_X");
            AQMod.CosmicEvent.tileY = (ushort)tag.GetInt("GlimmerEvent_Y");
            AQMod.CosmicEvent.spawnChance = tag.GetInt("GlimmerEvent_GlimmerChance");

            if (!Main.dayTime)
                GlimmerEventSky.InitNight();
        }

        public override void PostUpdate()
        {
            AQMod.CosmicEvent.UpdateWorld();
            CrabSeason.UpdateWorld();
            AQMod.AtmosphericEvent.UpdateWorld();
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            NobleMushroomsCount = tileCounts[ModContent.TileType<NobleMushrooms>()];
        }

        public override void NetSend(BinaryWriter writer)
        {
            if (AQMod.CosmicEvent.IsActive)
            {
                writer.Write(AQMod.CosmicEvent.tileX);
                writer.Write(AQMod.CosmicEvent.tileY);
            }
            writer.Write(AQMod.CosmicEvent.spawnChance);
        }

        public override void NetReceive(BinaryReader reader)
        {
            AQMod.CosmicEvent.tileX = reader.ReadUInt16();
            AQMod.CosmicEvent.tileY = reader.ReadUInt16();
            AQMod.CosmicEvent.spawnChance = reader.ReadInt32();
        }
    }
}