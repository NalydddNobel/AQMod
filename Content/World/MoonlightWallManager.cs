using AQMod.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.World
{
    /// <summary>
    /// Helps manage things that interact with the Moonlight Wall
    /// </summary>
    public static class MoonlightWallManager
    {
        private static bool _dayTimeCache;
        public static bool CurrentlyCachingDaytimeFlag { get; private set; }

        public static bool BehindMoonlightWall(Vector2 center)
        {
            return BehindMoonlightWall((int)center.X / 16, (int)center.Y / 16);
        }

        public static bool BehindMoonlightWall(int x, int y)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY)
            {
                return false;
            }
            return x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY
                ? false
                : Framing.GetTileSafely(x, y).wall == ModContent.WallType<MoonlightWallWall>();
        }

        public static void Begin()
        {
            CurrentlyCachingDaytimeFlag = true;
            _dayTimeCache = Main.dayTime;
            Main.dayTime = false;
        }

        public static void End()
        {
            CurrentlyCachingDaytimeFlag = false;
            Main.dayTime = _dayTimeCache;
        }
    }
}