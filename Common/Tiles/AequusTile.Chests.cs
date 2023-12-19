using Aequus.Core.Autoloading;

namespace Aequus.Common.Tiles;

public partial class AequusTile : GlobalTile, IPostSetupContent {
    public static void CheckCustomKeys(int i, int j, int type) {
        if (type == TileID.Containers) {
            var player = Main.LocalPlayer;
            if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
                return;
            }

            bool unlockedChest = TileHelper.GetStyle(i, j, coordinateFullWidthBackup: 36) switch {
                ChestType.LockedShadow => TryUnlockingWithKey(i, j, aequusPlayer.shadowKey),
                ChestType.LockedGreenDungeon => TryUnlockingWithKey(i, j, aequusPlayer.goldenKey),
                ChestType.LockedPinkDungeon => TryUnlockingWithKey(i, j, aequusPlayer.goldenKey),
                ChestType.LockedBlueDungeon => TryUnlockingWithKey(i, j, aequusPlayer.goldenKey),
                ChestType.LockedGold => TryUnlockingWithKey(i, j, aequusPlayer.goldenKey),
                _ => false,
            };
        }
    }

    private static bool TryUnlockingWithKey(int i, int j, Item key) {
        if (key == null || key.IsAir) {
            return false;
        }

        i -= Main.tile[i, j].TileFrameX % 36 / 18;
        j -= Main.tile[i, j].TileFrameY % 36 / 18;
        if ((key.consumable || key.type != ItemID.ShadowKey) && ItemLoader.ConsumeItem(key, Main.LocalPlayer)) {
            key.stack--;
            if (key.stack <= 0) {
                key.TurnToAir();
            }
        }
        Chest.Unlock(i, j);
        return true;
    }
}