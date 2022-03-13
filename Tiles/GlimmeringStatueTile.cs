using AQMod.Common.WorldGeneration;
using AQMod.Items.Placeable.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles
{
    public class GlimmeringStatueTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
            TileObjectData.newTile.AnchorInvalidTiles = new[] { (int)TileID.MagicalIceBlock, };
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(75, 139, 166), CreateMapEntryName());
            dustType = 15;
            soundType = SoundID.Shatter;
            soundStyle = 1;
            disableSmartCursor = true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<GlimmeringStatue>());
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<GlimmeringStatue>();
        }

        public override bool NewRightClick(int i, int j)
        {
            Main.PlaySound(SoundID.Mech, i * 16, j * 16, 0);
            HitWire(i, j);
            return true;
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int x = i - tile.frameX / 18 % 2;
            int y = j - tile.frameY / 18 % 3;
            if (Wiring.running)
            {
                Wiring.SkipWire(x, y);
                Wiring.SkipWire(x + 1, y);
                Wiring.SkipWire(x, y + 1);
                Wiring.SkipWire(x + 1, y + 1);
                Wiring.SkipWire(x, y + 2);
                Wiring.SkipWire(x + 1, y + 2);
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var texture = ModContent.GetTexture(this.GetPath("_Glow"));
            var frame = new Rectangle(Main.tile[i, j].frameX, Main.tile[i, j].frameY, 16, Main.tile[i, j].frameY == 36 ? 18 : 16);
            spriteBatch.Draw(texture, new Vector2(i * 16f, j * 16f) - Main.screenPosition + AQMod.Zero, frame, new Color(255, 255, 255, 255) * AQUtils.Wave(Main.GlobalTime * 5f, 0.2f, 0.3f), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        internal static bool TryGen(int x, int y)
        {
            if (!AQWorldGen.ActiveAndSolid(x, y) && !AQWorldGen.ActiveAndSolid(x - 1, y) && AQWorldGen.ActiveAndSolid(x, y + 1) && AQWorldGen.ActiveAndSolid(x - 1, y + 1) && Main.tile[x, y].wall == WallID.None)
            {
                WorldGen.PlaceTile(x, y, ModContent.TileType<GlimmeringStatueTile>());
                if (Main.tile[x, y].type == ModContent.TileType<GlimmeringStatueTile>())
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}