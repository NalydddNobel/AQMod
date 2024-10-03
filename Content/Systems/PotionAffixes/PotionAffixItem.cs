using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.DataSets;
using Aequus.Common.Entities.Items;
using Terraria.Audio;
using Terraria.Localization;

namespace Aequus.Content.Systems.PotionAffixes;

internal class PotionAffixItem(UnifiedPotionAffix Parent) : InstancedModItem(Parent.Name, $"{Parent.NamespacePath()}/{Parent.Name}Item"), IItemSlotOverrideWhileHeldInMouse {
    public override LocalizedText DisplayName => Parent.GetLocalization("ItemName");
    public override LocalizedText Tooltip => Parent.GetLocalization("ItemTooltip");

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 1);
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
    }

    public bool CanApplyPotionAffix(Item potion) {
        return ItemSets.IsPotion.Contains(potion.type) && potion.CanApplyPrefix(Parent.Type) && potion.prefix != Parent.Type;
    }

    bool IItemSlotOverrideWhileHeldInMouse.RightClickSlot(ref Item heldItem, Item[] inv, int context, int slot, Player player) {
        Item potion = inv[slot];
        int wantedPrefix = Parent.Type;

        if (!CanApplyPotionAffix(potion)) {
            return false;
        }

        if (potion.stack > heldItem.stack) {
            int difference = potion.stack - heldItem.stack;

            Item pickupItem = potion.Clone();
            pickupItem.stack = difference;
            pickupItem.newAndShiny = true;

            potion.ResetPrefix();
            potion.Prefix(wantedPrefix);

            player.GetItem(player.whoAmI, pickupItem, GetItemSettings.ItemCreatedFromItemUsage);

            potion.stack = heldItem.stack;
        }
        else {
            potion.ResetPrefix();
            potion.Prefix(wantedPrefix);
        }

        if (potion.prefix == wantedPrefix) {
            SoundEngine.PlaySound(AequusSounds.PotionPrefix with { Volume = 0.75f, PitchVariance = 0.2f });
            heldItem.stack -= potion.stack;
            if (heldItem.stack <= 0) {
                heldItem.TurnToAir();
            }
        }
        return true;
    }
}
