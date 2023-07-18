using Aequus.Common.Items;
using Aequus.Common.Items.Variants;
using Aequus.Common.Items.EquipmentBooster;
using Terraria;
using Terraria.GameContent.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.SentryChip {
    [LegacyName("SantankSentry")]
    public class Sentry6502 : ModItem, ItemHooks.IUpdateItemDye {
        public override void SetStaticDefaults() {
            AequusItemVariants.AddVariant(Type, ItemVariants.WeakerVariant, Condition.RemixWorld);
            EquipBoostDatabase.Instance.SetNoEffect(Type);
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.LightRed;
            Item.hasVanityEffects = true;
            Item.value = Item.buyPrice(gold: 10);

            if (Item.Variant == ItemVariants.WeakerVariant) {
                Item.rare = ItemDefaults.RarityGlimmer;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accSentryInheritence = Item;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player) {
            if (incomingItem.ModItem is Sentry6502) {
                return equippedItem.ModItem is not Sentry6502;
            }
            return true;
        }

        public virtual void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                player.Aequus().equippedEyes = Type;
                player.Aequus().cEyes = dyeItem.dye;
            }
        }
    }
}