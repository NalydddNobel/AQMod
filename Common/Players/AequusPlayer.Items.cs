using Aequus.Common.Items.Components;
using Aequus.Common.Players.Attributes;
using Aequus.Common.UI;
using Aequus.Content.DataSets;
using Aequus.Core.IO;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus;

public partial class AequusPlayer {
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

    /// <summary>
    /// Do not use this for iteration, use extraInventory.Length instead. This stat simply resizes the array when needed during the player updated.
    /// </summary>
    [ResetEffects]
    public int extraInventorySlots;

    [SaveData("ExtraInventory")]
    public Item[] extraInventory = new Item[0];

    public int ExtraInventoryCount => Math.Min(extraInventorySlots, extraInventory.Length);

    [ResetEffects]
    public Item goldenKey;
    [ResetEffects]
    public Item shadowKey;

    private void CheckExtraInventoryMax() {
        if (extraInventorySlots > extraInventory.Length) {
            Array.Resize(ref extraInventory, extraInventorySlots);
            for (int i = 0; i < extraInventorySlots; i++) {
                if (extraInventory[i] == null) {
                    extraInventory[i] = new();
                }
            }
        }
        else if (extraInventorySlots >= 0 && extraInventorySlots < extraInventory.Length && timeSinceRespawn > 30) {
            for (int i = extraInventory.Length - 1; i >= extraInventorySlots; i--) {
                Player.QuickSpawnItem(new EntitySource_Misc("Aequus: ExtraInventory"), extraInventory[i].Clone(), extraInventory[i].stack);
            }
            Array.Resize(ref extraInventory, extraInventorySlots);
        }
    }

    public void UpdateItemFields() {
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

    public override IEnumerable<Item> AddMaterialsForCrafting(out ItemConsumedCallback itemConsumedCallback) {
        itemConsumedCallback = null;
        return extraInventory;
    }

    public override void ResetInfoAccessories() {
        for (int i = 0; i < extraInventory.Length; i++) {
            if (ItemID.Sets.WorksInVoidBag[extraInventory[i].type]) {
                ItemLoader.UpdateInventory(extraInventory[i], Player);
                Player.RefreshInfoAccsFromItemType(extraInventory[i]);
                Player.RefreshMechanicalAccsFromItemType(extraInventory[i].type);
            }
            if (extraInventory[i].type == ItemID.GoldenKey && goldenKey == null) {
                goldenKey = extraInventory[i];
            }
            if (extraInventory[i].type == ItemID.ShadowKey && (shadowKey == null || shadowKey.consumable)) {
                shadowKey = extraInventory[i];
            }
            if (extraInventory[i].type == ItemID.Football) {
                Player.hasFootball = true;
            }
        }
    }

    public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer) {
        if (!otherPlayer.TryGetModPlayer<AequusPlayer>(out var otherAequusPlayer)) {
            return;
        }

        // TODO: Automate this?
        accMonocle |= otherAequusPlayer.accMonocle;
        accShimmerMonocle |= otherAequusPlayer.accShimmerMonocle;
        accDayCalendar |= otherAequusPlayer.accDayCalendar;
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
        if (UISystem.TalkInterface?.CurrentState is AequusUIState aequusUI) {
            returnValue |= aequusUI.HoverSlot(inventory, context, slot);
        }
        return returnValue;
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
    private static void ItemSlot_RightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
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
        if (player.HasLockedInventory() || !player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) || aequusPlayer.ExtraInventoryCount <= 0) {
            return;
        }

