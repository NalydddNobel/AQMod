using AQMod.Common.Graphics;
using AQMod.Items.Placeable.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Furniture
{
    public sealed class AQTombstones : ModTile
    {
        public const int HellTombstone = 0;
        public const int HellGraveMarker = 1;
        public const int HellCrossGraveMarker = 2;
        public const int HellCrossHeadstone = 3;
        public const int HellCrossGravestone = 4;
        public const int HellObelisk = 5;

        public override void SetDefaults()
        {
            Main.tileSign[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.RandomStyleRange = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(100, 20, 10, 255), CreateMapEntryName("HellstoneTombstone"));
            dustType = 37;
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Tombstones };
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override bool NewRightClick(int i, int j)
        {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new Rectangle(i * 16, j * 16, 32, 32), ModContent.ItemType<HellTombstone>());
            Sign.KillSign(i, j);
        }

        public override void MouseOverFar(int i, int j)
        {
            MouseOver(i, j);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.02f;
            g = 0.001f;
            b = 0.001f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var frame = new Rectangle(Main.tile[i, j].frameX, Main.tile[i, j].frameY, 16, 16);
            if (Main.tile[i, j].frameY >= 18)
            {
                frame.Y = 18;
            }
            Main.spriteBatch.Draw(ModContent.GetTexture(this.GetPath("_Glow")), new Vector2(i * 16f - Main.screenPosition.X, j * 16f - Main.screenPosition.Y) + AQGraphics.TileZero,
                frame, new Color(200, 100, 100, 0) * AQUtils.Wave(Main.GlobalTime * 10f, 0.3f, 0.7f), 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 0f);
        }
    }
}
