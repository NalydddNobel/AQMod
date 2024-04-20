using Aequus.Core;
using Terraria.DataStructures;

namespace Aequus.Content.Tools.Keychain;

[WorkInProgress]
internal class MagicKeychain : Keychain {
    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Cyan;
        Item.value = Item.sellPrice(gold: 5);
        _keys = new();
    }

    public override bool AcceptItem(Item acceptedItem) {
        lock (_keys) {
            bool anyStacks = false;
            // If any stacks exist already, prevent accepting this item.
            foreach (Item item in _keys) {
                if (item.type == acceptedItem.type) {
                    return false;
                }
            }

            // Prevent adding new keys if the limit was reached.
            if (_keys.Count >= MAX_KEYS_ALLOWED) {
                return anyStacks;
            }

            // Add a single key onto the keychain.
            if (acceptedItem.stack > 0) {
                Item addedItem = acceptedItem.Clone();
                addedItem.stack = 1;
                _keys.Add(addedItem);

                acceptedItem.stack--;
                if (acceptedItem.stack <= 0) {
                    acceptedItem.TurnToAir();
                }

                return true;
            }
        }

        return false;
    }

    public override bool ConsumeKey(Player player, int type) {
        lock (_keys) {
            foreach (Item item in _keys) {
                // Return true if there's a key on the keychain.
                if (item.type == type) {
                    return true;
                }
            }
        }

        return true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Keychain>())
            .AddIngredient(ItemID.SpectreBar, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
