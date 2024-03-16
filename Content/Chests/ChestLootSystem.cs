namespace Aequus.Content.Chests;

public class ChestLootSystem : ModSystem {
    public override void PostWorldGen() {
        int unopenedChestItemId = ModContent.ItemType<UnopenedChestItem>();

        for (int i = 0; i < Main.maxChests; i++) {
            if (Main.chest[i] == null) {
                continue;
            }

            Chest chest = Main.chest[i];
            // Find an empty slot starting from the last slot.
            for (int k = chest.item.Length - 1; k > 0; k++) {
                if (chest.item[k] == null || chest.item[k].IsAir) {
                    // Fill the last empty slot with the unopened chest item.
                    chest.item[k].SetDefaults(unopenedChestItemId);
                    break;
                }
            }
        }
    }
}
