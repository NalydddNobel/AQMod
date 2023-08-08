using Aequus.Common.Items;
using Aequus.Items.Materials.GaleStreams;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Combat.Sentry.SentrySquid {
    public class IcebergKraken : ModItem, ItemHooks.IUpdateItemDye {
        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.hasVanityEffects = true;
            Item.value = ItemDefaults.ValueGaleStreams;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accSentrySquid = Item;
            player.Aequus().accFrostburnTurretSquid++;
            player.maxTurrets++;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<SentrySquid>()
                .AddIngredient<FrozenTear>(12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                player.Aequus().equippedHat = Type;
                player.Aequus().cHat = dyeItem.dye;
            }
        }
    }
}