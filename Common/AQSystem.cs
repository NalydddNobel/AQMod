using AQMod.Common.Utilities;
using AQMod.Content.Skies;
using AQMod.Content.WorldEvents;
using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.NPCs.Town.Robster;
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
            AQMod.omegaStariteIndexCache = -1;
            AQMod.omegaStariteScene = 0;
            GlimmerEvent.ActuallyActive = false;
            GlimmerEvent.FakeActive = false;
            GlimmerEvent.X = 0;
            GlimmerEvent.Y = 0;
            GlimmerEvent.GlimmerChance = GlimmerEvent.GlimmerChanceMax;
            GlimmerEvent.DeactivationTimer = -1;
            CrabSeason.crabSeasonTimer = CrabSeason.CrabSeasonTimerMin;
            CrabSeason.CrabsonCachedID = -1;
            DemonSiege.Reset();
            DrawUtils.WorldEffects.Clear();
        }

        public override TagCompound Save()
        {
            if (GlimmerEvent.DeactivationTimer > 0)
                GlimmerEvent.Deactivate();
            var tag = new TagCompound()
            {
                ["GlimmerEvent_active"] = GlimmerEvent.ActuallyActive,
                ["GlimmerEvent_X"] = (int)GlimmerEvent.X,
                ["GlimmerEvent_Y"] = (int)GlimmerEvent.Y,
                ["GlimmerEvent_GlimmerChance"] = GlimmerEvent.GlimmerChance,

                ["CrabSeason_crabSeasonTimer"] = CrabSeason.crabSeasonTimer,

                ["EnergyDrops"] = AQNPC.NoEnergyDrops,
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            CrabSeason.crabSeasonTimer = tag.GetIntOrDefault("CrabSeason_crabSeasonTimer", CrabSeason.CrabSeasonTimerMin);

            AQNPC.NoEnergyDrops = tag.GetBool("EnergyDrops");

            GlimmerEvent.ActuallyActive = tag.GetBool("GlimmerEvent_active");
            if (GlimmerEvent.ActuallyActive)
            {
                GlimmerEvent.X = (ushort)tag.GetInt("GlimmerEvent_X");
                GlimmerEvent.Y = (ushort)tag.GetInt("GlimmerEvent_Y");
            }
            GlimmerEvent.GlimmerChance = tag.GetInt("GlimmerEvent_GlimmerChance");

            if (!Main.dayTime)
                GlimmerEventSky.InitNight();
        }

        public override void PostUpdate()
        {
            AQNPC.UpdateBossRush();
            GlimmerEvent.UpdateWorld();
            CrabSeason.UpdateWorld();
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            NobleMushroomsCount = tileCounts[ModContent.TileType<NobleMushrooms>()];
        }

        public override void PostDrawTiles()
        {
            Parralax.RefreshParralax();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(GlimmerEvent.ActuallyActive);
            writer.Write(GlimmerEvent.FakeActive);
            if (GlimmerEvent.ActuallyActive)
            {
                writer.Write(GlimmerEvent.X);
                writer.Write(GlimmerEvent.Y);
            }
            writer.Write(GlimmerEvent.GlimmerChance);
        }

        public override void NetReceive(BinaryReader reader)
        {
            GlimmerEvent.ActuallyActive = reader.ReadBoolean();
            GlimmerEvent.FakeActive = reader.ReadBoolean();
            if (GlimmerEvent.ActuallyActive)
            {
                GlimmerEvent.X = reader.ReadUInt16();
                GlimmerEvent.Y = reader.ReadUInt16();
            }
            else
            {
                GlimmerEvent.X = 0;
                GlimmerEvent.Y = 0;
            }
            GlimmerEvent.GlimmerChance = reader.ReadInt32();
        }
    }
}