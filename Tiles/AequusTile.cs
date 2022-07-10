using Aequus.Biomes.DemonSiege;
using Aequus.Common.Utilities;
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

        public override void RandomUpdate(int i, int j, int type)
        {
            switch (type)
            {
                case TileID.Meteorite:
                    if (AequusWorld.downedOmegaStarite && j < Main.rockLayer && WorldGen.genRand.NextBool(50))
                    {
                        TryPlaceHerb(i, j, new int[] { TileID.Meteorite, }, ModContent.TileType<MoonflowerTile>());
                    }
                    break;
            }
        }
        public static bool TryPlaceHerb(int i, int j, int[] validTile, int tile)
        {
            for (int y = j - 1; y > 20; y--)
            {
                if (!Main.tile[i, y].HasTile && Main.tile[i, y + 1].HasTile)
                {
                    for (int k = 0; k < validTile.Length; k++)
                    {
                        if (Main.tile[i, y + 1].TileType == validTile[k] && !CheckForType(new Rectangle(i - 6, y - 6, 12, 12).Fluffize(20), tile))
                        {
                            WorldGen.PlaceTile(i, y, tile, mute: true, forced: true);
                            return Main.tile[i, y].TileType == tile;
                        }
                    }
                }
            }
            return false;
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (WorldGen.gen)
                return true;

            foreach (var s in DemonSiegeSystem.ActiveSacrifices)
            {
                if (s.Value.ProtectedTiles().Contains(i, j))
                {
                    return false;
                }
            }
            return true;
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
                AequusWorld.shadowOrbsBrokenTotal++;
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
            return AequusWorld.shadowOrbsBrokenTotal < ShadowOrbDrops_Aequus ? AequusWorld.shadowOrbsBrokenTotal : WorldGen.genRand.Next(ShadowOrbDrops_Aequus);
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

        public static bool CheckForType(Rectangle rect, ArrayInterpreter<int> type)
        {
            return CheckTiles(rect, (i, j, tile) => type.Arr.ContainsAny(tile.TileType));
        }
        public static bool CheckTiles(Rectangle rect, Func<int, int, Tile, bool> function)
        {
            for (int i = rect.X; i < rect.X + rect.Width; i++)
            {
                for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                {
                    if (!function(i, j, Main.tile[i, j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}