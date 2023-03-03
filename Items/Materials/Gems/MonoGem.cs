using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Gems
{
    public class MonoGem : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MonoGemTile>());
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override void AddRecipes()
        {
        }
    }

    public class MonoGemTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileNoFail[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;

            AddMapEntry(new Color(66, 55, 55), Lang.GetItemName(ModContent.ItemType<MonoGem>()));
            DustType = DustID.Ambient_DarkBrown;
            ItemDrop = ModContent.ItemType<MonoGem>();
        }

        public override bool CanPlace(int i, int j)
        {
            var top = Framing.GetTileSafely(i, j - 1);
            if (top.HasTile && !top.BottomSlope && top.TileType >= 0 && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType])
            {
                return true;
            }
            var bottom = Framing.GetTileSafely(i, j + 1);
            if (bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType]))
            {
                return true;
            }
            var left = Framing.GetTileSafely(i - 1, j);
            if (left.HasTile && left.TileType >= 0 && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType])
            {
                return true;
            }
            var right = Framing.GetTileSafely(i + 1, j);
            if (right.HasTile && right.TileType >= 0 && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType])
            {
                return true;
            }
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.01f;
            g = 0.1f;
            b = 0.2f;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            AequusTile.GemFrame(i, j);
            return false;
        }
    }

}