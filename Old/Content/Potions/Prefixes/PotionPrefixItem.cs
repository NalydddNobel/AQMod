using Aequus.Common.Items.Components;
using Aequus.Content.DataSets;
using Aequus.Core.ContentGeneration;
using Terraria.Audio;
using Terraria.Localization;

namespace Aequus.Old.Content.Potions.Prefixes;

internal class PotionPrefixItem : InstancedModItem, IRightClickOverrideWhenHeld {
    [CloneByReference]
    private readonly PotionPrefix _parent;

    public PotionPrefixItem(PotionPrefix prefix) : base(prefix.Name, $"{prefix.NamespaceFilePath()}/{prefix.Name}Item") {
        _parent = prefix;
    }

    public override LocalizedText DisplayName => _parent.GetLocalization("ItemName");
    public override LocalizedText Tooltip => _parent.GetLocalization("ItemTooltip");

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Blue;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
    }

    public bool RightClickOverrideWhileHeld(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer aequus) {
        Item potion = inv[slot];
        int wantedPrefix = _parent.Type;

        if (!ItemSets.Potions.Contains(potion.type) || potion.prefix == wantedPrefix) {
            return false;
        }

        if (potion.stack > heldItem.stack) {
            int difference = potion.stack - heldItem.stack;

            Item pickupItem = potion.Clone();
            pickupItem.stack = difference;

            potion.Prefix(wantedPrefix);

            player.GetItem(player.whoAmI, pickupItem, GetItemSettings.ItemCreatedFromItemUsage);

            potion.stack = heldItem.stack;
        }
        else {
            potion.Prefix(wantedPrefix);
        }

        if (potion.prefix == wantedPrefix) {
            SoundEngine.PlaySound(SoundID.Item37);
            heldItem.stack -= potion.stack;
            if (heldItem.stack <= 0) {
                heldItem.TurnToAir();
            }
        }
        return true;
    }
}
