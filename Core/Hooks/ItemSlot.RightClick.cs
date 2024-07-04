using AequusRemake.Core.Entities.Items.Components;
using AequusRemake.DataSets;
using Terraria.UI;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for:
    /// <list type="number">
    /// <item>
    /// Right click overrides for held modded items which implement <see cref="IRightClickOverrideWhenHeld"/>.
    /// </item>
    /// <item>
    /// Handling custom golden key and shadow key logic.
    /// </item>
    /// </list>
    /// </summary>
    private static void ItemSlot_RightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        if (Main.mouseRight && Main.mouseRightRelease) {
            Player player = Main.LocalPlayer;
            AequusPlayer AequusRemake = player.GetModPlayer<AequusPlayer>();
            if (!Main.mouseItem.IsAir && Main.mouseItem.ModItem is IRightClickOverrideWhenHeld hold && hold.RightClickOverrideWhileHeld(ref Main.mouseItem, inv, context, slot, player, AequusRemake)) {
                Main.mouseRightRelease = false;
                // Set stack split delay to 3 seconds (so you don't instantly pick up the item with rclick)
                Main.stackSplit = 180;
                return;
            }
            if (!inv[slot].IsAir && inv[slot].ModItem is IRightClickOverrideWhenHovered hover && hover.RightClickOverrideWhenHovered(ref Main.mouseItem, inv, context, slot, player, AequusRemake)) {
                Main.mouseRightRelease = false;
                // Set stack split delay to 3 seconds (so you don't instantly pick up the item with rclick)
                Main.stackSplit = 180;
                return;
            }

            if (context == ItemSlot.Context.InventoryItem) {
                if (UseGoldenKey(inv, slot, player, AequusRemake)) {
                    return;
                }
                if (UseShadowKey(inv, slot, player, AequusRemake)) {
                    return;
                }
            }
        }

        orig(inv, context, slot);
    }

    private static bool UseGoldenKey(Item[] inv, int slot, Player Player, AequusPlayer AequusRemake) {
        Item goldenKey = AequusRemake.goldenKey;
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

    private static bool UseShadowKey(Item[] inv, int slot, Player Player, AequusPlayer AequusRemake) {
        Item shadowKey = AequusRemake.shadowKey;
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
}
