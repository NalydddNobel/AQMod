using AQMod.Common.Skies;
using AQMod.Content.WorldEvents.CrabSeason;
using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.Content.WorldEvents.ProgressBars;
using AQMod.Tiles.Nature;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod
{
    public class AQSystem : ModWorld
    {
        public static int NobleMushroomsCount { get; private set; }

        public override void Initialize()
        {
            OmegaStariteScenes.Initialize();
            CrabSeason.crabSeasonTimer = CrabSeason.CrabSeasonTimerMin;
            CrabSeason.CrabsonCachedID = -1;
            DemonSiege.Reset();
            if (Main.netMode != NetmodeID.Server)
                AQMod.WorldEffects.Clear();
            EventProgressBarLoader.ActiveBar = 255;
        }

        public override TagCompound Save()
        {
            if (GlimmerEvent.deactivationTimer > 0)
                GlimmerEvent.Deactivate();
            var tag = new TagCompound()
            {
                ["GlimmerEvent_active"] = GlimmerEvent.IsActive,
                ["GlimmerEvent_X"] = (int)GlimmerEvent.tileX,
                ["GlimmerEvent_Y"] = (int)GlimmerEvent.tileY,
                ["GlimmerEvent_GlimmerChance"] = GlimmerEvent.spawnChance,

                ["CrabSeason_crabSeasonTimer"] = CrabSeason.crabSeasonTimer,
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            CrabSeason.crabSeasonTimer = tag.GetIntOrDefault("CrabSeason_crabSeasonTimer", CrabSeason.CrabSeasonTimerMin);

            GlimmerEvent.tileX = (ushort)tag.GetInt("GlimmerEvent_X");
            GlimmerEvent.tileY = (ushort)tag.GetInt("GlimmerEvent_Y");
            GlimmerEvent.spawnChance = tag.GetInt("GlimmerEvent_GlimmerChance");

            if (!Main.dayTime)
                GlimmerEventSky.InitNight();
        }

        public override void PostUpdate()
        {
            CrabSeason.UpdateWorld();
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            NobleMushroomsCount = tileCounts[ModContent.TileType<NobleMushrooms>()];
        }
    }
}