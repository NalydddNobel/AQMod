using Aequus.Items.Placeable.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture
{
    public sealed class WallClocks : ModTile
    {
        public const int CrabClock = 0;

        public static List<Point> RenderPoints { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                RenderPoints = new List<Point>();
            }
        }
        public override void Unload()
        {
            RenderPoints?.Clear();
            RenderPoints = null;
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            DustType = 7;
            AddMapEntry(new Color(120, 85, 60), CreateMapEntryName("WallClocks"));

            if (!Main.dedServ)
            {
                AequusTile.ResetTileRenderPoints += () => RenderPoints.Clear();
                AequusTile.DrawSpecialTilePoints += DrawClockHands;     
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.tile[i, j].TileFrameY == 18 && Main.tile[i, j].TileFrameX % 36 == 18) // Bottom right
            {
                RenderPoints.Add(new Point(i, j));
            }
        }

        public static void DrawClockHands()
        {
            foreach (var p in RenderPoints)
            {
                var texture = TextureAssets.Tile[ModContent.TileType<WallClocks>()].Value;
                var handFrame = new Rectangle(Main.tile[p].TileFrameY - 18, 36, 16, 16);
                var origin = new Vector2(7f, 8f);
                var drawCoordinates = new Vector2(p.X * 16f - 1f, p.Y * 16f + 2f) - Main.screenPosition;
                var color = Lighting.GetColor(p);
                float progress = (float)Utils.GetDayTimeAs24FloatStartingFromMidnight() / 24f;
                handFrame.Y += 16;
                Main.spriteBatch.Draw(texture, drawCoordinates, handFrame, color, progress * MathHelper.TwoPi * 48f + MathHelper.Pi, origin, 1f, SpriteEffects.None, 0f);
                handFrame.Y -= 16;
                Main.spriteBatch.Draw(texture, drawCoordinates, handFrame, color, progress * MathHelper.TwoPi * 2f + MathHelper.Pi, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 32, 32), ModContent.ItemType<CrabClock>());
        }
    }
}