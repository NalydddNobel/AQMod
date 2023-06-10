using Aequus.Common.Rendering;
using Aequus.Items.Potions.Pollen;
using Aequus.Tiles.CrabCrevice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Misc.Herbs {
    public class MorayTile : HerbTileBase {
        protected override int[] GrowableTiles => new int[]
        {
            TileID.Grass,
            TileID.HallowedGrass,
            TileID.Sand,
            TileID.HardenedSand,
            ModContent.TileType<SedimentaryRockTile>(),
        };

        protected override Color MapColor => new Color(186, 122, 255, 255);
        public override Vector3 GlowColor => new Vector3(0.2f, 0.05f, 0.66f);
        protected override int DrawOffsetY => -14;

        public override bool IsBlooming(int i, int j) {
            return Main.raining && !Main.dayTime;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            var clr = GlowColor;
            float multiplier = Math.Max(Main.tile[i, j].TileFrameX / 56, 0.1f);
            r = clr.X * multiplier;
            g = clr.Y * multiplier;
            b = clr.Z * multiplier;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j) {
            bool regrowth = Main.player[Player.FindClosest(new Vector2(i * 16f, j * 16f), 16, 16)].HeldItemFixed().type == ItemID.StaffofRegrowth;
            List<Item> l = new();
            if (Main.tile[i, j].TileFrameX >= FrameShiftX) {
                l.Add(new(ModContent.ItemType<MorayPollen>(), regrowth ? WorldGen.genRand.Next(1, 3) : 1));
            }
            if (CanBeHarvestedWithStaffOfRegrowth(i, j)) {
                l.Add(new(ModContent.ItemType<MoraySeeds>(), regrowth ? WorldGen.genRand.Next(1, 6) : WorldGen.genRand.Next(1, 4)));
            }
            return l;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) {
            num = 6;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            var texture = Main.instance.TilePaintSystem.TryGetTileAndRequestIfNotReady(Type, 0, Main.tile[i, j].TileColor);
            if (texture == null) {
                return true;
            }
            var effects = SpriteEffects.None;
            SetSpriteEffects(i, j, ref effects);
            var frame = new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, FrameWidth, FrameHeight);
            var offset = (Helper.TileDrawOffset - Main.screenPosition).Floor();
            var groundPosition = new Vector2(i * 16f + 8f, j * 16f + 16f).Floor();
            spriteBatch.Draw(texture, groundPosition + offset, frame, Lighting.GetColor(i, j), 0f, new Vector2(FrameWidth / 2f, FrameHeight - 2f), 1f, effects, 0f);
            spriteBatch.Draw(PaintsRenderer.TryGetPaintedTexture(i, j, AequusTextures.MorayTile_Glow.Path), groundPosition + offset, frame, Color.White, 0f, new Vector2(FrameWidth / 2f, FrameHeight - 2f), 1f, effects, 0f);
            return false;
        }

        public static bool SedimentaryWallRandomUpdate(int i, int j) {
            return TryPlaceHerb<MorayTile>(i, j, 3, TileID.Sand, TileID.HardenedSand, TileID.Sandstone, ModContent.TileType<SedimentaryRockTile>());
        }
    }
}