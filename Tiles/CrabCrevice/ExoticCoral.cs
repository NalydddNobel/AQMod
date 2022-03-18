using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.CrabCrevice
{
    public class ExoticCoral : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateHeights = new[] { 20, };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 6;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.OnlyInLiquid;
            TileObjectData.newTile.AnchorValidTiles = new int[] { TileID.Dirt, TileID.Stone, TileID.Obsidian, TileID.Sand, TileID.HardenedSand, };
            TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            TileObjectData.newSubTile.RandomStyleRange = 3;
            TileObjectData.addSubTile(8);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(245, 122, 122), CreateMapEntryName("ExoticCoral"));
            dustType = DustID.Dirt;
            disableSmartCursor = true;
        }

        public override bool Drop(int i, int j)
        {
            Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<Items.Placeable.CrabCrevice.ExoticCoral>());
            return true;
        }

        public override void RandomUpdate(int i, int j)
        {
            switch (Main.tile[i, j].frameX)
            {
                case 0:
                case 54:
                    {
                        Main.tile[i, j].type = (ushort)ModContent.TileType<ExoticCoralNew>();
                        Main.tile[i, j].frameX = 0;
                    }
                    break;
                case 18:
                case 72:
                    {
                        Main.tile[i, j].type = (ushort)ModContent.TileType<ExoticCoralNew>();
                        Main.tile[i, j].frameX = 88;
                    }
                    break;
                case 36:
                case 90:
                    {
                        Main.tile[i, j].type = (ushort)ModContent.TileType<ExoticCoralNew>();
                        Main.tile[i, j].frameX = 176;
                    }
                    break;
            }
        }
    }
}