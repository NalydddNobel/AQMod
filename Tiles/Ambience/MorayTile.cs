using Aequus.Items.Misc;
using Aequus.Items.Placeable.Seeds;
using Aequus.Tiles.CrabCrevice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Ambience
{
    public class MorayTile : HerbTileBase
    {
        protected override int[] GrowableTiles => new int[]
        {
            TileID.Grass,
            TileID.HallowedGrass,
            ModContent.TileType<SedimentaryRockTile>(),
        };

        protected override Color MapColor => new Color(186, 122, 255, 255);
        public override Vector3 GlowColor => new Vector3(0.2f, 0.05f, 0.66f);
        protected override int DrawOffsetY => -14;

        public override bool IsBlooming(int i, int j)
        {
            return !Main.WindyEnoughForKiteDrops;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            var clr = GlowColor;
            float multiplier = Math.Max(Main.tile[i, j].TileFrameX / 56, 0.1f);
            r = clr.X * multiplier;
            g = clr.Y * multiplier;
            b = clr.Z * multiplier;
        }

        public override bool Drop(int i, int j)
        {
            if (Main.tile[i, j].TileFrameX >= FrameShiftX)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<MorayPollen>(), Main.rand.Next(3) + 1);
            }
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 6;
        }
    }
}