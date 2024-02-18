using Aequus.Common.Backpacks;
using Aequus.Common.Items.Components;
using Aequus.Common.UI;
using Aequus.Content.DataSets;
using Aequus.Core.CodeGeneration;
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
        if (UISystem.TalkInterface?.CurrentState is AequusUIState aequusUI) {
            returnValue |= aequusUI.HoverSlot(inventory, context, slot);
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
        if (ItemMetadata.IsDungeonLockBox.Contains(inv[slot].type)) {
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
        if (ItemMetadata.IsHellLockBox.Contains(inv[slot].type)) {
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

    #region Hooks
    private static void ItemSlot_RightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        if (Main.mouseRight && Main.mouseRightRelease) {
            var player = Main.LocalPlayer;
            var aequus = player.GetModPlayer<AequusPlayer>();
            if (!Main.mouseItem.IsAir && Main.mouseItem.ModItem is IRightClickOverrideWhenHeld rightClickOverride && rightClickOverride.RightClickOverrideWhileHeld(ref Main.mouseItem, inv, context, slot, player, aequus)) {
                Main.mouseRightRelease = false;
                // Set stack split delay to 3 seconds (so you don't instantly pick up the item with rclick)
                Main.stackSplit = 180;
                return;
            }

            if (context == ItemSlot.Context.InventoryItem) {
                if (aequus.UseGoldenKey(inv, slot)) {
                    return;
                }
                if (aequus.UseShadowKey(inv, slot)) {
                    return;
                }
            }
        }

        orig(inv, context, slot);
    }

    private static void On_Player_QuickStackAllChests(On_Player.orig_QuickStackAllChests orig, Player player) {
        orig(player);
        if (player.HasLockedInventory() || !player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return;
        }

        for (int i = 0; i < backpackPlayer.backpacks.Length; i++) {
            if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsQuickStack) {
                continue;
            }
            BackpackLoader.QuickStackToNearbyChest(player.Center, backpackPlayer.backpacks[i]);
        }
    }

    private static void On_ChestUI_QuickStack(On_ChestUI.orig_QuickStack orig, ContainerTransferContext context, bool voidStack) {
        orig(context, voidStack);
        if (voidStack || !Main.LocalPlayer.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return;
        }

        var player = Main.LocalPlayer;
        var containerWorldPosition = context.GetContainerWorldPosition();

        var chest = player.GetCurrentChest();
        bool anyTransfers = false;
        if (chest != null) {
            for (int i = 0; i < backpackPlayer.backpacks.Length; i++) {
                if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsQuickStack) {
                    continue;
                }
                BackpackLoader.QuickStack(backpackPlayer.backpacks[i], chest, containerWorldPosition, context);
            }
        }

        if (anyTransfers) {
            SoundEngine.PlaySound(SoundID.Grab);
        }
    }

    private static bool On_Player_ConsumeItem(On_Player.orig_ConsumeItem orig, Player player, int type, bool reverseOrder, bool includeVoidBag) {
        bool consumedItem = orig(player, type, reverseOrder, includeVoidBag);

        if (!consumedItem && includeVoidBag && player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            for (int i = 0; i < backpackPlayer.backpacks.Length; i++) {
                if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsConsumeItem) {
                    continue;
                }
                BackpackLoader.ConsumeItem(player, backpackPlayer.backpacks[i], type, reverseOrder);
            }
        }

        return consumedItem;
    }

    // TODO: Support Quick Buff

    private static Item On_Player_QuickMana_GetItemToUse(On_Player.orig_QuickMana_GetItemToUse orig, Player player) {
        var item = orig(player);

        if (item == null && player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            for (int i = 0; i < backpackPlayer.backpacks.Length && item == null; i++) {
                if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsConsumeItem) {
                    continue;
                }
                item = BackpackLoader.GetQuickManaItem(player, backpackPlayer.backpacks[i]);
            }
        }

        return item;
    }

    private static Item On_Player_QuickHeal_GetItemToUse(On_Player.orig_QuickHeal_GetItemToUse orig, Player player) {
        var item = orig(player);

        if (item == null && player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            for (int i = 0; i < backpackPlayer.backpacks.Length && item == null; i++) {
                if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsConsumeItem) {
                    continue;
                }
                item = BackpackLoader.GetQuickHealItem(player, backpackPlayer.backpacks[i]);
            }
        }

        return item;
    }

    private static Item On_Player_QuickMount_GetItemToUse(On_Player.orig_QuickMount_GetItemToUse orig, Player player) {
        var item = orig(player);

        if (item == null && player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            for (int i = 0; i < backpackPlayer.backpacks.Length && item == null; i++) {
                if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsConsumeItem) {
                    continue;
                }
                item = BackpackLoader.GetQuickMountItem(player, backpackPlayer.backpacks[i]);
            }
        }

        return item;
    }
    #endregion
}