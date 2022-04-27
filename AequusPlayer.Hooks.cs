using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;

namespace Aequus
{
    partial class AequusPlayer
    {
        public static float? PlayerDrawScale { get; set; }

        private void LoadHooks()
        {
            On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.DrawPlayers += LegacyPlayerRenderer_DrawPlayers;
            On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_RenderAllLayers += OnRenderPlayer;
        }


        private void LegacyPlayerRenderer_DrawPlayers(On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.orig_DrawPlayers orig, LegacyPlayerRenderer self, Camera camera, IEnumerable<Player> players)
        {
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
        }

        private static void OnRenderPlayer(On.Terraria.DataStructures.PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
        {
            if (PlayerDrawScale != null)
            {
                var drawPlayer = drawinfo.drawPlayer;
                var to = new Vector2((int)drawPlayer.position.X + drawPlayer.width / 2f, (int)drawPlayer.position.Y + drawPlayer.height);
                to -= Main.screenPosition;
                for (int i = 0; i < drawinfo.DrawDataCache.Count; i++)
                {
                    DrawData data = drawinfo.DrawDataCache[i];
                    data.position -= (data.position - to) * (1f - PlayerDrawScale.Value);
                    data.scale *= PlayerDrawScale.Value;
                    drawinfo.DrawDataCache[i] = data;
                }
            }
            drawinfo.drawPlayer.GetModPlayer<AequusPlayer>().PreDraw(ref drawinfo);
            orig(ref drawinfo);
            drawinfo.drawPlayer.GetModPlayer<AequusPlayer>().PostDraw(ref drawinfo);
        }
    }
}