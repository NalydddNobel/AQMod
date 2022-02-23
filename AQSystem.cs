using AQMod.Content.World.Events;
using AQMod.Effects;
using AQMod.Tiles.Nature.CrabCrevice;
using Terraria;
using Terraria.ModLoader;

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
            if (!Main.dayTime)
                SkyGlimmerEvent.InitNight();
            EventProgressBarLoader.ActiveBar = 255;
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            NobleMushroomsCount = tileCounts[ModContent.TileType<NobleMushrooms>()] + tileCounts[ModContent.TileType<NobleMushroomsNew>()];
        }
    }
}