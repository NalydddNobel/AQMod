using AQMod.Content.World.Events.ProgressBars;
using AQMod.Content.LegacyWorldEvents.CrabSeason;
using AQMod.Content.LegacyWorldEvents.DemonSiege;
using AQMod.Tiles.Nature;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using AQMod.Content.World.Events.GlimmerEvent;

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
            var tag = new TagCompound()
            {
                ["CrabSeason_crabSeasonTimer"] = CrabSeason.crabSeasonTimer,
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            CrabSeason.crabSeasonTimer = tag.GetIntOrDefault("CrabSeason_crabSeasonTimer", CrabSeason.CrabSeasonTimerMin);

            if (tag.ContainsKey("GlimmerEvent_active"))
            {
                GlimmerEvent.tileX = (ushort)tag.GetInt("GlimmerEvent_X");
                GlimmerEvent.tileY = (ushort)tag.GetInt("GlimmerEvent_Y");
            }

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