using Aequus.Common.Items.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Players;

public class BackpackLoader {
    public static readonly List<BackpackData> Backpacks = new();
    public static int Count => Backpacks.Count;

    private static readonly List<Item> _extraCraftingItems = new();
    internal static bool IgnoreBackpacks;

    public static void SaveBackpacks(TagCompound tag, AequusPlayer aequusPlayer) {
        for (int i = 0; i < aequusPlayer.backpacks.Length; i++) {
            aequusPlayer.backpacks[i].SaveData(tag);
        }
    }

    public static void LoadBackpacks(TagCompound tag, AequusPlayer aequusPlayer) {
        for (int i = 0; i < aequusPlayer.backpacks.Length; i++) {
            aequusPlayer.backpacks[i].LoadData(tag);
        }
    }

    public static void UpdateBackpacks(Player player, BackpackData[] backpacks) {
        for (int i = 0; i < backpacks.Length; i++) {
            backpacks[i].Update(player);
        }
    }

    public static void ResetEffects(Player player, BackpackData[] backpacks) {
        for (int i = 0; i < backpacks.Length; i++) {
            backpacks[i].ResetEffects(player);
        }
    }

    public static void ResetInfoAccessories(Player player, AequusPlayer aequusPlayer, BackpackData backpack) {
        for (int i = 0; i < backpack.Inventory.Length; i++) {
            if (ItemID.Sets.WorksInVoidBag[backpack.Inventory[i].type]) {
                ItemLoader.UpdateInventory(backpack.Inventory[i], player);
                player.RefreshInfoAccsFromItemType(backpack.Inventory[i]);
                player.RefreshMechanicalAccsFromItemType(backpack.Inventory[i].type);
            }
            if (backpack.Inventory[i].type == ItemID.GoldenKey && aequusPlayer.goldenKey == null) {
                aequusPlayer.goldenKey = backpack.Inventory[i];
            }
            if (backpack.Inventory[i].type == ItemID.ShadowKey && (aequusPlayer.shadowKey == null || aequusPlayer.shadowKey.consumable)) {
                aequusPlayer.shadowKey = backpack.Inventory[i];
            }
            if (backpack.Inventory[i].type == ItemID.Football) {
                player.hasFootball = true;
            }
            backpack.UpdateItem(player, aequusPlayer, i);
        }
    }

    public static bool CanAcceptItem(Player player, BackpackData backpackData, Item item, Item backpackItem, int slot) {
        return player.CanItemSlotAccept(backpackItem, item) && backpackData.CanAcceptItem(slot, item);
    }

    public static bool ItemSpace(Item item, Player player, BackpackData backpackData) {
        for (int i = 0; i < backpackData.Inventory.Length; i++) {
            if (CanAcceptItem(player, backpackData, item, backpackData.Inventory[i], i)) {
                return true;
            }
        }
        return false;
    }

    public static bool GrabItem(Item item, Player player, BackpackData[] backpacks, Player.ItemSpaceStatus itemSpace) {
        int transferredToBackpack = 0;
        for (int i = 0; i < backpacks.Length; i++) {
            if (!backpacks[i].IsActive(player) || !CanAcceptItem(player, backpacks[i], item, backpacks[i].Inventory[i], i)) {
                continue;
            }

            if (backpacks[i].Inventory[i].IsAir && (!itemSpace.CanTakeItem || itemSpace.ItemIsGoingToVoidVault)) {
                backpacks[i].Inventory[i] = item.Clone();
                transferredToBackpack = item.stack;
                item.stack = 0;
                break;
            }

            ItemLoader.StackItems(backpacks[i].Inventory[i], item, out int transferred);
            transferredToBackpack += transferred;
            if (item.stack <= 0) {
                break;
            }
        }

        if (transferredToBackpack > 0) {
            PopupText.NewText(PopupTextContext.RegularItemPickup, item, transferredToBackpack);
            SoundEngine.PlaySound(SoundID.Grab);
            if (item.stack <= 0) {
                item.TurnToAir();
                return true;
            }
        }

        return false;
    }

    public static void QuickStackToNearbyChest(Vector2 location, BackpackData backpack) {
        for (int i = 0; i < backpack.Inventory.Length; i++) {
            if (!backpack.Inventory[i].IsAir && !backpack.Inventory[i].favorited && !backpack.Inventory[i].IsACoin) {
                backpack.Inventory[i] = Chest.PutItemInNearbyChest(backpack.Inventory[i], location);
            }
        }
    }