        for (int i = 0; i < aequusPlayer.ExtraInventoryCount; i++) {
            if (!aequusPlayer.extraInventory[i].IsAir && !aequusPlayer.extraInventory[i].favorited && !aequusPlayer.extraInventory[i].IsACoin) {
                aequusPlayer.extraInventory[i] = Chest.PutItemInNearbyChest(aequusPlayer.extraInventory[i], player.Center);
            }
        }
    }

    private static void On_ChestUI_QuickStack(On_ChestUI.orig_QuickStack orig, ContainerTransferContext context, bool voidStack) {
        orig(context, voidStack);
        if (voidStack || !Main.LocalPlayer.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) || aequusPlayer.ExtraInventoryCount <= 0) {
            return;
        }

        var player = Main.LocalPlayer;
        var containerWorldPosition = context.GetContainerWorldPosition();

        var chest = player.GetCurrentChest();
        bool anyTransfers = false;
        if (chest != null) {
            for (int i = 0; i < aequusPlayer.ExtraInventoryCount; i++) {
                var item = aequusPlayer.extraInventory[i];
                if (item.IsAir || item.favorited) {
                    continue;
                }

                for (int j = 0; j < Chest.maxItems; j++) {
                    var chestItem = chest.item[j];
                    if (chestItem.IsAir || chestItem.type != item.type) {
                        continue;
                    }

                    ItemLoader.TryStackItems(chestItem, item, out var numTransferred);
                    anyTransfers |= numTransferred > 0;
                    if (context.CanVisualizeTransfers && numTransferred > 0) {
                        Chest.VisualizeChestTransfer(Main.LocalPlayer.Center, containerWorldPosition, item, numTransferred);
                    }
                    if (item.stack <= 0) {
                        item.TurnToAir();
                        break;
                    }
                }
            }
        }

        if (anyTransfers) {
            SoundEngine.PlaySound(SoundID.Grab);
        }
    }

    private static bool On_Player_ConsumeItem(On_Player.orig_ConsumeItem orig, Player player, int type, bool reverseOrder, bool includeVoidBag) {
        bool consumedItem = orig(player, type, reverseOrder, includeVoidBag);

        if (!consumedItem && includeVoidBag && player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.ExtraInventoryCount > 0) {
            for (int i = 0; i < aequusPlayer.ExtraInventoryCount; i++) {
                if (!aequusPlayer.extraInventory[i].IsAir && aequusPlayer.extraInventory[i].type == type && ItemLoader.ConsumeItem(aequusPlayer.extraInventory[i], player)) {
                    aequusPlayer.extraInventory[i].stack--;
                    if (aequusPlayer.extraInventory[i].stack <= 0) {
                        aequusPlayer.extraInventory[i].TurnToAir();
                    }
                }
            }
        }

        return consumedItem;
    }

    // TODO: Support Quick Buff

    private static Item On_Player_QuickMana_GetItemToUse(On_Player.orig_QuickMana_GetItemToUse orig, Player player) {
        var item = orig(player);

        if (item != null && player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.ExtraInventoryCount > 0) {
            for (int i = 0; i < aequusPlayer.ExtraInventoryCount; i++) {
                if (aequusPlayer.extraInventory[i].stack > 0 && aequusPlayer.extraInventory[i].type > ItemID.None && aequusPlayer.extraInventory[i].healMana > 0 && (player.potionDelay == 0 || !aequusPlayer.extraInventory[i].potion)) {
                    return aequusPlayer.extraInventory[i];
                }
            }
        }

        return item;
    }

    private static Item On_Player_QuickHeal_GetItemToUse(On_Player.orig_QuickHeal_GetItemToUse orig, Player player) {
        var item = orig(player);

        if (item != null && player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.ExtraInventoryCount > 0) {
            // TODO: Make this respect HP restoration priority?
            for (int i = 0; i < aequusPlayer.ExtraInventoryCount; i++) {
                if (aequusPlayer.extraInventory[i].stack > 0 && aequusPlayer.extraInventory[i].type > ItemID.None && aequusPlayer.extraInventory[i].potion && aequusPlayer.extraInventory[i].healLife > 0 && CombinedHooks.CanUseItem(player, aequusPlayer.extraInventory[i])) {
                    return aequusPlayer.extraInventory[i];
                }
            }
        }

        return item;
    }

    private static Item On_Player_QuickMount_GetItemToUse(On_Player.orig_QuickMount_GetItemToUse orig, Player player) {
        var item = orig(player);

        if (item != null && player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.ExtraInventoryCount > 0) {
            for (int i = 0; i < aequusPlayer.ExtraInventoryCount; i++) {
                if (aequusPlayer.extraInventory[i].mountType > MountID.None && !MountID.Sets.Cart[aequusPlayer.extraInventory[i].mountType] && CombinedHooks.CanUseItem(player, aequusPlayer.extraInventory[i])) {
                    return aequusPlayer.extraInventory[i];
                }
            }
        }

        return item;
    }
    #endregion
}