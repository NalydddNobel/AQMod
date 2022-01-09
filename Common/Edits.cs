using AQMod.Common.Graphics.SceneLayers;
using AQMod.Effects;
using AQMod.Effects.GoreNest;
using Terraria;

namespace AQMod.Common
{
    public static class Edits
    {
        internal static void LoadHooks()
        {
            On.Terraria.Main.DrawNPCs += Main_DrawNPCs;
            On.Terraria.Main.DrawTiles += Main_DrawTiles;
        }

        internal static void UnloadHooks() // I am pretty sure TModLoader automatically unloads hooks, so this will just be used in some other cases
        {
        }

        private static void Main_DrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            if (behindTiles)
            {
                SceneLayersManager.DrawLayer(SceneLayering.BehindTiles_BehindNPCs);
            }
            else
            {
                GoreNestRenderer.RenderGoreNests();
                CustomRenderUltimateSword.RenderUltimateSword();
                CustomRenderTrapperChains.RenderTrapperChains();
                SceneLayersManager.DrawLayer(SceneLayering.BehindNPCs);
            }
            orig(self, behindTiles);
            if (behindTiles)
            {
                SceneLayersManager.DrawLayer(SceneLayering.BehindTiles_InfrontNPCs);
            }
            else
            {
                SceneLayersManager.DrawLayer(SceneLayering.InfrontNPCs);
            }
        }

        private static void Main_DrawTiles(On.Terraria.Main.orig_DrawTiles orig, Main self, bool solidOnly, int waterStyleOverride)
        {
            if (!solidOnly)
            {
                GoreNestRenderer.RefreshCoordinates();
            }
            orig(self, solidOnly, waterStyleOverride);
        }
    }
}