    public static bool QuickStack(BackpackData backpack, Chest chest, Vector2 containerWorldPosition, ContainerTransferContext context) {
        bool anyTransfers = false;
        for (int i = 0; i < backpack.Inventory.Length; i++) {
            var item = backpack.Inventory[i];
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
        return anyTransfers;
    }

    public static void ConsumeItem(Player player, BackpackData backpack, int type, bool reverseOrder) {
        for (int i = 0; i < backpack.Inventory.Length; i++) {
            if (!backpack.Inventory[i].IsAir && backpack.Inventory[i].type == type && ItemLoader.ConsumeItem(backpack.Inventory[i], player)) {
                backpack.Inventory[i].stack--;
                if (backpack.Inventory[i].stack <= 0) {
                    backpack.Inventory[i].TurnToAir();
                }
            }
        }
    }

    public static Item GetQuickManaItem(Player player, BackpackData backpack) {
        for (int i = 0; i < backpack.Inventory.Length; i++) {
            if (backpack.Inventory[i].stack > 0 && backpack.Inventory[i].type > ItemID.None && backpack.Inventory[i].healMana > 0 && (player.potionDelay == 0 || !backpack.Inventory[i].potion) && CombinedHooks.CanUseItem(player, backpack.Inventory[i])) {
                return backpack.Inventory[i];
            }
        }
        return null;
    }

    // TODO: Make this respect HP restoration priority?
    public static Item GetQuickHealItem(Player player, BackpackData backpack) {
        for (int i = 0; i < backpack.Inventory.Length; i++) {
            if (backpack.Inventory[i].stack > 0 && backpack.Inventory[i].type > ItemID.None && backpack.Inventory[i].potion && backpack.Inventory[i].healLife > 0 && CombinedHooks.CanUseItem(player, backpack.Inventory[i])) {
                return backpack.Inventory[i];
            }
        }
        return null;
    }

    public static Item GetQuickMountItem(Player player, BackpackData backpack) {
        for (int i = 0; i < backpack.Inventory.Length; i++) {
            if (backpack.Inventory[i].mountType > MountID.None && !MountID.Sets.Cart[backpack.Inventory[i].mountType] && CombinedHooks.CanUseItem(player, backpack.Inventory[i])) {
                return backpack.Inventory[i];
            }
        }
        return null;
    }

    public static List<Item> GetExtraCraftingItems(AequusPlayer aequusPlayer) {
        _extraCraftingItems.Clear();
        for (int i = 0; i < aequusPlayer.backpacks.Length; i++) {
            if (!aequusPlayer.backpacks[i].IsActive(aequusPlayer.Player)) {
                continue;
            }
            for (int j = 0; j < aequusPlayer.backpacks[i].Inventory.Length; j++) {
                if (!aequusPlayer.backpacks[i].Inventory[j].IsAir) {
                    _extraCraftingItems.Add(aequusPlayer.backpacks[i].Inventory[j]);
                }
            }
        }
        return _extraCraftingItems;
    }

    public static void AnimateBackpacks(BackpackData[] backpacks, out int totalInventorySlots, out int activeBackpacks) {
        totalInventorySlots = 0;
        activeBackpacks = 0;
        for (int i = 0; i < backpacks.Length; i++) {
            if (!backpacks[i].IsActive(Main.LocalPlayer) || !backpacks[i].IsVisible() || backpacks[i].Inventory == null) {
                if (backpacks[i].slotsToRender > 0) {
                    backpacks[i].slotsToRender--;
                    backpacks[i].nextSlotAnimation = 0f;
                }
                continue;
            }

            if (backpacks[i].slotsToRender < backpacks[i].Inventory.Length) {
                backpacks[i].nextSlotAnimation += 0.09f + backpacks[i].Inventory.Length * 0.015f;
                if (backpacks[i].nextSlotAnimation > 1f) {
                    backpacks[i].nextSlotAnimation = 0f;
                    backpacks[i].slotsToRender++;
                }
            }
            else if (backpacks[i].slotsToRender > backpacks[i].Inventory.Length) {
                backpacks[i].slotsToRender--;
                backpacks[i].nextSlotAnimation = 0f;
            }

            totalInventorySlots += backpacks[i].Inventory.Length;
            activeBackpacks++;
        }
    }

    public static void SetBackpack<T>(Player player, IStorageItem storageItem, int slotAmount) where T : BackpackItemData {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return;
        }
        storageItem.EnsureInventory(slotAmount);
        (aequusPlayer.backpacks[ModContent.GetInstance<T>().Type] as T).BackpackItem = storageItem;
    }

    public static T Get<T>(AequusPlayer player) where T : BackpackData {
        return (T)player.backpacks[ModContent.GetInstance<T>().Type];
    }
    public static T Get<T>(Player player) where T : BackpackData {
        return Get<T>(player.GetModPlayer<AequusPlayer>());
    }
}