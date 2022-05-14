using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    partial class EffectsSystem : ModSystem
    {
        private static void LegacyPlayerRenderer_DrawPlayers(On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.orig_DrawPlayers orig, LegacyPlayerRenderer self, Camera camera, IEnumerable<Player> players)
        {
            CommonSpriteBatchBegins.GeneralEntities.Begin(Main.spriteBatch);
            BehindPlayers.Draw(Main.spriteBatch);
            Main.spriteBatch.End();

            var aequusPlayers = new List<AequusPlayer>();
            foreach (var p in players)
            {
                aequusPlayers.Add(p.GetModPlayer<AequusPlayer>());
            }
            foreach (var aequus in aequusPlayers)
            {
                aequus.PreDrawAllPlayers(self, camera, players);
            }
            orig(self, camera, players);
            //foreach (var p in active)
            //{
            //    p.PostDrawAllPlayers(self);
            //}
            CommonSpriteBatchBegins.GeneralEntities.Begin(Main.spriteBatch);
            AbovePlayers.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
        }

        private static void Main_DoDraw_UpdateCameraPosition(On.Terraria.Main.orig_DoDraw_UpdateCameraPosition orig)
        {
            orig();
            for (int i = 0; i < necromancyRenderers.Length; i++)
            {
                if (necromancyRenderers[i] != null)
                {
                    necromancyRenderers[i].Request();
                    necromancyRenderers[i].PrepareRenderTarget(Main.instance.GraphicsDevice, Main.spriteBatch);
                }
            }
        }

        private static void Hook_OnDrawDust(On.Terraria.Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            try
            {
                for (int i = 0; i < necromancyRenderers.Length; i++)
                {
                    if (necromancyRenderers[i] != null && necromancyRenderers[i].IsReady)
                    {
                        necromancyRenderers[i].DrawOntoScreen(Main.spriteBatch);
                    }
                }
            }
            catch
            {

            }
        }

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