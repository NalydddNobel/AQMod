using Aequus.Content.WorldGeneration;
using Aequus.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Misc.HardmodeChests {
    public class HardFrozenChest : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.FrozenChest;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardFrozenChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void AddRecipes() {
            CreateRecipe(5)
                .AddIngredient(ItemID.FrozenChest, 5)
                .AddIngredient(ItemID.FrostCore)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class HardFrozenChestTile : BaseChest {
        public override void SetStaticDefaults() {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Frozen);
            ChestType.IsGenericUndergroundChest.Add(new TileKey(Type));
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ItemDrop = ModContent.ItemType<HardFrozenChest>();
            AddMapEntry(new Color(105, 115, 255), CreateMapEntryName());
        }
    }
}