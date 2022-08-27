using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Sentry
{
    public class IcebergKraken : ModItem, ItemHooks.IUpdateItemDye
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.value = ItemDefaults.GaleStreamsValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().turretSquidItem = Item;
            player.Aequus().accFrostburnTurretSquid = true;
            player.maxTurrets++;
        }

        public override void AddRecipes()
        {
            AequusRecipes.SpaceSquidRecipe(this, ModContent.ItemType<SentrySquid>(), sort: false)
                .SortAfterFirstRecipesOf(ItemID.PygmyNecklace);
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (!isSetToHidden || !isNotInVanitySlot)
            {
                player.Aequus().equippedHat = Type;
                player.Aequus().cHat = dyeItem.dye;
            }
        }
    }
}