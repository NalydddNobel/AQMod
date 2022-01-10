using System.Collections.Generic;
using Terraria;

namespace AQMod.Effects
{
    internal static class CustomRenderBehindTiles
    {
        public static bool DrawingNow { get; private set; }
        public static List<int> DrawProjsCache;

        public static void Render()
        {
            DrawingNow = true;
            try
            {
                if (DrawProjsCache != null)
                {
                    for (int i = 0; i < DrawProjsCache.Count; i++)
                    {
                        Main.instance.DrawProj(DrawProjsCache[i]);
                    }
                }
            }
            catch
            {
            }
            DrawingNow = false;
            DrawProjsCache = new List<int>();
        }
    }
}