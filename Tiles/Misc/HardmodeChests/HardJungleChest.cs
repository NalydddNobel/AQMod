using Aequus.Content.World;
using Aequus.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Misc.HardmodeChests {
    public class HardJungleChest : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.IvyChest;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardJungleChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void AddRecipes() {
            CreateRecipe(5)
                .AddIngredient(ItemID.IvyChest, 5)
                .AddIngredient(ItemID.TurtleShell)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class HardJungleChestTile : BaseChest<HardJungleChest> {
        public override Color MapColor => new(170, 105, 70);
        public override void SetStaticDefaults() {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Ivy);
            ChestType.IsGenericUndergroundChest.Add(new(Type));
            base.SetStaticDefaults();
            DustType = DustID.WoodFurniture;
        }
    }
}