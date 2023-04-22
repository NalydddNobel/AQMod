using Aequus.Items.Accessories.CrownOfBlood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Offense.Sentry {
    [LegacyName("SantankSentry")]
    public class Sentry6502 : ModItem, ItemHooks.IUpdateItemDye {
        public override void SetStaticDefaults() {
            CrownOfBloodItem.NoBoost.Add(Type);
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.hasVanityEffects = true;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accSentryInheritence = Item;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player) {
            return CheckSentinel6510(equippedItem) && CheckSentinel6510(incomingItem);
        }
        public bool CheckSentinel6510(Item item) {
            return item.type != ModContent.ItemType<Sentinel6510>();
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                player.Aequus().equippedEyes = Type;
                player.Aequus().cEyes = dyeItem.dye;
            }
        }
    }
}