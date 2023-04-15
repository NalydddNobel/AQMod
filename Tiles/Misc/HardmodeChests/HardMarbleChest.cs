using Aequus.Content.World;
using Aequus.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Misc.HardmodeChests {
    public class HardMarbleChest : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MarbleChest;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardMarbleChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void AddRecipes() {
            CreateRecipe(5)
                .AddIngredient(ItemID.MarbleChest, 5)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class HardMarbleChestTile : BaseChest {
        public override void SetStaticDefaults() {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Gold);
            ChestType.IsGenericUndergroundChest.Add(new TileKey(Type));
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ItemDrop = ModContent.ItemType<HardMarbleChest>();
            AddMapEntry(new Color(200, 185, 100), CreateMapEntryName());
        }
    }
}