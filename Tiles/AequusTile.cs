using Aequus.Items.Weapons.Summon.Candles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles
{
    public class AequusTile : GlobalTile
    {
        public static Action ResetTileRenderPoints;
        public static Action DrawSpecialTilePoints;

        public const int ShadowOrbDrops_Aequus = 5;

        public override void Load()
        {
            On.Terraria.GameContent.Drawing.TileDrawing.PreDrawTiles += TileDrawing_PreDrawTiles;
            On.Terraria.GameContent.Drawing.TileDrawing.DrawReverseVines += TileDrawing_DrawReverseVines;
        }

        public override void Unload()
        {
            ResetTileRenderPoints = null;
            DrawSpecialTilePoints = null;
        }

        public override bool Drop(int i, int j, int type)
        {
            if (type == TileID.ShadowOrbs && Main.tile[i, j].TileFrameX % 36 == 0 && Main.tile[i, j].TileFrameY % 36 == 0)
            {
                if (Main.tile[i, j].TileFrameX < 36)
                {
                    CorruptionOrbDrops(i, j);
                }
                else
                {
                    CrimsonOrbDrops(i, j);
                }
                AequusSystem.shadowOrbsBrokenTotal++;
            }
            return true;
        }
        public void CrimsonOrbDrops(int i, int j)
        {
            int c = OrbDrop();
            Main.NewText(c);
            switch (c)
            {
                case 1:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16f, j * 16f), 32, 32, ModContent.ItemType<CrimsonCandle>());
                    break;
            }
        }
        public void CorruptionOrbDrops(int i, int j)
        {
            int c = OrbDrop();
            Main.NewText(c);
            switch (c)
            {
                case 1:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16f, j * 16f), 32, 32, ModContent.ItemType<CorruptionCandle>());
                    break;
            }
        }
        public int OrbDrop()
        {
            return AequusSystem.shadowOrbsBrokenTotal < ShadowOrbDrops_Aequus ? AequusSystem.shadowOrbsBrokenTotal : WorldGen.genRand.Next(ShadowOrbDrops_Aequus);
        }

        private void TileDrawing_DrawReverseVines(On.Terraria.GameContent.Drawing.TileDrawing.orig_DrawReverseVines orig, Terraria.GameContent.Drawing.TileDrawing self)
        {
            orig(self);
            DrawSpecialTilePoints?.Invoke();
        }

        private void TileDrawing_PreDrawTiles(On.Terraria.GameContent.Drawing.TileDrawing.orig_PreDrawTiles orig, Terraria.GameContent.Drawing.TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
        {
            orig(self, solidLayer, forRenderTargets, intoRenderTargets);
            bool flag = intoRenderTargets || Lighting.UpdateEveryFrame;
            if (!solidLayer && flag)
            {
                ResetTileRenderPoints?.Invoke();
            }
        }
    }
}