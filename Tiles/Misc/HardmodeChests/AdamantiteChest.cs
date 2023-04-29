using Aequus.Content.World;
using Aequus.Items;
using Aequus.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Misc.HardmodeChests {
    public class AdamantiteChest : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.GoldChest;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<AdamantiteChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void AddRecipes() {
            CreateRecipe(5)
                .AddIngredient(ItemID.GoldChest, 5)
                .AddIngredient(ItemID.AdamantiteBar, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register()
                .DisableDecraft()
                .Clone()
                .ReplaceItem(ItemID.AdamantiteBar, ItemID.TitaniumBar)
                .Register();
        }
    }

    public class AdamantiteChestTile : BaseChest<AdamantiteChest> {
        public override Color MapColor => new(160, 25, 50);

        public override void SetStaticDefaults() {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Gold);
            ChestType.IsGenericUndergroundChest.Add(new(Type));
            base.SetStaticDefaults();
            DustType = DustID.Adamantite;
        }
    }
}