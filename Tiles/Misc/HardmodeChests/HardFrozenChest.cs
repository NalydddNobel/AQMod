using Aequus.Common.Tiles;
using Aequus.Content.World;
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
                .AddIngredient(ItemID.IceChest, 5)
                .AddIngredient(ItemID.FrostCore)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class HardFrozenChestTile : BaseChest<HardFrozenChest> {
        public override Color MapColor => new(105, 115, 255);

        public override void SetStaticDefaults() {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Frozen);
            ChestType.IsGenericUndergroundChest.Add(new(Type));
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
        }
    }
}