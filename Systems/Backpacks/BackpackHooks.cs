using Aequus.Common;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.UI;

namespace Aequus.Systems.Backpacks;

public class BackpackHooks : LoadedType {
    protected override void Load() {
        On_ChestUI.QuickStack += On_ChestUI_QuickStack;
#if GOLF_BALL
        On_Player.GetPreferredGolfBallToUse += On_Player_GetPreferredGolfBallToUse;
#endif
        On_Player.QuickStackAllChests += On_Player_QuickStackAllChests;
        On_Player.ConsumeItem += On_Player_ConsumeItem;
        On_Player.QuickMount_GetItemToUse += On_Player_QuickMount_GetItemToUse;
        On_Player.QuickHeal_GetItemToUse += On_Player_QuickHeal_GetItemToUse;
        On_Player.QuickMana_GetItemToUse += On_Player_QuickMana_GetItemToUse;
    }

    static void On_ChestUI_QuickStack(On_ChestUI.orig_QuickStack orig, ContainerTransferContext context, bool voidStack) {
        orig(context, voidStack);

        if (voidStack || !Main.LocalPlayer.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return;
        }

        Player player = Main.LocalPlayer;
        Vector2 containerWorldPosition = context.GetContainerWorldPosition();

        Chest chest = player.GetCurrentChest();
        bool anyTransfers = false;

        if (chest != null) {
            for (int i = 0; i < backpackPlayer.backpacks.Length; i++) {
                if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsQuickStack) {
                    continue;
                }

                anyTransfers |= BackpackLoader.QuickStack(backpackPlayer.backpacks[i], chest, containerWorldPosition, context);
            }
        }

        if (anyTransfers) {
            SoundEngine.PlaySound(SoundID.Grab);
        }
    }

    static bool On_Player_ConsumeItem(On_Player.orig_ConsumeItem orig, Player player, int type, bool reverseOrder, bool includeVoidBag) {
        bool consumedItem = orig(player, type, reverseOrder, includeVoidBag);

        if (consumedItem) {
            return true;
        }

        // Check Backpacks for consuming an item.
        if (includeVoidBag && player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            for (int i = 0; i < backpackPlayer.backpacks.Length; i++) {
                if (backpackPlayer.backpacks[i].IsActive(player) && backpackPlayer.backpacks[i].SupportsConsumeItem && BackpackLoader.ConsumeItem(player, backpackPlayer.backpacks[i], type, reverseOrder)) {
                    return true;
                }
            }
        }

#if KEYCHAIN
        // Check Keychain for consuming an item.
        if (player.TryGetModPlayer(out KeychainPlayer keychain) && keychain.ConsumeKey(player, type) == true) {
            keychain.RefreshKeys();
            return true;
        }
#endif

        return false;
    }

    static Item On_Player_QuickHeal_GetItemToUse(On_Player.orig_QuickHeal_GetItemToUse orig, Player player) {
        Item item = orig(player);

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

#if GOLF_BALL
    private static void On_Player_GetPreferredGolfBallToUse(On_Player.orig_GetPreferredGolfBallToUse orig, Player player, out int projType) {
        orig(player, out projType);

        Item heldItem = player.HeldItem;
        if (!heldItem.IsAir && heldItem.shoot == projType) {
            return;
        }

        if (!heldItem.IsAir && (ProjectileLoader.GetProjectile(heldItem.shoot) as IGolfBallProjectile)?.OverrideGolfBallId(player, heldItem, projType) == true) {
            projType = heldItem.shoot;
            return;
        }

        for (int i = 0; i < Main.InventorySlotsTotal; i++) {
            Item item = player.inventory[i];
            if (!item.IsAir && (ProjectileLoader.GetProjectile(item.shoot) as IGolfBallProjectile)?.OverrideGolfBallId(player, item, projType) == true) {
                projType = item.shoot;
            }
        }

        BackpackLoader.GetPreferredGolfBallToUse(player, player.GetModPlayer<BackpackPlayer>().backpacks, ref projType);
    }
#endif
}
