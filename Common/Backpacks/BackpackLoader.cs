using Aequus.Common.Golfing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Backpacks;

public class BackpackLoader {
    public const String BackpacksSaveKey = "Backpacks";

    public static readonly List<BackpackData> Backpacks = new();
    public static Int32 Count => Backpacks.Count;

    internal static Boolean IgnoreBackpacks { get; set; }

    public static void InitializeBackpacks(ref BackpackData[] backpacks) {
        backpacks = new BackpackData[Count];
        for (Int32 i = 0; i < backpacks.Length; i++) {
            backpacks[i] = Backpacks[i].CreateInstance();
        }
    }

    public static void SaveBackpacks(TagCompound tag, BackpackData[] backpacks, List<(String, TagCompound)> unloadedBackpacksList) {
        // Create a tag which will contain all of the individual backpack tag compounds.
        TagCompound backpacksSaveTag = new TagCompound();

        for (Int32 i = 0; i < backpacks.Length; i++) {
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

    public static void LoadBackpacks(TagCompound tag, BackpackData[] backpacks, List<(String, TagCompound)> unloadedBackpacksList) {
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
        for (Int32 i = 0; i < backpacks.Length; i++) {
            backpacks[i].Update(player);
        }
    }

    public static void ResetEffects(Player player, BackpackData[] backpacks) {
        for (Int32 i = 0; i < backpacks.Length; i++) {
            backpacks[i].ResetEffects(player);
        }
    }

    public static void UpdateInfoAccessories(Player player, BackpackData[] backpacks) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        for (Int32 i = 0; i < backpacks.Length; i++) {
            if (!backpacks[i].SupportsInfoAccessories || !backpacks[i].IsActive(player)) {
                continue;
            }
            UpdateSingleInfoAccessory(player, aequusPlayer, backpacks[i]);
        }
    }
    private static void UpdateSingleInfoAccessory(Player player, AequusPlayer aequusPlayer, BackpackData backpack) {
        for (Int32 i = 0; i < backpack.Inventory.Length; i++) {
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

    public static Boolean CanAcceptItem(Player player, BackpackData backpackData, Item item, Item backpackItem, Int32 slot) {
        return player.CanItemSlotAccept(backpackItem, item) && backpackData.CanAcceptItem(slot, item);
    }

    public static Boolean ItemSpace(Item item, Player player, BackpackData backpackData) {
        for (Int32 i = 0; i < backpackData.Inventory.Length; i++) {
            if (CanAcceptItem(player, backpackData, item, backpackData.Inventory[i], i)) {
                return true;
            }
        }
        return false;
    }

    public static Boolean GrabItem(Item item, Player player, BackpackData[] backpacks, Player.ItemSpaceStatus itemSpace) {
        Int32 transferredToBackpack = 0;
        for (Int32 i = 0; i < backpacks.Length; i++) {
            if (!backpacks[i].IsActive(player)) {
                continue;
            }
            for (Int32 j = 0; j < backpacks[i].Inventory.Length; j++) {
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
                    ItemLoader.TryStackItems(backpacks[i].Inventory[j], item, out Int32 transferred);
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
        for (Int32 i = 0; i < backpack.Inventory.Length; i++) {
            if (!backpack.Inventory[i].IsAir && !backpack.Inventory[i].favorited && !backpack.Inventory[i].IsACoin) {
                backpack.Inventory[i] = Chest.PutItemInNearbyChest(backpack.Inventory[i], location);
            }
        }
    }

    public static Boolean QuickStack(BackpackData backpack, Chest chest, Vector2 containerWorldPosition, ContainerTransferContext context) {
        Boolean anyTransfers = false;
        for (Int32 i = 0; i < backpack.Inventory.Length; i++) {
            Item item = backpack.Inventory[i];
            if (item.IsAir || item.favorited) {
                continue;
            }

            for (Int32 j = 0; j < Chest.maxItems; j++) {
                Item chestItem = chest.item[j];
                if (chestItem.IsAir || chestItem.type != item.type) {
                    continue;
                }

                ItemLoader.TryStackItems(chestItem, item, out Int32 numTransferred);
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

    public static void ConsumeItem(Player player, BackpackData backpack, Int32 type, Boolean reverseOrder) {
        for (Int32 i = 0; i < backpack.Inventory.Length; i++) {
            if (!backpack.Inventory[i].IsAir && backpack.Inventory[i].type == type && ItemLoader.ConsumeItem(backpack.Inventory[i], player)) {
                backpack.Inventory[i].stack--;
                if (backpack.Inventory[i].stack <= 0) {
                    backpack.Inventory[i].TurnToAir();
                }
            }
        }
    }

    public static Item GetQuickManaItem(Player player, BackpackData backpack) {
        for (Int32 i = 0; i < backpack.Inventory.Length; i++) {
            if (backpack.Inventory[i].stack > 0 && backpack.Inventory[i].type > ItemID.None && backpack.Inventory[i].healMana > 0 && (player.potionDelay == 0 || !backpack.Inventory[i].potion) && CombinedHooks.CanUseItem(player, backpack.Inventory[i])) {
                return backpack.Inventory[i];
            }
        }
        return null;
    }

    // TODO: Make this respect HP restoration priority?
    public static Item GetQuickHealItem(Player player, BackpackData backpack) {
        for (Int32 i = 0; i < backpack.Inventory.Length; i++) {
            if (backpack.Inventory[i].stack > 0 && backpack.Inventory[i].type > ItemID.None && backpack.Inventory[i].potion && backpack.Inventory[i].healLife > 0 && CombinedHooks.CanUseItem(player, backpack.Inventory[i])) {
                return backpack.Inventory[i];
            }
        }
        return null;
    }

    public static Item GetQuickMountItem(Player player, BackpackData backpack) {
        for (Int32 i = 0; i < backpack.Inventory.Length; i++) {
            if (backpack.Inventory[i].mountType > MountID.None && !MountID.Sets.Cart[backpack.Inventory[i].mountType] && CombinedHooks.CanUseItem(player, backpack.Inventory[i])) {
                return backpack.Inventory[i];
            }
        }
        return null;
    }

    public static IEnumerable<Item> GetExtraCraftingItems(Player player, BackpackData[] backpacks) {
        for (Int32 i = 0; i < backpacks.Length; i++) {
            if (!backpacks[i].IsActive(player)) {
                continue;
            }
            for (Int32 j = 0; j < backpacks[i].Inventory.Length; j++) {
                if (!backpacks[i].Inventory[j].IsAir) {
                    yield return backpacks[i].Inventory[j];
                }
            }
        }
    }

    public static void AnimateBackpacks(BackpackData[] backpacks, out Int32 totalInventorySlots, out Int32 activeBackpacks) {
        totalInventorySlots = 0;
        activeBackpacks = 0;
        Boolean anyBackpacks = false;
        for (Int32 i = 0; i < backpacks.Length; i++) {
            if (!backpacks[i].IsActive(Main.LocalPlayer) || !backpacks[i].IsVisible() || backpacks[i].Inventory == null) {
                if (backpacks[i].slotsToRender > 0) {
                    backpacks[i].slotsToRender--;
                    backpacks[i].nextSlotAnimation = 0f;
                    anyBackpacks = true;
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
            anyBackpacks = true;
        }

        if (!anyBackpacks || !Main.playerInventory) {
            return;
        }

        BackpackSlotsUI backpackUI = ModContent.GetInstance<BackpackSlotsUI>();
        if (!backpackUI.Active) {
            backpackUI.Activate();
        }
    }

    public static Boolean GetPreferredGolfBallToUse(Player player, BackpackData[] backpacks, ref Int32 projType) {
        for (Int32 k = 0; k < backpacks.Length; k++) {
            if (!backpacks[k].IsActive(player) || !backpacks[k].SupportsGolfBalls) {
                continue;
            }

            for (Int32 i = 0; i < backpacks[k].Inventory.Length; i++) {
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
    private static void LogUnloadedBackpacks(List<(String, TagCompound)> unloadedBackpacksList) {
        foreach (var backpack in unloadedBackpacksList) {
            Aequus.Log.Debug($"{backpack.Item1}: {backpack.Item2}");
        }
    }
}