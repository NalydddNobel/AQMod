using Aequus.Common.Backpacks;
using Aequus.Common.Items.Components;
using Aequus.Core.CodeGeneration;
using Aequus.Core.UI;
using Aequus.DataSets;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.UI;

namespace Aequus;

public partial class AequusPlayer {
    public byte disableItem;

    public int itemHits;
    /// <summary>
    /// Tracks <see cref="Player.selectedItem"/>
    /// </summary>
    public int lastSelectedItem = -1;
    /// <summary>
    /// Increments when the player uses an item. Does not increment when the player is using the alt function of an item.
    /// </summary>
    public ushort itemUsage;
    /// <summary>
    /// A short lived timer which gets set to 30 when the player has a different selected item.
    /// </summary>
    public ushort itemSwitch;

    public bool forceUseItem;

    [ResetEffects]
    public Item goldenKey;
    [ResetEffects]
    public Item shadowKey;

    public void UpdateItemFields() {
        if (Player.itemAnimation == 0 && disableItem > 0) {
            disableItem--;
        }

        if (itemSwitch > 0) {
            itemUsage = 0;
            itemSwitch--;
        }
        else if (Player.itemTime > 0) {
            itemUsage++;
        }
        else {
            itemUsage = 0;
        }
    }

    public override void PostItemCheck() {
        if (Player.selectedItem != lastSelectedItem) {
            lastSelectedItem = Player.selectedItem;
            itemSwitch = Math.Max((ushort)30, itemSwitch);
            itemUsage = 0;
            itemHits = 0;
        }
    }

    public override bool HoverSlot(Item[] inventory, int context, int slot) {
        bool returnValue = false;
        if (inventory[slot].ModItem is IHoverSlot hoverSlot) {
            returnValue |= hoverSlot.HoverSlot(inventory, context, slot);
        }
        if (inventory[slot].ModItem is ITransformItem transformItem && (context == ItemSlot.Context.InventoryItem || Math.Abs(context) == ItemSlot.Context.EquipAccessory) && Main.mouseRight && Main.mouseRightRelease && Main.LocalPlayer.ItemTimeIsZero && CombinedHooks.CanUseItem(Player, inventory[slot])) {
            transformItem.SlotTransform(inventory, context, slot);
            Main.mouseRightRelease = false;
        }
        return returnValue;
    }

    private void SetControls_ForceItemUse() {
        if (forceUseItem) {
            Player.controlUseItem = true;
            Player.releaseUseItem = true;
            Player.itemAnimation = 0;
        }
        forceUseItem = false;
    }

    // TODO: Support Quick Buff
}