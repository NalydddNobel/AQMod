using AequusRemake.Content.Backpacks;
using AequusRemake.Content.Items.Tools.Keychain;
using AequusRemake.Core.Structures.Enums;

namespace AequusRemake.Core.Entities.Tiles;
public class LockedTiles : GlobalTile {
    public static void CheckVanillaKeyInteractions(int i, int j, int type) {
        Tile tile = Main.tile[i, j];
        bool unlockedChest = ChestStyleConversion.ToEnum(tile) switch {
            ChestStyle.LockedJungle => TryUnlockingChestWithKey(i, j, ItemID.JungleKey),
            ChestStyle.LockedIce => TryUnlockingChestWithKey(i, j, ItemID.FrozenKey),
            ChestStyle.LockedHallow => TryUnlockingChestWithKey(i, j, ItemID.HallowedKey),
            ChestStyle.LockedDesert => TryUnlockingChestWithKey(i, j, ItemID.DungeonDesertKey),
            ChestStyle.LockedCrimson => TryUnlockingChestWithKey(i, j, ItemID.CrimsonKey),
            ChestStyle.LockedCorruption => TryUnlockingChestWithKey(i, j, ItemID.CorruptionKey),
            ChestStyle.LockedShadow => TryUnlockingChestWithKey(i, j, ItemID.ShadowKey),
            ChestStyle.LockedGreenDungeon => TryUnlockingChestWithKey(i, j, ItemID.GoldenKey),
            ChestStyle.LockedPinkDungeon => TryUnlockingChestWithKey(i, j, ItemID.GoldenKey),
            ChestStyle.LockedBlueDungeon => TryUnlockingChestWithKey(i, j, ItemID.GoldenKey),
            ChestStyle.LockedGold => TryUnlockingChestWithKey(i, j, ItemID.GoldenKey),
            _ => false,
        };
        if (type == TileID.ClosedDoor) {
            if (tile.TileFrameY >= 594 && tile.TileFrameY <= 646 && tile.TileFrameX < 54) {
                TryUnlockingDoorWithKey(i, j, ItemID.TempleKey);
            }
        }
    }

    public override void RightClick(int i, int j, int type) {
        CheckVanillaKeyInteractions(i, j, type);
    }

    private static bool TryUnlockingDoorWithKey(int i, int j, int key) {
        if (!ConsumeKey(key)) {
            return false;
        }

        i -= Main.tile[i, j].TileFrameX % 36 / 18;
        j -= Main.tile[i, j].TileFrameY % 36 / 18;

        WorldGen.UnlockDoor(i, j);
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            NetMessage.SendData(MessageID.LockAndUnlock, number: Main.myPlayer, number2: 2f, number3: i, number4: j);
        }
        return true;
    }

    private static bool TryUnlockingChestWithKey(int i, int j, int key) {
        if (!ConsumeKey(key)) {
            return false;
        }

        i -= Main.tile[i, j].TileFrameX % 36 / 18;
        j -= Main.tile[i, j].TileFrameY % 36 / 18;

        Chest.Unlock(i, j);
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            NetMessage.SendData(MessageID.LockAndUnlock, number: Main.myPlayer, number2: 1f, number3: i, number4: j);
        }
        return true;
    }

    private static bool ConsumeKey(int key) {
        Player player = Main.LocalPlayer;
        if (player.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            for (int k = 0; k < backpackPlayer.backpacks.Length; k++) {
                if (backpackPlayer.backpacks[k].IsActive(player) && backpackPlayer.backpacks[k].SupportsConsumeItem && BackpackLoader.ConsumeItem(player, backpackPlayer.backpacks[k], key, reverseOrder: false)) {
                    return true;
                }
            }
        }

        if (player.TryGetModPlayer(out KeychainPlayer keychain) && keychain.ConsumeKey(player, key) == true) {
            keychain.RefreshKeys();
            return true;
        }

        return false;
    }
}
