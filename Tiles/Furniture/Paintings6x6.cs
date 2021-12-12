using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Furniture
{
    [Obsolete("Not used anymore.")]
    public class Paintings6x6 : ModTile
    {
        public const int RockFromAnAlternateUniverse = 0;

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 6;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 18;
            TileObjectData.addTile(Type);
            dustType = 7;
            disableSmartCursor = true;
            AddMapEntry(new Color(120, 85, 60), CreateMapEntryName("Painting"));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            switch (frameX / 54)
            {
                case 0:
                Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<Items.Placeable.Furniture.RockFromAnAlternateUniverse>());
                break;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            WorldGen.KillTile(i, j);
            return base.PreDraw(i, j, spriteBatch);
        }
    }
}