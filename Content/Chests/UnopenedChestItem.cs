namespace Aequus.Content.Chests;

/// <summary>Item used to determine if a chest has been opened or not.</summary>
public class UnopenedChestItem : ModItem {
    public override string Texture => AequusTextures.None.Path;

    public override void Load() {
        On_Player.OpenChest += RemoveUnopenedChestItemUponOpeningChest;
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
    }

    private static void RemoveUnopenedChestItemUponOpeningChest(On_Player.orig_OpenChest orig, Player self, int x, int y, int newChest) {
        if (Main.chest.IndexInRange(newChest)) {
            int unopenedChestItemId = ModContent.ItemType<UnopenedChestItem>();
            Chest chest = Main.chest[newChest];
            for (int i = 0; i < chest.item.Length; i++) {
                Item item = chest.item[i];
                if (item.type == unopenedChestItemId) {
                    chest.item[i].TurnToAir();
                }
            }
        }
        orig(self, x, y, newChest);
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (Main.LocalPlayer.chest > 0) {
            Item.TurnToAir();
        }
        return false;
    }
}
