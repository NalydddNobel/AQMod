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

    public bool UseGoldenKey(Item[] inv, int slot) {
        if (goldenKey == null) {
            return false;
        }

        if (inv[slot].type == ItemID.LockBox) {
            if ((goldenKey.consumable || goldenKey.type == ItemID.GoldenKey) && ItemLoader.ConsumeItem(goldenKey, Player)) {
                goldenKey.stack--;
                if (goldenKey.stack < 0) {
                    goldenKey.TurnToAir();
                }
            }

            if (ItemLoader.ConsumeItem(inv[slot], Player)) {
                inv[slot].stack--;
            }
            if (inv[slot].stack < 0) {
                inv[slot].SetDefaults();
            }
            Main.stackSplit = 30;
            Main.mouseRightRelease = false;
            Player.OpenLockBox(inv[slot].type);
            Recipe.FindRecipes();
            return true;
        }
        if (ItemDataSet.IsDungeonLockBox.Contains(inv[slot].type)) {
            if ((goldenKey.consumable || goldenKey.type == ItemID.GoldenKey) && ItemLoader.ConsumeItem(goldenKey, Player)) {
                goldenKey.stack--;
                if (goldenKey.stack < 0) {
                    goldenKey.TurnToAir();
                }
            }

            ItemLoader.RightClick(inv[slot], Player);
            Main.ItemDropSolver.TryDropping(new() {
                item = inv[slot].type,
                player = Player,
                IsExpertMode = Main.expertMode,
                IsMasterMode = Main.masterMode,
                rng = Main.rand,
            });
            return true;
        }
        return false;
    }

    public bool UseShadowKey(Item[] inv, int slot) {
        if (shadowKey == null) {
            return false;
        }

        if (inv[slot].type == ItemID.ObsidianLockbox) {
            if (shadowKey.consumable && ItemLoader.ConsumeItem(shadowKey, Player)) {
                shadowKey.stack--;
                if (shadowKey.stack < 0) {
                    shadowKey.TurnToAir();
                }
            }

            if (ItemLoader.ConsumeItem(inv[slot], Player)) {
                inv[slot].stack--;
            }
            if (inv[slot].stack < 0) {
                inv[slot].SetDefaults();
            }
            Main.stackSplit = 30;
            Main.mouseRightRelease = false;
            Player.OpenShadowLockbox(inv[slot].type);
            Recipe.FindRecipes();
            return true;
        }
        if (ItemDataSet.IsHellLockBox.Contains(inv[slot].type)) {
            if (shadowKey.consumable && ItemLoader.ConsumeItem(shadowKey, Player)) {
                shadowKey.stack--;
                if (shadowKey.stack < 0) {
                    shadowKey.TurnToAir();
                }
            }

            ItemLoader.RightClick(inv[slot], Player);
            Main.ItemDropSolver.TryDropping(new() {
                item = inv[slot].type,
                player = Player,
                IsExpertMode = Main.expertMode,
                IsMasterMode = Main.masterMode,
                rng = Main.rand,
            });
            return true;
        }
        return false;
    }

    // TODO: Support Quick Buff
}