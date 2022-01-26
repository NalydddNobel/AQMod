using AQMod.Content.World.Events;
using AQMod.Content.World.Events.DemonSiege;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Tiles.Nature.CrabCrevice;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod
{
    public class AQSystem : ModWorld
    {
        public static int NobleMushroomsCount { get; private set; }

        public static int DayrateIncrease { get; set; }

        public static bool UpdatingTime { get; internal set; }
        public static bool CosmicanonActive { get; internal set; }

        public override void Initialize()
        {
            OmegaStariteScenes.Initialize();
            DemonSiege.Reset();
            if (Main.netMode != NetmodeID.Server)
                AQMod.WorldEffects.Clear();
            if (!Main.dayTime)
                GlimmerEventSky.InitNight();
            EventProgressBarLoader.ActiveBar = 255;
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            NobleMushroomsCount = tileCounts[ModContent.TileType<NobleMushrooms>()] + tileCounts[ModContent.TileType<NobleMushroomsNew>()];
        }
    }
}