using Aequus.Common.Backpacks;
using Aequus.Common.Items.Components;
using Aequus.Common.UI;
using Aequus.Content.DataSets;
using Aequus.Core.Generator;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.UI;

namespace Aequus;

public partial class AequusPlayer {
    public Byte disableItem;

    public Int32 itemHits;
    /// <summary>
    /// Tracks <see cref="Player.selectedItem"/>
    /// </summary>
    public Int32 lastSelectedItem = -1;
    /// <summary>
    /// Increments when the player uses an item. Does not increment when the player is using the alt function of an item.
    /// </summary>
    public UInt16 itemUsage;
    /// <summary>
    /// A short lived timer which gets set to 30 when the player has a different selected item.
    /// </summary>
    public UInt16 itemSwitch;

    public Boolean forceUseItem;

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
            itemSwitch = Math.Max((UInt16)30, itemSwitch);
            itemUsage = 0;
            itemHits = 0;
        }
    }

    public override Boolean HoverSlot(Item[] inventory, Int32 context, Int32 slot) {
        Boolean returnValue = false;
        if (inventory[slot].ModItem is IHoverSlot hoverSlot) {
            returnValue |= hoverSlot.HoverSlot(inventory, context, slot);
        }
        if (inventory[slot].ModItem is ITransformItem transformItem && (context == ItemSlot.Context.InventoryItem || Math.Abs(context) == ItemSlot.Context.EquipAccessory) && Main.mouseRight && Main.mouseRightRelease && Main.LocalPlayer.ItemTimeIsZero) {
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

    public Boolean UseGoldenKey(Item[] inv, Int32 slot) {
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
        if (ItemSets.IsDungeonLockBox.Contains(inv[slot].type)) {
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

    public Boolean UseShadowKey(Item[] inv, Int32 slot) {
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
        if (ItemSets.IsHellLockBox.Contains(inv[slot].type)) {
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
    private static void ItemSlot_RightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, Int32 context, Int32 slot) {
        if (Main.mouseRight && Main.mouseRightRelease) {
            var player = Main.LocalPlayer;
            var aequus = player.GetModPlayer<AequusPlayer>();
            if (Main.mouseItem.ModItem is IRightClickOverrideWhenHeld rightClickOverride && rightClickOverride.RightClickOverrideWhileHeld(ref Main.mouseItem, inv, context, slot, player, aequus)) {
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

        for (Int32 i = 0; i < backpackPlayer.backpacks.Length; i++) {
            if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsQuickStack) {
                continue;
            }
            BackpackLoader.QuickStackToNearbyChest(player.Center, backpackPlayer.backpacks[i]);
        }
    }

    private static void On_ChestUI_QuickStack(On_ChestUI.orig_QuickStack orig, ContainerTransferContext context, Boolean voidStack) {
        orig(context, voidStack);
        if (voidStack || !Main.LocalPlayer.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return;
        }

        var player = Main.LocalPlayer;
        var containerWorldPosition = context.GetContainerWorldPosition();

        var chest = player.GetCurrentChest();
        Boolean anyTransfers = false;
        if (chest != null) {
            for (Int32 i = 0; i < backpackPlayer.backpacks.Length; i++) {
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

    private static Boolean On_Player_ConsumeItem(On_Player.orig_ConsumeItem orig, Player player, Int32 type, Boolean reverseOrder, Boolean includeVoidBag) {
        Boolean consumedItem = orig(player, type, reverseOrder, includeVoidBag);

        if (!consumedItem && includeVoidBag && player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            for (Int32 i = 0; i < backpackPlayer.backpacks.Length; i++) {
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
            for (Int32 i = 0; i < backpackPlayer.backpacks.Length && item == null; i++) {
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
            for (Int32 i = 0; i < backpackPlayer.backpacks.Length && item == null; i++) {
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
            for (Int32 i = 0; i < backpackPlayer.backpacks.Length && item == null; i++) {
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