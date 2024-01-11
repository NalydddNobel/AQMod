using Aequus.Common.Golfing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Backpacks;

public class BackpackLoader {
    public const string BackpacksSaveKey = "Backpacks";
    
    public static readonly List<BackpackData> Backpacks = new();
    public static int Count => Backpacks.Count;

    internal static bool IgnoreBackpacks { get; set; }

    public static void InitializeBackpacks(ref BackpackData[] backpacks) {
        backpacks = new BackpackData[Count];
        for (int i = 0; i < backpacks.Length; i++) {
            backpacks[i] = Backpacks[i].CreateInstance();
        }
    }

    public static void SaveBackpacks(TagCompound tag, BackpackData[] backpacks, List<(string, TagCompound)> unloadedBackpacksList) {
        // Create a tag which will contain all of the individual backpack tag compounds.
        TagCompound backpacksSaveTag = new TagCompound();
        
        for (int i = 0; i < backpacks.Length; i++) {
            // Create backpack tag compound, and save data to it
            TagCompound backpackTag = new TagCompound();
            backpacks[i].SaveData(backpackTag);

            // If any data is saved, add this tag to the main tag.
            if (backpackTag.Count != 0) {
                backpacksSaveTag[backpacks[i].FullName] = backpackTag;
            }
        }

        // Save backpacks if any backpacks were saved.
        if (backpacksSaveTag.Count != 0) {
            tag[BackpacksSaveKey] = backpacksSaveTag;
        }
    }

    public static void LoadBackpacks(TagCompound tag, BackpackData[] backpacks, List<(string, TagCompound)> unloadedBackpacksList) {
        if (!tag.TryGet(BackpacksSaveKey, out TagCompound backpacksSaveTag)) {
            return;
        }

        foreach (var pair in backpacksSaveTag) {
            if (pair.Value is not TagCompound backpackTag) {
                continue;
            }

            // Find backpack with matching name.
            var backpack = Array.Find(backpacks, (b) => b.FullName.Equals(pair.Key));

            // If found, load data.
            if (backpack != null) {
                backpack.LoadData(backpackTag);
                continue;
            }

            // Otherwise add to unloaded backpacks list.
            unloadedBackpacksList.Add((pair.Key, backpackTag));
        }
        LogUnloadedBackpacks(unloadedBackpacksList);
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

    public static void UpdateInfoAccessories(Player player, BackpackData[] backpacks) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        for (int i = 0; i < backpacks.Length; i++) {
            if (!backpacks[i].SupportsInfoAccessories || !backpacks[i].IsActive(player)) {
                continue;
            }
            UpdateSingleInfoAccessory(player, aequusPlayer, backpacks[i]);
        }
    }
    private static void UpdateSingleInfoAccessory(Player player, AequusPlayer aequusPlayer, BackpackData backpack) {
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
            if (!backpacks[i].IsActive(player)) {
                continue;
            }
            for (int j = 0; j < backpacks[i].Inventory.Length; j++) {
                if (!CanAcceptItem(player, backpacks[i], item, backpacks[i].Inventory[j], i)) {
                    continue;
                }

                if (backpacks[i].Inventory[j].IsAir) {
                    if (!itemSpace.CanTakeItem || itemSpace.ItemIsGoingToVoidVault) {
                        backpacks[i].Inventory[j] = item.Clone();
                        transferredToBackpack = item.stack;
                        item.stack = 0;
                        break;
                    }
                    continue;
                }

                if (backpacks[i].Inventory[j].type == item.type) {
                    ItemLoader.TryStackItems(backpacks[i].Inventory[j], item, out int transferred);
                    transferredToBackpack += transferred;
                }

                if (item.stack <= 0) {
                    break;
                }
            }

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
            Item item = backpack.Inventory[i];
            if (item.IsAir || item.favorited) {
                continue;
            }

            for (int j = 0; j < Chest.maxItems; j++) {
                Item chestItem = chest.item[j];
                if (chestItem.IsAir || chestItem.type != item.type) {
                    continue;
                }

                ItemLoader.TryStackItems(chestItem, item, out int numTransferred);
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

    public static IEnumerable<Item> GetExtraCraftingItems(Player player, BackpackData[] backpacks) {
        for (int i = 0; i < backpacks.Length; i++) {
            if (!backpacks[i].IsActive(player)) {
                continue;
            }
            for (int j = 0; j < backpacks[i].Inventory.Length; j++) {
                if (!backpacks[i].Inventory[j].IsAir) {
                    yield return backpacks[i].Inventory[j];
                }
            }
        }
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

    public static bool GetPreferredGolfBallToUse(Player player, BackpackData[] backpacks, ref int projType) {
        for (int k = 0; k < backpacks.Length; k++) {
            if (!backpacks[k].IsActive(player) || !backpacks[k].SupportsGolfBalls) {
                continue;
            }

            for (int i = 0; i < backpacks[k].Inventory.Length; i++) {
                Item item = backpacks[k].Inventory[i];
                if (!item.IsAir && (projType == ProjectileID.DirtGolfBall && ProjectileID.Sets.IsAGolfBall[item.shoot] || (ProjectileLoader.GetProjectile(item.shoot) as IGolfBallProjectile)?.OverrideGolfBallId(player, item, projType) == true)) {
                    projType = item.shoot;
                }
            }
        }
        return false;
    }

    public static BackpackData Get(BackpackPlayer player, BackpackData backpack) {
        return player.backpacks[backpack.Type];
    }
    public static BackpackData Get(Player player, BackpackData backpack) {
        return Get(player.GetModPlayer<BackpackPlayer>(), backpack);
    }
    public static T Get<T>(BackpackPlayer player) where T : BackpackData {
        return (T)Get(player, ModContent.GetInstance<T>());
    }
    public static T Get<T>(Player player) where T : BackpackData {
        return Get<T>(player.GetModPlayer<BackpackPlayer>());
    }

    [Conditional("DEBUG")]
    private static void LogUnloadedBackpacks(List<(string, TagCompound)> unloadedBackpacksList) {
        foreach (var backpack in unloadedBackpacksList) {
            Aequus.Log.Debug($"{backpack.Item1}: {backpack.Item2}");
        }
    }
}