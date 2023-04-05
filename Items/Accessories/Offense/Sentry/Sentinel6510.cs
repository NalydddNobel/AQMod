using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Offense.Sentry
{
    [LegacyName("MechsSentry")]
    public class Sentinel6510 : ModItem, ItemHooks.IUpdateItemDye
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
            Item.rare = ItemRarityID.Pink;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.value = Item.sellPrice(gold: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.accSentrySlot = true;
            aequus.accSentryInheritence = Item;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return CheckSentry6502(equippedItem) && CheckSentry6502(incomingItem);
        }
        public bool CheckSentry6502(Item item)
        {
            return item.type != ModContent.ItemType<Sentry6502>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Sentry6502>())
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.PapyrusScarab);
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (!isSetToHidden || !isNotInVanitySlot)
            {
                player.Aequus().equippedEyes = Type;
                player.Aequus().cEyes = dyeItem.dye;
            }
        }
    }
}