using AQMod.Common.Graphics;
using AQMod.Common.Utilities;
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
    public sealed class WallClocks : ModTile
    {
        public const int CrabClock = 0;

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            dustType = 7;
            disableSmartCursor = true;
            AddMapEntry(new Color(120, 85, 60), CreateMapEntryName("WallClocks"));
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].frameY == 18 && (Main.tile[i, j].frameX % 36) == 18) // Bottom right
            {
                var texture = Main.tileTexture[Type];
                var handFrame = new Rectangle(Main.tile[i, j].frameX - 18, 36, 16, 16);
                var origin = new Vector2(7f, 8f);
                var drawCoordinates = new Vector2(i * 16f - 1f, j * 16f + 2f) - Main.screenPosition + AQGraphics.TileZero;
                var color = Lighting.GetColor(i, j);
                float progress = (float)TimeActions.GetInGameTimePercentageUsing12AMAs0Percent();
                handFrame.Y += 16;
                Main.spriteBatch.Draw(texture, drawCoordinates, handFrame, color, progress * MathHelper.TwoPi * 48f + MathHelper.Pi, origin, 1f, SpriteEffects.None, 0f);
                handFrame.Y -= 16;
                Main.spriteBatch.Draw(texture, drawCoordinates, handFrame, color, progress * MathHelper.TwoPi * 2f + MathHelper.Pi, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new Rectangle(i * 16, j * 16, 32, 32), ModContent.ItemType<CrabClock>());
        }
    }
}