using AQMod.Items.Placeable.Furniture;
using AQMod.NPCs.Friendly;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.ExporterQuest
{
    public class JeweledCandelabraTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.addTile(Type);
            AddMapEntry(Robster.JeweledTileMapColor, AQMod.GetTranslation("ItemName.JeweledCandelabra"));
            soundStyle = SoundID.Dig;
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Chandeliers };
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameX == 18 && tile.frameY % 54 == 18)
            {
                r = 1f;
                g = 0.7f;
                b = 0f;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<JeweledCandelabra>());
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].frameX == 18 && Main.tile[i, j].frameY % 36 >= 18)
            {
                ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
                var drawCoordinates = new Vector2((i - 1) * 16f, (j - 1) * 16f) + AQMod.Zero - Main.screenPosition;
                var texture = ModContent.GetTexture(this.GetPath("_Flame"));
                for (int k = 0; k < 7; k++)
                {
                    Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(Utils.RandomInt(ref randSeed, -10, 11) * 0.15f, Utils.RandomInt(ref randSeed, -10, 1) * 0.35f),
                        new Rectangle(Main.tile[i, j].frameX - 18, Main.tile[i, j].frameY - 18, 36, 36), new Color(100, 100, 100, 0), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}