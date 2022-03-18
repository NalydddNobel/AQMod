using AQMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Tiles
{
    public class LightbulbTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileObsidianKill[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            AddMapEntry(new Color(240, 255, 10), CreateMapEntryName());
        }

        public override bool CanPlace(int i, int j)
        {
            Tile top = Framing.GetTileSafely(i, j - 1);
            if (top.active() && !top.bottomSlope() && top.type >= 0 && Main.tileSolid[top.type] && !Main.tileSolidTop[top.type])
            {
                return true;
            }
            Tile bottom = Framing.GetTileSafely(i, j + 1);
            if (bottom.active() && !bottom.halfBrick() && !bottom.topSlope() && bottom.type >= 0 && (Main.tileSolid[bottom.type] || Main.tileSolidTop[bottom.type]))
            {
                return true;
            }
            Tile left = Framing.GetTileSafely(i - 1, j);
            if (left.active() && left.type >= 0 && Main.tileSolid[left.type] && !Main.tileSolidTop[left.type])
            {
                return true;
            }
            Tile right = Framing.GetTileSafely(i + 1, j);
            if (right.active() && right.type >= 0 && Main.tileSolid[right.type] && !Main.tileSolidTop[right.type])
            {
                return true;
            }
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }

        public override bool Drop(int i, int j)
        {
            Item.NewItem(new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<Lightbulb>());
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].frameX == 0)
            {
                r = 0.44f;
                g = 0.5f;
                b = 0.05f;
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
        {
            if (Main.tile[i, j].frameX == 0)
                drawColor = new Color(255, 255, 255, 255);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileUtils.GemFrame(i, j);
            return false;
        }

        public override void HitWire(int i, int j)
        {
            Main.tile[i, j].frameX = Main.tile[i, j].frameX == 0 ? (short)18 : (short)0;
        }
    }
}