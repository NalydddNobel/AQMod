using System.Collections.Generic;
using System.Linq;

namespace Aequus.Common.Entities.Containers;

public class StorageItemPlayer : ModPlayer {
    internal ICustomStorage? Storage;

    public void SetStorage(ICustomStorage? storage) {
        if (Storage == storage) {
            storage = null;
        }

        if (storage != null) {
            Player.SetTalkNPC(-1);

            // Random client garbage.
            if (Main.myPlayer == Player.whoAmI) {
                Main.SetNPCShopIndex(0);
                Main.playerInventory = true;
                /*
                UILinkPointNavigator.ForceMovementCooldown(120);
                if (PlayerInput.GrappleAndInteractAreShared) {
                    PlayerInput.Triggers.JustPressed.Grapple = false;
                }
                */
                Main.recBigList = false;

                Instance<StorageInterface>().Activate();
            }
        }
        else if (Main.myPlayer == Player.whoAmI) {
            Instance<StorageInterface>().Deactivate();
        }

        // Close previous storage. (If not null.)
        Storage?.Close(Player);

        Storage = storage;

        // Open new storage. (If not null.)
        Storage?.Open(Player);

        Recipe.FindRecipes();
    }

    public override IEnumerable<Item>? AddMaterialsForCrafting(out ItemConsumedCallback? itemConsumedCallback) {
        itemConsumedCallback = null;
        return Storage?.Items?.Where(i => i != null && !i.IsAir);
    }
}
