using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Effects
{
    partial class EffectsSystem : ModSystem
    {
        private void Hook_OnDrawProjs(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            BehindProjs.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
            orig(self);
        }

        internal static void Hook_OnDrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            try
            {
                NPCsBehindAllNPCs.renderingNow = true;
                for (int i = 0; i < NPCsBehindAllNPCs.Count; i++)
                {
                    Main.instance.DrawNPC(NPCsBehindAllNPCs.Index(i), behindTiles);
                }
                NPCsBehindAllNPCs.Clear();
            }
            catch
            {
                NPCsBehindAllNPCs?.Clear();
                NPCsBehindAllNPCs = new DrawIndexCache();
            }
            //if (!behindTiles)
            //{
            //    GoreNestRenderer.Render();
            //    UltimateSwordRenderer.Render();
            //}
            orig(self, behindTiles);
            if (behindTiles)
            {
                try
                {
                    ProjsBehindTiles.renderingNow = true;
                    if (ProjsBehindTiles != null)
                    {
                        for (int i = 0; i < ProjsBehindTiles.Count; i++)
                        {
                            Main.instance.DrawProj(ProjsBehindTiles.Index(i));
                        }
                    }
                    ProjsBehindTiles.Clear();
                }
                catch
                {
                    ProjsBehindTiles?.Clear();
                    ProjsBehindTiles = new DrawIndexCache();
                }
            }
        }
    }
}