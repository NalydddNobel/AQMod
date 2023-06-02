using Aequus.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Bait {
    public class IchorPopper : ModItem, ItemHooks.IModifyFishingPower {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults() {
            Item.width = 6;
            Item.height = 6;
            Item.bait = 31;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Green;
        }

        void ItemHooks.IModifyFishingPower.ModifyFishingPower(Player player, AequusPlayer fishing, Item fishingRod, ref float fishingLevel) {
            if (player.ZoneCrimson)
                fishingLevel += 0.35f;
        }

        public override void AddRecipes() {
            CreateRecipe(10)
                .AddIngredient(ItemID.BloodWater, 10)
                .AddIngredient(ItemID.Ichor, 10)
                .AddTile(TileID.Bottles)
                .TryRegisterBefore(ItemID.EnchantedNightcrawler);
        }
    }
}