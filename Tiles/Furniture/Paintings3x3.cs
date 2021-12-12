using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Furniture
{
    [Obsolete("Use trophies instead!")]
    public class Paintings3x3 : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            dustType = 7;
            disableSmartCursor = true;
            AddMapEntry(new Color(120, 85, 60), CreateMapEntryName("Painting"));
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Main.tile[i, j].type = (ushort)ModContent.TileType<Trophies>();
            Main.tile[i, j].frameX += 162;
            NetMessage.SendTileSquare(-1, i, j, 1);
            return base.PreDraw(i, j, spriteBatch);
        }
    }
}