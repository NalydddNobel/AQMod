using AQMod.Items.Placeable.Furniture;
using AQMod.NPCs.Friendly;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Furniture
{
    public sealed class JeweledChandlierTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[3] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);
            AddMapEntry(Robster.JeweledTileMapColor, AQMod.GetTranslation("ItemName.JeweledChandelier"));
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            disableSmartCursor = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameX == 18 && (tile.frameY % 54) == 18)
            {
                r = 1f;
                g = 0.7f;
                b = 0f;
            }
        }

        //public override void HitWire(int i, int j)
        //{
        //    if (Main.tile[i, j].frameY < 54)
        //    {
        //        return;
        //    }
        //    int x = i - Main.tile[i, j].frameX / 18 % 3;
        //    int y = j - Main.tile[i, j].frameY / 18 % 3;
        //    for (int m = x; m < x + 3; m++)
        //    {
        //        for (int n = y; n < y + 3; n++)
        //        {
        //            if (Main.tile[m, n] == null)
        //            {
        //                Main.tile[m, n] = new Tile();
        //            }
        //            if (Main.tile[m, n].active() && Main.tile[m, n].type == Type)
        //            {
        //                if (Main.tile[m, n].frameX < 54)
        //                {
        //                    Main.tile[m, n].frameX += 54;
        //                }
        //                else
        //                {
        //                    Main.tile[m, n].frameX -= 54;
        //                }
        //            }
        //        }
        //    }
        //    NetMessage.SendTileSquare(-1, x, y, 3, TileChangeType.None);
        //    if (!Wiring.running)
        //    {
        //        return;
        //    }
        //    for (int k = 0; k < 3; k++)
        //    {
        //        for (int l = 0; l < 3; l++)
        //        {
        //            Wiring.SkipWire(x + k, y + l);
        //        }
        //    }
        //}

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<JeweledChandelier>());
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].frameX == 36 && (Main.tile[i, j].frameY % 54) >= 36)
            {
                ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
                var drawCoordinates = new Vector2((i - 2) * 16f, (j - 2) * 16f) + AQMod.Zero - Main.screenPosition;
                var texture = ModContent.GetTexture(this.GetPath("_Flame"));
                for (int k = 0; k < 7; k++)
                {
                    Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(Utils.RandomInt(ref randSeed, -10, 11) * 0.15f, Utils.RandomInt(ref randSeed, -10, 1) * 0.35f),
                        new Rectangle(Main.tile[i, j].frameX - 36, Main.tile[i, j].frameY - 36, 54, 54), new Color(100, 100, 100, 0), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}