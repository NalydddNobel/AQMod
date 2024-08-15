namespace Aequus.Content.Chests;

/// <summary>Item used to determine if a chest has been opened or not.</summary>
public class UnopenedChestItem : ModItem {
    public override string Texture => AequusTextures.None.FullPath;

    public override void Load() {
        On_Player.OpenChest += RemoveUnopenedChestItemUponOpeningChest;
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
    }

    private static void RemoveUnopenedChestItemUponOpeningChest(On_Player.orig_OpenChest orig, Player self, int x, int y, int newChest) {
        Remove(newChest);
        orig(self, x, y, newChest);
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (Main.LocalPlayer.chest > 0) {
            Item.TurnToAir();
        }
        return false;
    }

    public static void Remove(int chestId) {
        if (Main.chest.IndexInRange(chestId)) {
            int unopenedChestItemId = ModContent.ItemType<UnopenedChestItem>();
            Chest chest = Main.chest[chestId];
            for (int i = 0; i < chest.item.Length; i++) {
                Item item = chest.item[i];
                if (item != null && item.type == unopenedChestItemId) {
                    chest.item[i].TurnToAir();
                }
            }
        }
    }

    public static void Place(Chest chest) {
        // Find an empty slot starting from the last slot.
        Item[] items = chest.item;
        for (int k = items.Length - 1; k > 0; k--) {
            Item item = items[k];
            if (item == null || item.IsAir) {
                // Fill the last empty slot with the unopened chest item.
                (items[k] ??= new()).SetDefaults(ModContent.ItemType<UnopenedChestItem>());
                break;
            }
        }
    }
}

public class UnopenedChestItemPlayer : ModPlayer {
    public override void PostUpdate() {
        if (Player.chest >= 0) {
            UnopenedChestItem.Remove(Player.chest);
        }
    }
}